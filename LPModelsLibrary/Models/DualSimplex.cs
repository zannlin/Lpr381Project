using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace LPModelsLibrary.Models
{
    public class DualSimplex
    {

        public static SimplexResult Solve(double[,] ConstraintsValues, double[] RHS, double[] ZRow, char[] constraintTypes, bool isMaximization = false, double eps = 1e-9)
        {
            int numConstraints = ConstraintsValues.GetLength(0);
            int numDecisionVar = ConstraintsValues.GetLength(1);

            // Validate inputs
            if (constraintTypes.Length != numConstraints || constraintTypes.Any(ct => ct != 'L' && ct != 'G' && ct != 'E'))
            {
                return new SimplexResult
                {
                    IsInfeasible = true,
                    Message = "Infeasible: Constraint types must be 'L' (≤), 'G' (≥), or 'E' (=)."
                };
            }

            // Adjust objective for maximization
            double[] adjustedZRow = isMaximization ? ZRow.Select(x => -x).ToArray() : (double[])ZRow.Clone();

            // Check dual feasibility
            if (adjustedZRow.Any(x => x > eps))
            {
                return new SimplexResult
                {
                    IsInfeasible = true,
                    Message = "Infeasible: Adjusted objective coefficients must be non-positive for dual simplex."
                };
            }

            // Build initial tableau
            int cols = numDecisionVar + numConstraints + 1;
            int rows = numConstraints + 1;
            double[,] Table = new double[rows, cols];

            // Fill constraint rows
            for (int i = 0; i < numConstraints; i++)
            {
                if (constraintTypes[i] == 'G')
                {
                    for (int j = 0; j < numDecisionVar; j++)
                        Table[i, j] = -ConstraintsValues[i, j];
                    for (int j = 0; j < numConstraints; j++)
                        Table[i, numDecisionVar + j] = (i == j) ? 1.0 : 0.0;
                    Table[i, cols - 1] = -RHS[i];
                }
                else if (constraintTypes[i] == 'L')
                {
                    for (int j = 0; j < numDecisionVar; j++)
                        Table[i, j] = ConstraintsValues[i, j];
                    for (int j = 0; j < numConstraints; j++)
                        Table[i, numDecisionVar + j] = (i == j) ? 1.0 : 0.0;
                    Table[i, cols - 1] = RHS[i];
                }
                else // 'E'
                {
                    for (int j = 0; j < numDecisionVar; j++)
                        Table[i, j] = ConstraintsValues[i, j];
                    for (int j = 0; j < numConstraints; j++)
                        Table[i, numDecisionVar + j] = 0.0;
                    Table[i, cols - 1] = RHS[i];
                }
            }

            // Fill objective row
            for (int j = 0; j < numDecisionVar; j++)
                Table[rows - 1, j] = adjustedZRow[j];

            // Initial basis
            int[] basis = new int[numConstraints];
            int basisIndex = numDecisionVar;
            for (int i = 0; i < numConstraints; i++)
            {
                if (constraintTypes[i] == 'E')
                    basis[i] = -1; // No basic variable for equality constraints
                else
                    basis[i] = basisIndex++;
            }

            // Build headers
            string[] colHeaders = new string[cols];
            for (int j = 0; j < numDecisionVar; j++)
                colHeaders[j] = $"x{j + 1}";
            for (int j = 0; j < numConstraints; j++)
                colHeaders[numDecisionVar + j] = constraintTypes[j] == 'G' ? $"e{j + 1}" : constraintTypes[j] == 'L' ? $"s{j + 1}" : $"eq{j + 1}";
            colHeaders[cols - 1] = "RHS";

            string[] rowHeaders = new string[rows];
            for (int i = 0; i < numConstraints; i++)
                rowHeaders[i] = (i + 1).ToString();
            rowHeaders[rows - 1] = "z";

            var result = new SimplexResult();
            result.Tableaus.Add(new TableauTemplate(Table, colHeaders, rowHeaders, 0, null, null, "Initial tableau"));

            int iteration = 0;

            // Dual Simplex Phase
            while (true)
            {
                iteration++;

                // Check if RHS is non-negative (primal feasible)
                bool isPrimalFeasible = true;
                for (int i = 0; i < numConstraints; i++)
                {
                    if (Table[i, cols - 1] < -eps)
                    {
                        isPrimalFeasible = false;
                        break;
                    }
                }

                if (isPrimalFeasible)
                    break; // Switch to primal simplex phase

                // Choose leaving variable (most negative RHS)
                int pivotRow = -1;
                double mostNeg = -eps;
                for (int i = 0; i < numConstraints; i++)
                {
                    if (Table[i, cols - 1] < mostNeg)
                    {
                        mostNeg = Table[i, cols - 1];
                        pivotRow = i;
                    }
                }

                // Choose entering variable (dual ratio test)
                int pivotColumn = -1;
                double minRatio = double.PositiveInfinity;
                for (int j = 0; j < cols - 1; j++)
                {
                    double aij = Table[pivotRow, j];
                    if (aij < -eps)
                    {
                        double ratio = Math.Abs(Table[rows - 1, j] / aij);
                        if (ratio > eps && ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotColumn = j;
                        }
                    }
                }

                if (pivotColumn == -1)
                {
                    result.IsInfeasible = true;
                    result.Message = "Infeasible: No entering variable found.";
                    result.Tableaus.Add(new TableauTemplate(Table, colHeaders, rowHeaders, iteration, pivotRow, null, "Infeasible"));
                    return result;
                }

                // Perform pivot
                Pivot(Table, pivotRow, pivotColumn, eps);

                // Update basis and row headers
                basis[pivotRow] = pivotColumn;
                rowHeaders[pivotRow] = colHeaders[pivotColumn];

                result.Tableaus.Add(new TableauTemplate(Table, colHeaders, rowHeaders, iteration, pivotRow, pivotColumn, "Dual pivot"));
            }

            // Primal Simplex Phase (if needed)
            while (true)
            {
                iteration++;

                // Check for optimality
                bool isOptimal = true;
                for (int j = 0; j < cols - 1; j++)
                {
                    double zj = Table[rows - 1, j];
                    if ((isMaximization && zj < -eps) || (!isMaximization && zj > eps))
                    {
                        isOptimal = false;
                        break;
                    }
                }

                if (isOptimal)
                {
                    var x = new double[numDecisionVar + numConstraints];
                    for (int i = 0; i < numConstraints; i++)
                        if (basis[i] >= 0)
                            x[basis[i]] = Table[i, cols - 1];

                    result.OptimalValue = Table[rows - 1, cols - 1];
                    if (isMaximization)
                        result.OptimalValue = result.OptimalValue;
                    result.PrimalVariables = x.Take(numDecisionVar).ToArray();
                    result.SlackExcessVariables = x.Skip(numDecisionVar).ToArray();
                    result.Message = "Optimal solution found.";
                    result.Tableaus.Add(new TableauTemplate(Table, colHeaders, rowHeaders, iteration, null, null, "Optimal tableau"));
                    break;
                }

                // Choose entering variable (primal simplex: most negative for max, most positive for min)
                int pivotColumn = -1;
                double mostExtreme = isMaximization ? -eps : eps;
                for (int j = 0; j < cols - 1; j++)
                {
                    double zj = Table[rows - 1, j];
                    if (isMaximization && zj < mostExtreme)
                    {
                        mostExtreme = zj;
                        pivotColumn = j;
                    }
                    else if (!isMaximization && zj > mostExtreme)
                    {
                        mostExtreme = zj;
                        pivotColumn = j;
                    }
                }

                // Minimum ratio test for leaving variable
                int pivotRow = -1;
                double bestRatio = double.PositiveInfinity;
                for (int i = 0; i < numConstraints; i++)
                {
                    double aij = Table[i, pivotColumn];
                    if (aij > eps)
                    {
                        double ratio = Table[i, cols - 1] / aij;
                        if (ratio < bestRatio - eps || (Math.Abs(ratio - bestRatio) <= eps && i < pivotRow))
                        {
                            bestRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                {
                    result.IsUnbounded = true;
                    result.Message = "Unbounded: No leaving variable found.";
                    result.Tableaus.Add(new TableauTemplate(Table, colHeaders, rowHeaders, iteration, null, pivotColumn, "Unbounded"));
                    break;
                }

                // Perform pivot
                Pivot(Table, pivotRow, pivotColumn, eps);

                // Update basis and row headers
                basis[pivotRow] = pivotColumn;
                rowHeaders[pivotRow] = colHeaders[pivotColumn];

                result.Tableaus.Add(new TableauTemplate(Table, colHeaders, rowHeaders, iteration, pivotRow, pivotColumn, "Primal pivot"));
            }

            return result;
        }

        private static void Pivot(double[,] Table, int pr, int pc, double eps)
        {
            int m = Table.GetLength(0);
            int n = Table.GetLength(1);

            double pivot = Table[pr, pc];
            if (Math.Abs(pivot) < eps) throw new InvalidOperationException("Numerical pivot too small.");

            // Scale pivot row
            for (int j = 0; j < n; j++) Table[pr, j] /= pivot;

            // Eliminate pivot column
            for (int i = 0; i < m; i++)
            {
                if (i == pr) continue;
                double factor = Table[i, pc];
                if (Math.Abs(factor) > eps)
                {
                    for (int j = 0; j < n; j++)
                        Table[i, j] -= factor * Table[pr, j];
                }
            }

            // Clean rounding errors
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                    if (Math.Abs(Table[i, j]) < eps) Table[i, j] = 0.0;
        }
    }
}
