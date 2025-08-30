using System;
using System.Collections.Generic;
using System.Linq;
using LPModelsLibrary.Models;

namespace LPModelsLibrary.Models
{
    public class RevisedPrimalSimplex
    {
        public static SimplexResult Solve(double[,] A, double[] b, double[] c, double eps = 1e-9)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);

            if (b.Any(x => x < -eps))
            {
                return new SimplexResult
                {
                    IsInfeasible = true,
                    Message = "Infeasible: RHS contains negative values. Phase I required."
                };
            }

            // Initial basis (slack variables)
            int[] basis = Enumerable.Range(n, m).ToArray();

            // Initial B inverse (identity)
            double[,] B_inv = new double[m, m];
            for (int i = 0; i < m; i++) B_inv[i, i] = 1.0;

            // Initial x_B = b
            double[] x_B = (double[])b.Clone();

            // Headers
            string[] colHeaders = new string[n + m + 1];
            for (int j = 0; j < n; j++) colHeaders[j] = $"x{j + 1}";
            for (int j = 0; j < m; j++) colHeaders[n + j] = $"s{j + 1}";
            colHeaders[n + m] = "rhs";

            string[] rowHeaders = new string[m + 1];
            for (int i = 0; i < m; i++) rowHeaders[i] = colHeaders[basis[i]];
            rowHeaders[m] = "z";

            var result = new SimplexResult();
            int iteration = 0;
            const int maxIterations = 100;

            // Build initial tableau for display
            double[,] initialT = BuildInitialTableau(A, b, c, m, n, eps);
            result.Tableaus.Add(new TableauTemplate(initialT, colHeaders, rowHeaders, iteration, null, null, "Initial tableau"));

            while (iteration < maxIterations)
            {
                // Compute reduced costs
                double[] reducedCosts = new double[n + m];
                int enteringVar = -1;

                // Bland's Rule: pick smallest index with positive reduced cost
                for (int j = 0; j < n + m; j++)
                {
                    if (basis.Contains(j)) continue;

                    double[] a_j = GetColumn(A, j, m, n);
                    double[] B_inv_a_j = MultiplyMatrixVector(B_inv, a_j);

                    double c_B_dot_B_inv_a_j = 0;
                    for (int i = 0; i < m; i++)
                    {
                        double c_B_i = (basis[i] < n) ? c[basis[i]] : 0.0;
                        c_B_dot_B_inv_a_j += c_B_i * B_inv_a_j[i];
                    }

                    reducedCosts[j] = GetCValue(c, j, n) - c_B_dot_B_inv_a_j;

                    if (reducedCosts[j] > eps && enteringVar == -1)
                    {
                        enteringVar = j; // first positive reduced cost → Bland’s rule
                    }
                }

                // Check for optimality
                if (enteringVar == -1)
                {
                    // Compute optimal value
                    result.OptimalValue = DotProduct(GetCB(c, basis, n, m), x_B);

                    // Assign variables
                    result.PrimalVariables = new double[n];
                    result.SlackExcessVariables = new double[m];
                    for (int i = 0; i < m; i++)
                    {
                        int basicVar = basis[i];
                        if (basicVar < n)
                            result.PrimalVariables[basicVar] = x_B[i];
                        else
                            result.SlackExcessVariables[basicVar - n] = x_B[i];
                    }

                    double[,] finalT = BuildTableau(A, b, c, B_inv, basis, x_B, m, n, eps);
                    result.Tableaus.Add(new TableauTemplate(finalT, colHeaders, rowHeaders, iteration, null, null, "Optimal tableau"));
                    result.Message = "Optimal solution found.";
                    break;
                }

                // Compute a_hat = B_inv * a_entering
                double[] a_entering = GetColumn(A, enteringVar, m, n);
                double[] a_hat = MultiplyMatrixVector(B_inv, a_entering);

                // Ratio test for leaving variable
                int leavingVarRow = -1;
                double minRatio = double.PositiveInfinity;
                for (int i = 0; i < m; i++)
                {
                    if (a_hat[i] > eps)
                    {
                        double ratio = x_B[i] / a_hat[i];
                        if (ratio < minRatio - eps && ratio >= 0)
                        {
                            minRatio = ratio;
                            leavingVarRow = i;
                        }
                    }
                }

                if (leavingVarRow == -1)
                {
                    result.IsUnbounded = true;
                    result.Message = "Unbounded: no valid leaving variable.";
                    double[,] ubT = BuildTableau(A, b, c, B_inv, basis, x_B, m, n, eps);
                    result.Tableaus.Add(new TableauTemplate(ubT, colHeaders, rowHeaders, iteration, null, enteringVar, "Unbounded"));
                    break;
                }

                // Update basis
                basis[leavingVarRow] = enteringVar;

                // Rebuild B from new basis
                double[,] B = new double[m, m];
                for (int col = 0; col < m; col++)
                {
                    double[] a_col = GetColumn(A, basis[col], m, n);
                    for (int i = 0; i < m; i++) B[i, col] = a_col[i];
                }

                // Recompute B_inv and x_B
                B_inv = InvertMatrix(B);
                x_B = MultiplyMatrixVector(B_inv, b);

                // Update row headers
                rowHeaders[leavingVarRow] = colHeaders[basis[leavingVarRow]];

                // Add tableau for this iteration
                double[,] currentT = BuildTableau(A, b, c, B_inv, basis, x_B, m, n, eps);
                result.Tableaus.Add(new TableauTemplate(currentT, colHeaders, rowHeaders, iteration, leavingVarRow, enteringVar));

                iteration++;
            }

