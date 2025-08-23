namespace LPModelsLibrary.Models
{
    public class CuttingPlaneSimplex
    {
        public SimplexResult cuttingPlaneResult;
        public TableauTemplate optimalTableau;

        public CuttingPlaneSimplex(SimplexResult result)
        {
            cuttingPlaneResult = result ?? throw new InvalidOperationException("No Simplex Result found in the result.");
            optimalTableau = cuttingPlaneResult.Tableaus.LastOrDefault()
                              ?? throw new InvalidOperationException("No tableau available in the result.");
        }

        public void solveCuttingPlane()
        {

            var tab = optimalTableau;
            var currentTableau = tab.Tableau;
            // Implement the cutting plane method here
            // 1. Check if the current solution is integer
            bool isInteger = IsIntegerSolution(currentTableau);
            int gaurd = 0, gaurdMax = 100;

            try
            {
                while (!isInteger && gaurd < gaurdMax)
                {
                    //2. add a cutting plane constraint
                    double[] newConstraint = CreateConstraint(currentTableau);
                    double[,] newTableau = addConstraint(currentTableau, newConstraint);
                    tab.Tableau = newTableau;

                    // 3. Re-solve using dual simplex
                    tab = DualSimplex(tab);
                    

                    // 4. Repeat until an integer solution is found or infeasibility is determined
                    tab.ColHeaders = updateColumnHeaders(tab.ColHeaders);
                    tab.RowHeaders = updateRowHeaders(tab.RowHeaders);
                    cuttingPlaneResult.Tableaus.Add(tab);
                    currentTableau = tab.Tableau;
                    isInteger = IsIntegerSolution(currentTableau);
                    gaurd++;
                }
                cuttingPlaneResult.OptimalValue = tab.Tableau[0, tab.Tableau.GetLength(1) - 1];
                cuttingPlaneResult.Message = isInteger ? "Integer solution found." : "Max iterations reached without finding an integer solution.";



            }
            catch (InvalidOperationException ex)
            {
                cuttingPlaneResult.IsInfeasible = true;
                cuttingPlaneResult.Message = ex.Message;

            }
        }

        private TableauTemplate DualSimplex(TableauTemplate tab, double eps = 1e-9)
        {

            int m = tab.Tableau.GetLength(0);
            int n = tab.Tableau.GetLength(1);
            int zRow = 0;
            int rhs = n - 1;

            int guard = 0, guardMax = 1000;

            while (true)
            {
                guard++;
                if (guard > guardMax) break;

                // 1) pick leaving row = most negative RHS among constraints
                int pr = -1;
                double mostNeg = -eps;
                for (int i = 1; i <m; i++)
                {
                    double b = tab.Tableau[i, rhs];
                    if (b < mostNeg)
                    {
                        mostNeg = b;
                        pr = i;
                    }
                }

                // If none negative -> primal feasible -> done (since z-row is unchanged)
                if (pr == -1) break;

                // 2) entering column via dual ratio test: argmin_j { z_j / (-a_prj) | a_prj < 0 }
                int pc = -1;
                double bestRatio = double.PositiveInfinity;
                for (int j = 0; j < rhs; j++) // exclude RHS
                {
                    double a = tab.Tableau[pr, j];
                    if (a < -eps)
                    {
                        double zj = tab.Tableau[zRow, j];
                        double ratio = zj / (-a);
                        if (ratio < bestRatio - 1e-15)
                        {
                            bestRatio = ratio;
                            pc = j;
                        }
                    }
                }

                if (pc == -1)
                {
                    
                    throw new InvalidOperationException("LP is primal infeasible (dual unbounded).");
                }

                // 3) pivot and update row header for the basic variable
                tab.Tableau = Pivot(tab.Tableau, pr, pc, eps);

                
            }
            return tab;

        }

        private static double [,] Pivot(double[,] T, int pr, int pc, double eps) // pr = pivot row, pc = pivot column
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

            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                    if (Math.Abs(T[i, j]) < eps) T[i, j] = 0.0;
          return T;
        }

        private double [] CreateConstraint(double[,] currentTablue)
        {
            // Create a new constraint from the current tableau
           int rows =  currentTablue.GetLength(0);
           int cols =  currentTablue.GetLength(1);

            double closestFractionalValue = double.MaxValue;
            int rowIndex = -1;

            for( int i  =0; i < rows; i++) // Exclude the last row (objective function)
            {
                double rhsValue = currentTablue[i, cols - 1]; 
                

                rhsValue = rhsValue - Math.Floor(rhsValue);
                if(Math.Abs(rhsValue - 0.5) < Math.Abs(closestFractionalValue - 0.5))
                {
                    closestFractionalValue = rhsValue;
                    rowIndex = i;
                }
            }

            if(rowIndex != -1)
            {
                //getting the old row.
                double[] oldRow = new double[cols];

                for (int j = 0; j < cols; j++) // Exclude the last column (RHS)
                {
                    
                    oldRow[j] = currentTablue[rowIndex, j];
                }

                for (int j = 0; j < cols; j++) // Exclude the last column (RHS)
                { // -1.3333 -> 1.3333 --1 -> -0.3333 ->
                    double newValue = oldRow[j] - Math.Truncate(oldRow[j]);
                    oldRow[j] = oldRow[j] - Math.Truncate(oldRow[j]);

                    if (oldRow[j] >= 0) 
                    { 
                        oldRow[j] = newValue; 
                    } 
                    else 
                    { 
                        newValue = Math.Abs(newValue); 
                        oldRow[j] = 1 - newValue;
                    }

                    // negation of everything to convert
                    oldRow[j] *= -1;
                }

                double [] newRow = new double[cols + 1];
                for(int j = 0; j < cols -1; j++)
                {
                    newRow[j] = oldRow[j];
                }
                newRow[newRow.Length -1] = oldRow[cols - 1];
                newRow[newRow.Length-2] = 1;

                return newRow;

            }
            else
            {
                throw new InvalidOperationException("No fractional solution found.");
            }


        }

        private double[,] addConstraint(double[,] currentTableau,double[] constraint)
        {
            double[,] newTableau = new double[currentTableau.GetLength(0) + 1, currentTableau.GetLength(1) + 1];
            int rows = currentTableau.GetLength(0);
            int cols = currentTableau.GetLength(1);
            // Copy old tableau to new tableau
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols-1; j++)
                {
                    newTableau[i, j] = currentTableau[i, j];
                }
            }

            // Copy old RHS to new tableau
            for (int i = 0; i < rows; i++)
            {
                newTableau[i, cols] = currentTableau[i, cols - 1];
            }
            // Add new constraint row
            for (int j = 0; j < constraint.Length; j++)
            {
                newTableau[rows, j] = constraint[j];
            }

            // add z row 

           
            return newTableau;
            
        }

        private bool IsIntegerSolution(double[,] tableau)
        {
            int rhsCol = tableau.GetLength(1) - 1;
            for (int i = 1; i < tableau.GetLength(0); i++) // Skip the objective function row
            {
                double value = tableau[i, rhsCol];

                if(Math.Round(value,6) != Math.Truncate(value))
                {
                    return false;
                }

            }
            return true;
        }

        private string[] updateColumnHeaders(string[] colHeaders)
        {
            string[] newColHeaders = new string[colHeaders.Length + 1];
            for(int i = 0; i < colHeaders.Length -1; i++)
            {
                newColHeaders[i] = colHeaders[i];
            }
            int slackIndex = newColHeaders.Length - 3;

            int slackNum = newColHeaders[slackIndex].Substring(1) != "" ? int.Parse(newColHeaders[slackIndex].Substring(1)) : 0;
            newColHeaders[newColHeaders.Length - 2] = $"s{slackNum+1}";
            newColHeaders[newColHeaders.Length - 1] = colHeaders[colHeaders.Length - 1];

            return newColHeaders;
        }

        private string[] updateRowHeaders(string[] rowHeaders)
        {
            string[] newRowHeaders = new string[rowHeaders.Length + 1];
            for (int i = 0; i < rowHeaders.Length; i++)
            {
                newRowHeaders[i] = rowHeaders[i];
            }
            newRowHeaders[newRowHeaders.Length - 1] = $"{rowHeaders.Length}";
            return newRowHeaders;
        }
    }
}
