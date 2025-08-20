using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LPModelsLibrary.Models;
namespace LPModelsLibrary.Models
{
    public class PrimalSimplex
    {
        
            public static SimplexResult Solve(double[,] A, double[] b, double[] c, double eps = 1e-9)
            {
                int m = A.GetLength(0); // Gets the number of constraints passed to the simplex result class created above
                int n = A.GetLength(1); // gets the decision variables passed to the simplex result class created above

                // This checks if the rhs matric contains any negative values because we're using simplex to solve a maximization problem, we're assuming the right-hand side (RHS) values are non-negative.
                if (b.Any(x => x < -eps))
                {
                    return new SimplexResult
                    {
                        IsInfeasible = true,
                        Message = "Infeasible: RHS contains negative values. Phase I required."
                    };
                }

                // This builds out the intial tableau using the number of slack variables and decision variables for columns and the number of constraints for rows and objective function for rows.
                int cols = n + m + 1;
                int rows = m + 1;
                double[,] T = new double[rows, cols];

                // Fill constraint rows
                for (int i = 0; i < m; i++)
                {
                    // Original coefficients
                    for (int j = 0; j < n; j++) T[i, j] = A[i, j]; // just copying the objective function coefficients and constraint coefficients to the tableau
                    // Slack variable
                    for (int j = 0; j < m; j++) T[i, n + j] = i == j ? 1.0 : 0.0; // adds the slack variables to the tableau, where each slack variable corresponds to a constraint
                    // RHS
                    T[i, cols - 1] = b[i];
                }

                // Fill objective row: -c for decision vars, 0 for slacks, 0 for RHS
                for (int j = 0; j < n; j++) T[rows - 1, j] = -c[j];

                // Initial array of basic variables (slack variables). At the beginning , all slack variables are basic.
                int[] basis = Enumerable.Range(n, m).ToArray();

                // Build headers for display
                string[] colHeaders = new string[cols];
                for (int j = 0; j < n; j++) colHeaders[j] = $"x{j + 1}";
                for (int j = 0; j < m; j++) colHeaders[n + j] = $"s{j + 1}";
                colHeaders[cols - 1] = "RHS";

                string[] rowHeaders = new string[rows];
                for (int i = 0; i < m; i++) rowHeaders[i] = (i+1).ToString();
                rowHeaders[rows - 1] = "z";

                var result = new SimplexResult();

                // Save initial tableau
                int iteration = 0;
                result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration, null, null, "Initial tableau"));


               
                while (true)
                {
                    iteration++;

                    // 
                    int pivotColumn = -1;
                   
                    
                        // deciding what the pivot column is going to be 
                        double mostNeg = -eps;
                        for (int j = 0; j < cols - 1; j++)
                        {
                            if (T[rows - 1, j] < mostNeg)
                            {
                                mostNeg = T[rows - 1, j];
                                pivotColumn = j;
                            }
                        }


                    // if there is no pc then the solution is optimal
                    if (pivotColumn == -1)
                    {
                        var x = new double[n + m];
                        for (int i = 0; i < m; i++) x[basis[i]] = T[i, cols - 1];

                        result.OptimalValue = T[rows - 1, cols - 1];
                        result.PrimalVariables = x.Take(n).ToArray();
                        result.Message = "Optimal solution found.";
                        result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration, null, null, "Optimal tableau"));
                        break;
                    }

                    // STEP 2: Minimum ratio test for leaving variable
                    int pivotRow = -1;
                    double bestRatio = double.PositiveInfinity;
                    for (int i = 0; i < m; i++)
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

                    // If no leaving variable: problem is unbounded
                    if (pivotRow == -1)
                    {
                        result.IsUnbounded = true;
                        result.Message = "Unbounded: no leaving variable found.";
                        result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration, null, pivotColumn, "Unbounded"));
                        break;
                    }

                    // Perform pivot
                    Pivot(T, pivotRow, pivotColumn, eps);

                    // Update basis and row headers
                    basis[pivotRow] = pivotColumn;
                    //rowHeaders[pivotRow] = colHeaders[pivotColumn];

                    // Save tableau
                    result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration, pivotRow, pivotColumn)); 
                }

                return result;
            }

            private static void Pivot(double[,] T, int pr, int pc, double eps) // pr = pivot row, pc = pivot column
            {
                int m = T.GetLength(0);
                int n = T.GetLength(1);

                double pivot = T[pr, pc];
                if (Math.Abs(pivot) < eps) throw new InvalidOperationException("Numerical pivot too small.");

                // Scale pivot row to make pivot element = 1
                for (int j = 0; j < n; j++) T[pr, j] /= pivot; 

                // Eliminate pivot column from all other rows
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