            if (iteration >= maxIterations)
            {
                result.Message = "Maximum iterations reached, possible convergence failure.";
            }

            return result;
        }

        private static double[,] BuildInitialTableau(double[,] A, double[] b, double[] c, int m, int n, double eps)
        {
            int cols = n + m + 1;
            int rows = m + 1;
            double[,] T = new double[rows, cols];

            // Objective row (z) - negative coefficients for maximization
            for (int j = 0; j < n; j++) T[0, j] = -c[j];
            for (int j = n; j < n + m; j++) T[0, j] = 0.0;
            T[0, cols - 1] = 0.0;

            // Constraint rows with slack variables
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) T[i + 1, j] = A[i, j];
                for (int j = 0; j < m; j++) T[i + 1, n + j] = (i == j) ? 1.0 : 0.0;
                T[i + 1, cols - 1] = b[i];
            }

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (Math.Abs(T[i, j]) < eps) T[i, j] = 0.0;

            return T;
        }

        private static double[,] BuildTableau(double[,] A, double[] b, double[] c, double[,] B_inv, int[] basis, double[] x_B, int m, int n, double eps)
        {
            int cols = n + m + 1;
            int rows = m + 1;
            double[,] T = new double[rows, cols];

            // Objective row (reduced costs)
            for (int j = 0; j < n + m; j++)
            {
                if (basis.Contains(j))
                {
                    T[0, j] = 0;
                }
                else
                {
                    double[] a_j = GetColumn(A, j, m, n);
                    double[] B_inv_a_j = MultiplyMatrixVector(B_inv, a_j);
                    double reducedCost = GetCValue(c, j, n) - DotProduct(GetCB(c, basis, n, m), B_inv_a_j);
                    T[0, j] = reducedCost;
                }
            }
            T[0, cols - 1] = DotProduct(GetCB(c, basis, n, m), x_B);

            // Constraint rows
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n + m; j++)
                {
                    if (basis.Contains(j))
                    {
                        T[i + 1, j] = (basis[i] == j) ? 1.0 : 0.0;
                    }
                    else
                    {
                        double[] a_j = GetColumn(A, j, m, n);
                        double[] B_inv_a_j = MultiplyMatrixVector(B_inv, a_j);
                        T[i + 1, j] = B_inv_a_j[i];
                    }
                }
                T[i + 1, cols - 1] = x_B[i];
            }

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (Math.Abs(T[i, j]) < eps) T[i, j] = 0.0;

            return T;
        }

        private static double[] GetColumn(double[,] A, int j, int m, int n)
        {
            double[] col = new double[m];
            for (int i = 0; i < m; i++)
            {
                col[i] = (j < n) ? A[i, j] : (j - n == i ? 1.0 : 0.0);
            }
            return col;
        }

        private static double GetCValue(double[] c, int j, int n)
        {
            return (j < n) ? c[j] : 0.0;
        }

        private static double[] GetCB(double[] c, int[] basis, int n, int m)
        {
            double[] cB = new double[m];
            for (int i = 0; i < m; i++)
            {
                cB[i] = GetCValue(c, basis[i], n);
            }
            return cB;
        }

        private static double[] MultiplyMatrixVector(double[,] mat, double[] vec)
        {
            int rows = mat.GetLength(0);
            int cols = mat.GetLength(1);
            double[] result = new double[rows];
            for (int i = 0; i < rows; i++)
            {
                double sum = 0;
                for (int k = 0; k < cols; k++)
                {
                    sum += mat[i, k] * vec[k];
                }
                result[i] = sum;
            }
            return result;
        }

        private static double DotProduct(double[] a, double[] b)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += a[i] * b[i];
            }
            return sum;
        }

        private static double[,] InvertMatrix(double[,] A)
        {
            int n = A.GetLength(0);
            var M = (double[,])A.Clone();
            var I = new double[n, n];
            for (int i = 0; i < n; i++) I[i, i] = 1.0;

            for (int k = 0; k < n; k++)
            {
                int piv = k;
                double max = Math.Abs(M[k, k]);
                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(M[i, k]) > max)
                    {
                        max = Math.Abs(M[i, k]);
                        piv = i;
                    }
                }
                if (Math.Abs(max) < 1e-12) throw new Exception("Singular matrix in inversion");

                if (piv != k)
                {
                    for (int j = 0; j < n; j++)
                    {
                        (M[k, j], M[piv, j]) = (M[piv, j], M[k, j]);
                        (I[k, j], I[piv, j]) = (I[piv, j], I[k, j]);
                    }
                }

                double diag = M[k, k];
                for (int j = 0; j < n; j++)
                {
                    M[k, j] /= diag;
                    I[k, j] /= diag;
                }

                for (int i = 0; i < n; i++)
                {
                    if (i == k) continue;
                    double factor = M[i, k];
                    for (int j = 0; j < n; j++)
                    {
                        M[i, j] -= factor * M[k, j];
                        I[i, j] -= factor * I[k, j];
                    }
                }
            }
            return I;
        }
    }
}