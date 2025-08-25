using System;
using System.Collections.Generic;
using System.Linq;
using LPModelsLibrary.Models;

namespace LPModelsLibrary.Models
{
    public class PrimalSimplex
    {
        public static SimplexResult Solve(double[,] A, double[] b, double[] c, bool isMaximization = true, double eps = 1e-9)
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

            int cols = n + m + 1;
            int rows = m + 1;
            double[,] T = new double[rows, cols];

            // For MAXIMIZATION: T[0, j] = -c[j] (we want negatives for entering variable test)
            // For MINIMIZATION: T[0, j] = c[j] (we want positives, then check for positives to enter)
            for (int j = 0; j < n; j++)
            {
                T[0, j] = isMaximization ? -c[j] : c[j];
            }

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) T[i + 1, j] = A[i, j];
                for (int j = 0; j < m; j++) T[i + 1, n + j] = i == j ? 1.0 : 0.0;
                T[i + 1, cols - 1] = b[i];
            }

            // Column headers
            string[] colHeaders = new string[cols];
            for (int j = 0; j < n; j++) colHeaders[j] = $"x{j + 1}";
            for (int j = 0; j < m; j++) colHeaders[n + j] = $"s{j + 1}";
            colHeaders[cols - 1] = "RHS";

            // Row headers: first Z, then constraints
            string[] rowHeaders = new string[rows];
            rowHeaders[0] = "z";
            for (int i = 0; i < m; i++) rowHeaders[i + 1] = (i + 1).ToString();

            var result = new SimplexResult();
            int iteration = 0;
            string problemType = isMaximization ? "MAX" : "MIN";
            result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration, null, null, $"Initial tableau ({problemType})"));

            while (true)
            {
                iteration++;

                
                int pivotColumn = -1;
                double bestValue = isMaximization ? -eps : eps; // For MAX: look for most negative, for MIN: look for most positive

                for (int j = 0; j < cols - 1; j++)
                {
                    if (isMaximization)
                    {
                        // Maximization: enter variable with most negative coefficient
                        if (T[0, j] < bestValue)
                        {
                            bestValue = T[0, j];
                            pivotColumn = j;
                        }
                    }
                    else
                    {
                        // Minimization: enter variable with most positive coefficient
                        if (T[0, j] > bestValue)
                        {
                            bestValue = T[0, j];
                            pivotColumn = j;
                        }
                    }
                }

                // Check optimality condition
                bool isOptimal = (pivotColumn == -1);
                if (isOptimal)
                {
                    var x = new double[n + m];

                    
                    for (int i = 1; i < rows; i++)
                    {
                        // Find which variable is basic in this row (has coefficient 1 and others 0)
                        for (int j = 0; j < cols - 1; j++)
                        {
                            bool isBasicInThisRow = Math.Abs(T[i, j] - 1.0) < eps;
                            if (isBasicInThisRow)
                            {
                                // Check if this column has zeros elsewhere
                                bool isBasicVariable = true;
                                for (int k = 0; k < rows; k++)
                                {
                                    if (k != i && Math.Abs(T[k, j]) > eps)
                                    {
                                        isBasicVariable = false;
                                        break;
                                    }
                                }
                                if (isBasicVariable && j < n + m)
                                {
                                    x[j] = T[i, cols - 1];
                                }
                            }
                        }
                    }

                    
                    double objectiveValue = isMaximization ? T[0, cols - 1] : -T[0, cols - 1];

                    result.OptimalValue = objectiveValue;
                    result.PrimalVariables = x.Take(n).ToArray();
                    result.Message = $"Optimal solution found ({problemType}).";
                    result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration, null, null, $"Optimal tableau ({problemType})"));
                    break;
                }

                
                int pivotRow = -1;
                double bestRatio = double.PositiveInfinity;
                for (int i = 1; i < rows; i++)
                {
                    double aij = T[i, pivotColumn];
                    if (aij > eps)
                    {
                        double ratio = T[i, cols - 1] / aij;
                        if (ratio < bestRatio - 1e-15 || (Math.Abs(ratio - bestRatio) <= 1e-15 && i < pivotRow))
                        {
                            bestRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                {
                    result.IsUnbounded = true;
                    result.Message = $"Unbounded: no leaving variable found ({problemType}).";
                    result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration, null, pivotColumn, $"Unbounded ({problemType})"));
                    break;
                }

                
                Pivot(T, pivotRow, pivotColumn, eps);

                
                result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration, pivotRow, pivotColumn, $"Iteration {iteration} ({problemType})"));
            }

            return result;
        }

        private static void Pivot(double[,] T, int pr, int pc, double eps)
        {
            int m = T.GetLength(0);
            int n = T.GetLength(1);

            double pivot = T[pr, pc];
            if (Math.Abs(pivot) < eps) throw new InvalidOperationException("Numerical pivot too small.");

            // Dive the pivor row by the pivot number
            for (int j = 0; j < n; j++) T[pr, j] /= pivot;

            // Eliminate pivot column in other rows
            for (int i = 0; i < m; i++)
            {
                if (i == pr) continue;
                double factor = T[i, pc];
                if (Math.Abs(factor) > 0)
                {
                    for (int j = 0; j < n; j++)
                        T[i, j] -= factor * T[pr, j];
                }
            }

            // Clean small rounding errors
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                    if (Math.Abs(T[i, j]) < eps) T[i, j] = 0.0;
        }
    }
}
