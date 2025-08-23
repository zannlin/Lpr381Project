using System;
using System.Collections.Generic;
using System.Linq;
using LPModelsLibrary.Models;

namespace LPModelsLibrary.Models
{
    public class PrimalSimplex
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

            int cols = n + m + 1;   
            int rows = m + 1;       
            double[,] T = new double[rows, cols];

            
            for (int j = 0; j < n; j++) T[0, j] = -c[j];

            
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
            result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration, null, null, "Initial tableau"));

            while (true)
            {
                iteration++;

                // STEP 1: Choose pivot column (most negative in Z row, i.e., row 0)
                int pivotColumn = -1;
                double mostNeg = -eps;
                for (int j = 0; j < cols - 1; j++)
                {
                    if (T[0, j] < mostNeg)
                    {
                        mostNeg = T[0, j];
                        pivotColumn = j;
                    }
                }

                // No negative coefficients means  optimal
                if (pivotColumn == -1)
                {
                    var x = new double[n + m];
                    

                    result.OptimalValue = T[0, cols - 1];
                    result.PrimalVariables = x.Take(n).ToArray();
                    result.Message = "Optimal solution found.";
                    result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration, null, null, "Optimal tableau"));
                    break;
                }

                // STEP 2: Minimum ratio test we ignore row 0 since its the objective function row
                int pivotRow = -1;
                double bestRatio = double.PositiveInfinity;
                for (int i = 1; i < rows; i++)
                {
                    double aij = T[i, pivotColumn];
                    if (aij > eps)
                    {
                        double ratio = T[i, cols - 1] / aij;
                        if (ratio < bestRatio - 1e-15 || Math.Abs(ratio - bestRatio) <= 1e-15 && i < pivotRow)
                        {
                            bestRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                {
                    result.IsUnbounded = true;
                    result.Message = "Unbounded: no leaving variable found.";
                    result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration, null, pivotColumn, "Unbounded"));
                    break;
                }

                // Pivot
                Pivot(T, pivotRow, pivotColumn, eps);

                // Update basis (pivotRow - 1 because row 0 is Z)
                

                // Save tableau so we can display later
                result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration, pivotRow, pivotColumn));
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
