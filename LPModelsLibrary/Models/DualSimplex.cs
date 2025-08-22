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

        public static SimplexResult Solve(double[,] ConstraintsValues, double[] RHS, double[] ZRow, char[] constraintTypes, double eps = 1e-9)
        {
            int numConstraints = ConstraintsValues.GetLength(0); // Number of constraints
            int numDecisionVar = ConstraintsValues.GetLength(1); // Number of decision variables

            // Validate constraint types
            if (constraintTypes.Length != numConstraints || constraintTypes.Any(ct => ct != 'L' && ct != 'G' && ct != 'E'))
            {
                return new SimplexResult
                {
                    IsInfeasible = true,
                    Message = "Infeasible: Constraint types must be 'L' (≤), 'G' (≥), or 'E' (=)."
                };
            }            

            // Check if objective row coefficients are non-negative (dual feasible)
            if (ZRow.Any(x => x > eps))
            {
                return new SimplexResult
                {
                    IsInfeasible = true,
                    Message = "Infeasible: Objective coefficients must be non-positive for dual simplex."
                };
            }

            // Build initial tableau
            int cols = numDecisionVar + numConstraints + 1; // Decision vars + slack/excess vars + RHS
            int rows = numConstraints + 1; // Constraints + objective row
            double[,] Table = new double[rows, cols];

            // Fill constraint rows
            for (int i = 0; i < numConstraints; i++)
            {
                // Case 1: ≥ constraints → flip all coefficients and RHS, then add +1 slack
                if (constraintTypes[i] == 'G')
                {
                    for (int j = 0; j < numDecisionVar; j++)
                        Table[i, j] = -ConstraintsValues[i, j];

                    for (int j = 0; j < numConstraints; j++)
                        Table[i, numDecisionVar + j] = (i == j) ? 1.0 : 0.0;

                    Table[i, cols - 1] = -RHS[i];
                }
                else // Case 2: ≤ or = constraints → keep coefficients, add +1 slack
                {
                    for (int j = 0; j < numDecisionVar; j++)
                        Table[i, j] = ConstraintsValues[i, j];

                    for (int j = 0; j < numConstraints; j++)
                        Table[i, numDecisionVar + j] = (i == j) ? 1.0 : 0.0;

                    Table[i, cols - 1] = RHS[i];
                }
            }

            // Fill objective row: -c for decision vars, 0 for slacks/excess, 0 for RHS
            for (int j = 0; j < numDecisionVar; j++) Table[rows - 1, j] = ZRow[j];

            // Initial basis (slack or excess variables)
            int[] basis = Enumerable.Range(numDecisionVar, numConstraints).ToArray();

            // Build headers
            string[] colHeaders = new string[cols];
            for (int j = 0; j < numDecisionVar; j++) colHeaders[j] = $"x{j + 1}";
            for (int j = 0; j < numConstraints; j++)
                colHeaders[numDecisionVar + j] = constraintTypes[j] == 'G' ? $"e{j + 1}" : $"s{j + 1}";
            colHeaders[cols - 1] = "RHS";

            string[] rowHeaders = new string[rows];
            for (int i = 0; i < numConstraints; i++) rowHeaders[i] = (i + 1).ToString();
            rowHeaders[rows - 1] = "z";

            var result = new SimplexResult();

            // Save initial tableau
            int iteration = 0;
            result.Tableaus.Add(new TableauTemplate(Table, colHeaders, rowHeaders, iteration, null, null, "Initial tableau"));

            while (true)
            {
                iteration++;

                // Step 1: Check for optimality (all RHS values non-negative)
                bool isOptimal = true;
                for (int i = 0; i < numConstraints; i++)
                {
                    if (Table[i, cols - 1] < -eps)
                    {
                        isOptimal = false;
                        break;
                    }
                }

                if (isOptimal)
                {
                    var x = new double[numDecisionVar + numConstraints];
                    for (int i = 0; i < numConstraints; i++) x[basis[i]] = Table[i, cols - 1];

                    result.OptimalValue = Table[rows - 1, cols - 1];
                    result.PrimalVariables = x.Take(numDecisionVar).ToArray();
                    result.SlackExcessVariables = x.Skip(numDecisionVar).ToArray();
                    result.Message = "Optimal solution found.";
                    result.Tableaus.Add(new TableauTemplate(Table, colHeaders, rowHeaders, iteration, null, null, "Optimal tableau"));
                    break;
                }


                // Step 2: Choose leaving variable (most negative RHS)
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
                
               // Console.WriteLine($"Leaving variable row: {pivotRow}, RHS = {mostNeg:F6}");

                // Step 3: Choose entering variable (dual ratio test)
                int pivotColumn = -1;
                double minRatio = double.PositiveInfinity;
                for (int j = 0; j < cols - 1; j++)
                {
                    double aij = Table[pivotRow, j];
                    
                    if (aij < -eps) // candidate entering column
                    {
                        double ratio = Math.Abs(Table[rows - 1, j] / aij); // take absolute value
                       // Console.WriteLine($"Column {j}: aij = {aij}, Zj = {Table[rows - 1, j]}, ratio = {ratio}");
                        if (ratio > eps && ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotColumn = j;
                        }
                    }
                }
               // Console.WriteLine($"Entering variable column: {pivotColumn}, ratio = {bestRatio:F6}");

                // If no entering variable: problem is infeasible
                if (pivotColumn == -1)
                {
                    result.IsInfeasible = true;
                    result.Message = "Infeasible: No entering variable found.";
                    result.Tableaus.Add(new TableauTemplate(Table, colHeaders, rowHeaders, iteration, pivotRow, null, "Infeasible"));
                    break;
                }

                // Step 4: Perform pivot
                Pivot(Table, pivotRow, pivotColumn, eps);

                // Update basis and row headers
                basis[pivotRow] = pivotColumn;
                rowHeaders[pivotRow] = colHeaders[pivotColumn];

                // Save tableau
                result.Tableaus.Add(new TableauTemplate(Table, colHeaders, rowHeaders, iteration, pivotRow, pivotColumn));
            }

            return result;
        }

        private static void Pivot(double[,] Table, int pr, int pc, double eps)
        {
            int m = Table.GetLength(0);
            int n = Table.GetLength(1);

            double pivot = Table[pr, pc];
            if (Math.Abs(pivot) < eps) throw new InvalidOperationException("Numerical pivot too small.");

           // Console.WriteLine($"\nPivoting on row {pr}, column {pc}, pivot value = {pivot:F6}");

            // Scale pivot row to make pivot element = 1
            for (int j = 0; j < n; j++) Table[pr, j] /= pivot;

            // Eliminate pivot column from all other rows
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

            // Clean small rounding errors
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                    if (Math.Abs(Table[i, j]) < eps) Table[i, j] = 0.0;
        }
    }
}
