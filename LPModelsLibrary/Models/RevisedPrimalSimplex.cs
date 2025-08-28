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

            // Build initial tableau for display
            double[,] initialT = BuildInitialTableau(A, b, c, m, n, eps);
            result.Tableaus.Add(new TableauTemplate(initialT, colHeaders, rowHeaders, iteration, null, null, "Initial tableau"));

            while (true)
            {
                // Compute reduced costs
                double[] reducedCosts = new double[n + m];
                int pivotCol = -1;
                double minReducedCost = 0;

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

                    reducedCosts[j] = ((j < n) ? c[j] : 0.0) - c_B_dot_B_inv_a_j;

                    if (reducedCosts[j] < minReducedCost - eps)
                    {
                        minReducedCost = reducedCosts[j];
                        pivotCol = j;
                    }
                }

                if (pivotCol == -1) // Optimal
                {
                    // Compute optimal value
                    result.OptimalValue = DotProduct(GetCB(c, basis, n, m), x_B);

                    // Primal variables
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

                    // Final tableau
                    double[,] finalT = BuildTableau(A, b, c, B_inv, basis, x_B, m, n, eps);
                    result.Tableaus.Add(new TableauTemplate(finalT, colHeaders, rowHeaders, iteration, null, null, "Optimal tableau"));

                    result.Message = "Optimal solution found.";
                    break;
                }

                // Compute a_hat = B_inv * a_pivot
                double[] a_pivot = GetColumn(A, pivotCol, m, n);
                double[] a_hat = MultiplyMatrixVector(B_inv, a_pivot);

                // Find pivot row
                int pivotRow = -1;
                double minRatio = double.PositiveInfinity;

                for (int i = 0; i < m; i++)
                {
                    if (a_hat[i] > eps)
                    {
                        double ratio = x_B[i] / a_hat[i];
                        if (ratio < minRatio - eps)
                        {
                            minRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                {
                    result.IsUnbounded = true;
                    result.Message = "Unbounded: no positive a_hat entries.";
                    result.Tableaus.Add(new TableauTemplate(initialT, colHeaders, rowHeaders, iteration, null, pivotCol, "Unbounded"));
                    break;
                }

                // Update basis
                basis[pivotRow] = pivotCol;

                // Update x_B
                x_B[pivotRow] = minRatio;
                for (int i = 0; i < m; i++)
                {
                    if (i != pivotRow)
                    {
                        x_B[i] -= minRatio * a_hat[i];
                    }
                }

                // Update B_inv
                B_inv = ComputeBInv(A, basis, m, n, eps);

                // Update row headers
                rowHeaders[pivotRow] = colHeaders[pivotCol];

                // Add tableau for this iteration
                double[,] currentT = BuildTableau(A, b, c, B_inv, basis, x_B, m, n, eps);
                result.Tableaus.Add(new TableauTemplate(currentT, colHeaders, rowHeaders, iteration, pivotRow, pivotCol));

                iteration++;
            }

            return result;
        }

        private static double[,] BuildInitialTableau(double[,] A, double[] b, double[] c, int m, int n, double eps)
        {
            int cols = n + m + 1;
            int rows = m + 1;
            double[,] T = new double[rows, cols];

            // Objective row
            for (int j = 0; j < n; j++) T[0, j] = -c[j];

            // Constraint rows
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) T[i + 1, j] = A[i, j];
                for (int j = 0; j < m; j++) T[i + 1, n + j] = (i == j) ? 1.0 : 0.0;
                T[i + 1, cols - 1] = b[i];
            }

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

            // Objective RHS (optimal value)
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

            // Clean small values
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
            double[] result = new double[rows];
            for (int i = 0; i < rows; i++)
            {
                double sum = 0;
                for (int k = 0; k < rows; k++)
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

        private static double[,] ComputeBInv(double[,] A, int[] basis, int m, int n, double eps)
        {
            // Build B matrix from basis columns
            double[,] B = new double[m, m];
            for (int i = 0; i < m; i++)
            {
                double[] col = GetColumn(A, basis[i], m, n);
                for (int k = 0; k < m; k++)
                {
                    B[k, i] = col[k];
                }
            }

            // Compute inverse using Gaussian elimination
            double[,] aug = new double[m, 2 * m]; // Augmented matrix [B | I]
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    aug[i, j] = B[i, j];
                    aug[i, m + j] = (i == j) ? 1.0 : 0.0;
                }
            }

            // Gaussian elimination with partial pivoting
            for (int p = 0; p < m; p++)
            {
                // Find pivot row
                int maxRow = p;
                for (int i = p + 1; i < m; i++)
                {
                    if (Math.Abs(aug[i, p]) > Math.Abs(aug[maxRow, p]))
                    {
                        maxRow = i;
                    }
                }

                // Swap rows
                if (maxRow != p)
                {
                    double[] temp = new double[2 * m];
                    for (int j = 0; j < 2 * m; j++)
                    {
                        temp[j] = aug[p, j];
                        aug[p, j] = aug[maxRow, j];
                        aug[maxRow, j] = temp[j];
                    }
                }

                // Check for singularity
                if (Math.Abs(aug[p, p]) < eps)
                {
                    throw new InvalidOperationException("Matrix B is singular; cannot invert.");
                }

                // Eliminate below pivot
                for (int i = p + 1; i < m; i++)
                {
                    double factor = aug[i, p] / aug[p, p];
                    for (int j = p; j < 2 * m; j++)
                    {
                        aug[i, j] -= factor * aug[p, j];
                    }
                }
            }

            // Back substitution
            for (int p = m - 1; p >= 0; p--)
            {
                // Normalize pivot row
                double pivot = aug[p, p];
                for (int j = p; j < 2 * m; j++)
                {
                    aug[p, j] /= pivot;
                }

                // Eliminate above pivot
                for (int i = 0; i < p; i++)
                {
                    double factor = aug[i, p];
                    for (int j = p; j < 2 * m; j++)
                    {
                        aug[i, j] -= factor * aug[p, j];
                    }
                }
            }

            // Extract inverse
            double[,] B_inv = new double[m, m];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    B_inv[i, j] = aug[i, m + j];
                }
            }

            return B_inv;
        }
    }
}