using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System;
using NCalc;
using Expression = NCalc.Expression;
namespace LPModelsLibrary.Models
{
    public class MathematicalSensitivity
    {
        private TableauTemplate optimalTab;
        private TableauTemplate originalTab;
        public MathematicalSensitivity(TableauTemplate optimalTable,TableauTemplate originalTab)
        {
            this.optimalTab = optimalTable;
            this.originalTab = originalTab;
        }
        private void SensitivityAnalysis()
        {
            // just output the general sensitivity analysis information
        }

        private double ComputeLHS(double[] cBV, double[,] Binv, double[] aj)
        {
            // Multiply Binv * aj
            int n = aj.Length;
            double[] temp = new double[n];
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                    sum += Binv[i, j] * aj[j];
                temp[i] = sum;
            }

            // Dot product with cBV
            double lhs = 0;
            for (int i = 0; i < n; i++)
                lhs += cBV[i] * temp[i];

            return lhs;
        }

        private string deltaRangeNonBasicVariable(double lhs, double cj)
        {
            string rangeInfo = "";
            if (lhs > 0)
            {
                double upperBound = lhs;
                rangeInfo = $"Δ ≤ {upperBound}";
            }
            else if (lhs < 0)
            {
                double lowerBound = lhs;
                rangeInfo = $"Δ ≥ {lowerBound}";
            }
            else
            {
                rangeInfo = "Δ can take any value (unbounded)";
            }
            return rangeInfo;
        }

        public List<int> getBasicVarColumnIndex()
        {// This method goescolumn by column to check if it is a basic variable
            int rows = optimalTab.Tableau.GetLength(0);
            int cols = optimalTab.Tableau.GetLength(1);
            List<int> columns = new List<int>();
            List<(int row, int col)> basicVarPositions = new List<(int row, int col)>();

            for (int j = 0; j < cols - 1; j++) // Exclude RHS column
            {
                int countOnes = 0;
                int oneRowIndex = -1;
                bool isBasic = true;
                for (int i = 1; i < rows; i++) // Exclude objective function row
                {
                    double cellValue = optimalTab.Tableau[i, j];
                 if(cellValue ==1)
                    {
                        oneRowIndex = i;
                        countOnes++;
                    }
                    else if(cellValue != 0)
                    {
                        isBasic = false;
                        break; // No need to check further, not a basic variable
                    }
                }
                if(isBasic && countOnes == 1)
                {
                    basicVarPositions.Add((oneRowIndex, j)); // Store the coefficient from the objective function row
                }
                
            }
            columns = basicVarPositions
        .OrderBy(bv => bv.row)
        .Select(bv => bv.col)
        .ToList();

            //returns the cbv of an optimal solution.
            return columns;
        }

        private double[] getCbv()
        {
            List<int> basicColumnIndicies = getBasicVarColumnIndex();
            double[] cbv = new double[basicColumnIndicies.Count];
            for (int i = 0; i < basicColumnIndicies.Count; i++)
            {
                
                int colIndex = basicColumnIndicies[i];
                double value = originalTab.Tableau[0, colIndex];
                cbv[i] = Math.Abs(value);
            }
            return cbv;
        }

        private double[,] constructBMatrix()
        {
             List<int> basicColumnIndicies = getBasicVarColumnIndex();
            int numRows = originalTab.Tableau.GetLength(0) - 1; // Exclude objective function row
            double[,] B = new double[numRows, basicColumnIndicies.Count];
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < basicColumnIndicies.Count; j++)
                {
                    B[i, j] = originalTab.Tableau[i+1, basicColumnIndicies[j]]; // +1 to skip objective function row
                }
            }
            return B;
        }

        private double[,] InverseMatrix(double[,] matrixToInverse)
        {

            Matrix<double> matrix = Matrix<double>.Build.DenseOfArray(matrixToInverse);
            
            return matrix.Inverse().ToArray();
            
        }

        private double[] computeSmallB()
        {
            double[] rhsColumn = new double[originalTab.Tableau.GetLength(0)-1];
            for(int i = 1; i < originalTab.Tableau.GetLength(0); i++)
            {
                rhsColumn[i-1] = originalTab.Tableau[i, originalTab.Tableau.GetLength(1) - 1];
            }
            return rhsColumn;
        }
            
        public string Range_Of_NonBasic_Variable(int columnIndex)
        {

            Console.WriteLine("Range of Non Basic Variable");

            Console.WriteLine("Optimal Tableau");
            Console.WriteLine(optimalTab.ToString());
            // Cj∗​=cBV​B−1aj​−(cj​+Δ) Formulate forcalculate the range of optimality for a  variable
            double cj = originalTab.Tableau[0, columnIndex] * -1;
            double[] cBV = getCbv();
            double[,] BInverse = InverseMatrix(constructBMatrix());
            double[] aj = new double[originalTab.Tableau.GetLength(0) - 1];
            for (int i = 1; i < originalTab.Tableau.GetLength(0); i++)
            {
                aj[i - 1] = originalTab.Tableau[i, columnIndex];
            }

            double lhs = ComputeLHS(cBV, BInverse, aj) - cj;
            string rangeInfo = deltaRangeNonBasicVariable(lhs, cj);

            Console.WriteLine("CJ :");
            Console.WriteLine($"c[{columnIndex}] = {cj}");

            Console.WriteLine("cBV:");
            for (int i = 0; i < cBV.Length; i++)
            {
                Console.WriteLine($"cB[{i}] = {cBV[i]}");
            }

            Console.WriteLine("B Matrix:");
            double[,] B = constructBMatrix();
            for (int i = 0; i < B.GetLength(0); i++)
            {
                for (int j = 0; j < B.GetLength(1); j++)
                {
                    Console.Write($"{B[i, j]} ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("B Inverse Matrix:");
            for (int i = 0; i < BInverse.GetLength(0); i++)
            {
                for (int j = 0; j < BInverse.GetLength(1); j++)
                {
                    Console.Write($"{BInverse[i, j]} ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("aj:");
            for (int i = 0; i < aj.Length; i++)
            {
                Console.WriteLine($"a[{i}] = {aj[i]}");
            }

            Console.WriteLine($"LHS (cBV * B^-1 * aj) = {lhs}");
            Console.WriteLine($"Range Information: {rangeInfo}");

            return rangeInfo;
        }

        public string change_nonBasic_Variable_Coefficient(int columnIndex, double newCoefficient)
        {
            double cj = newCoefficient;
            double[] cBV = getCbv();
            double[,] BInverse = InverseMatrix(constructBMatrix());
            double[] aj = new double[originalTab.Tableau.GetLength(0) - 1];
            for (int i = 1; i < originalTab.Tableau.GetLength(0); i++)
            {
                aj[i - 1] = originalTab.Tableau[i, columnIndex];
            }

            double lhs = ComputeLHS(cBV, BInverse, aj) - cj;
            string orignalRange  = "The original optimal value is for this variable was: " + optimalTab.Tableau[0, columnIndex];
            string middle = $"Changing the variable from {Math.Abs(originalTab.Tableau[0, columnIndex])} to {newCoefficient} ";
            string rangeInfo = $"The new optimal value for {optimalTab.ColHeaders[columnIndex]} is {lhs}";

            // We  still need to deal with the dboule round ie 1.9999999999991 -> 2
            return orignalRange + "\n" + middle + "\n" + rangeInfo;
        }

        public (string,string) range_Of_Basic_Variable(int columnIndex)
        {
            
            double[] cBV = getCbv();
            double[,] BInverse = InverseMatrix(constructBMatrix());
            List<int> basicColumnIndices = getBasicVarColumnIndex();
            double Coefficient = Math.Abs(originalTab.Tableau[0, columnIndex]);
            double increment = 0.1;
            string upperRange = "Has an infinite higher range";
            string lowerRange = "Has an infinite lower range";
            bool flag = false;
            double gaurdMax = 1000/increment;
            double gaurd = 0;

            while (!flag && gaurd <= gaurdMax)
            {
                Coefficient -= increment;
                // we need to change the cbv here
                for (int i = 0; i < cBV.Length; i++)
                {
                    if (basicColumnIndices[i] == columnIndex)
                    {
                        cBV[i] = Math.Abs(Coefficient);
                        Console.WriteLine($"Updated CBV[{i}] to {cBV[i]} (was {getCbv()[i]})");
                        break;
                    }
                }

                double[] newZrow = computeNewZRow(cBV, BInverse);
                
                for (int j = 0; j < newZrow.Length - 1; j++) // Exclude RHS column
                {
                    // since we arent updating the value of the basic varibable cj when computing the  new z-row values we neeed to skip it when checking ( math wise it will always stay 0 but since we arent also updating it cj to make the new incremenet it causes bugs. )
                    if (newZrow[j] < 0 && j != columnIndex)
                    {

                        double range = Math.Abs(originalTab.Tableau[0, columnIndex]) - (Coefficient);
                       lowerRange = ($"Allowabe  decrease {range}");
                        flag = true;
                        
                    }
                    Console.Write($"Z Row Value at index {j}: {newZrow[j]}\n");
                }
                Console.WriteLine($"Coefficient after increment: {Coefficient}");
                gaurd++;   
            }
            
            while (!flag && gaurd <= gaurdMax)
            {
                Coefficient += increment;
                // we need to change the cbv here
                for (int i = 0; i < cBV.Length; i++)
                {
                    if (basicColumnIndices[i] == columnIndex)
                    {
                        cBV[i] = Math.Abs(Coefficient);
                        Console.WriteLine($"Updated CBV[{i}] to {cBV[i]} (was {getCbv()[i]})");
                        break;
                    }
                }

                double[] newZrow = computeNewZRow(cBV, BInverse);

                for (int j = 0; j < newZrow.Length - 1; j++) // Exclude RHS column
                {
                    // since we arent updating the value of the basic varibable cj when computing the  new z-row values we neeed to skip it when checking ( math wise it will always stay 0 but since we arent also updating it cj to make the new incremenet it causes bugs. )
                    if (newZrow[j] < 0 && j != columnIndex)
                    {

                        double range = Math.Abs(originalTab.Tableau[0, columnIndex]) - (Coefficient);
                        upperRange = ($"Allowabe increase of  {range}");
                        flag = true;

                    }
                    Console.Write($"Z Row Value at index {j}: {newZrow[j]}\n");
                }
                Console.WriteLine($"Coefficient after increment: {Coefficient}");
                gaurd++;
            }

            return (upperRange,lowerRange);

        }

        private double [] computeNewZRow(double[] cBV, double[,] BInverse)
        {
            double[] newZrow = new double[optimalTab.Tableau.GetLength(1)];
            int cols = originalTab.Tableau.GetLength(1);    

            for(int j =0;j<cols-1;j++) // Exclude RHS column
            {
                double  cj = Math.Abs(originalTab.Tableau[0, j]);
                double[] aj = new double[originalTab.Tableau.GetLength(0) - 1];
                for (int i = 1; i < originalTab.Tableau.GetLength(0); i++)
                {
                    aj[i - 1] = originalTab.Tableau[i, j];
                }
                double lhs = ComputeLHS(cBV, BInverse, aj);
                newZrow[j] = lhs-cj;
            }

            return newZrow;
        }

        public double[] change_Basic_Variable_Coefficient(int columnIndex, double newCoefficient)
        {
            // The reason this isnt working is because when changing the cbv we need to see the changes for all thevalues in the z row not  just the one we are changing
            // Those values will change as well, so we need to recalculate the entire z row.

            double cj = newCoefficient;
            double[] cBV = getCbv();
            double oldValue = originalTab.Tableau[0, columnIndex];
            List<int> basicColumnIndices = getBasicVarColumnIndex();
            // we need to change the cbv here

            // Creates the new CBV we'll be using to calculate the new optimal value for each Z - row value.
            for (int i = 0; i < cBV.Length; i++)
            {
                if (basicColumnIndices[i]  == columnIndex)
                {
                    cBV[i] = Math.Abs(newCoefficient);
                    break;
                }
            }

            double[,] BInverse = InverseMatrix(constructBMatrix());
            double[] aj = new double[originalTab.Tableau.GetLength(0) - 1];
            for (int i = 1; i < originalTab.Tableau.GetLength(0); i++)
            {
                aj[i-1] = originalTab.Tableau[i, columnIndex];
            }


            double[] newZrow = computeNewZRow(cBV, BInverse);
            newZrow[columnIndex] = 0;
            
            return newZrow;

        }

        public (string, string) range_RightHandSide(int rowIndex)
        {
            double[,] BInverse = InverseMatrix(constructBMatrix());
            double[] smallB = computeSmallB();
            double increment = 0.1;
            string upperRange = "Has an infinite higher range";
            string lowerRange = "Has an infinite lower range";
            bool flag = false;
            double gaurdMax = 1000 / increment;
            double gaurd = 0;
            double currentRhs = originalTab.Tableau[rowIndex, originalTab.Tableau.GetLength(1) - 1];

            while (!flag && gaurd <= gaurdMax)
            {
                currentRhs -= increment;

                for (int i = 0; i < smallB.Length; i++)
                {
                    if (i == rowIndex - 1)
                    {
                        smallB[i] = currentRhs;
                        break;
                    }
                }


                double[] bOptimal = new double[BInverse.GetLength(1)];
                for (int i = 0; i < BInverse.GetLength(0); i++)
                {
                    double sum = 0;
                    for (int j = 0; j < BInverse.GetLength(1); j++)
                    {
                        sum += BInverse[i, j] * smallB[j];
                    }
                    bOptimal[i] = sum;
                }

                for (int j = 0; j < bOptimal.Length; j++)
                {
                    if(bOptimal[j] < 0)
                    {
                        double range = originalTab.Tableau[rowIndex, originalTab.Tableau.GetLength(1) - 1] - (currentRhs);
                        lowerRange = ($"Allowabe  decrease {range}");
                        flag = true;
                    }
                    
                }

                gaurd++;
            }
            gaurdMax = 1000 / increment;
            while (!flag && gaurd <= gaurdMax)
            {
                currentRhs += increment;

                for (int i = 0; i < smallB.Length; i++)
                {
                    if (i == rowIndex - 1)
                    {
                        smallB[i] = currentRhs;
                        break;
                    }
                }


                double[] bOptimal = new double[BInverse.GetLength(1)];
                for (int i = 0; i < BInverse.GetLength(0); i++)
                {
                    double sum = 0;
                    for (int j = 0; j < BInverse.GetLength(1); j++)
                    {
                        sum += BInverse[i, j] * smallB[j];
                    }
                    bOptimal[i] = sum;
                }

                for (int j = 0; j < bOptimal.Length; j++)
                {
                    if (bOptimal[j] < 0)
                    {
                        double range = originalTab.Tableau[rowIndex, originalTab.Tableau.GetLength(1) - 1] - (currentRhs);
                        upperRange = ($"Allowabe  increase {range}");
                        flag = true;
                    }

                }

                gaurd++;
            }
           return (lowerRange,upperRange);
        }   

        public (double[],double) change_RightHandSide(int rowIndex, double newValue)
        {
            double[] cbv = getCbv();
            double[,] BInverse = InverseMatrix(constructBMatrix());
            double[] smallB = computeSmallB();
            double newRhs = newValue;

            for (int i = 0; i < smallB.Length; i++)
            {
                if (i == rowIndex - 1)
                {
                    smallB[i] = newRhs;
                    break;
                }
            }

            double[] bOptimal = new double[BInverse.GetLength(1)];
            for (int i = 0; i < BInverse.GetLength(0); i++)
            {
                double sum = 0;
                for (int j = 0; j < BInverse.GetLength(1); j++)
                {
                    sum += BInverse[i, j] * smallB[j];
                }
                bOptimal[i] = sum;
            }

            double optimalValue = 0;
            for (int i = 0; i < cbv.Length; i++)
            {
                optimalValue += cbv[i] * bOptimal[i];
            }
            return (bOptimal,optimalValue);
        }

        public TableauTemplate AddRow(double[] newConstraintRow, string newConstraintName)
        {
            int m = optimalTab.Tableau.GetLength(0); // current rows
            int n = optimalTab.Tableau.GetLength(1); // current cols (includes RHS)

            // new tableau: one more row, one more column (new slack)
            double[,] newTableau = new double[m + 1, n + 1];

            // copy old values into newTableau
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n - 1; j++) // copy all cols except RHS
                    newTableau[i, j] = optimalTab.Tableau[i, j];

            // copy RHS column into the last column of newTableau
            for (int i = 0; i < m; i++)
                newTableau[i, n] = optimalTab.Tableau[i, n - 1];

            // fill new row coefficients (decision vars part)
            for (int j = 0; j < newConstraintRow.Length - 1; j++)
                newTableau[m, j] = newConstraintRow[j];

            // add RHS for the new constraint at the last column
            newTableau[m, n] = newConstraintRow[^1];

            // add slack column at index n-1 (just before RHS)
            for (int i = 0; i < m; i++)
                newTableau[i, n - 1] = 0;
            newTableau[m, n - 1] = 1;

            // maintain basic variables (row ops to keep tableau valid)
            for (int j = 0; j < n - 1; j++)
            {
                if (Math.Abs(newTableau[m, j]) > 1e-9)
                {
                    int basicRow = -1;
                    int ones = 0;
                    for (int i = 1; i < m; i++)
                    {
                        if (Math.Abs(newTableau[i, j] - 1) < 1e-9)
                        {
                            basicRow = i;
                            ones++;
                        }
                        else if (Math.Abs(newTableau[i, j]) > 1e-9)
                        {
                            ones = -99;
                            break;
                        }
                    }

                    if (ones == 1 && basicRow != -1)
                    {
                        double factor = newTableau[m, j];
                        for (int k = 0; k < n + 1; k++)
                            newTableau[m, k] -= factor * newTableau[basicRow, k];
                    }
                }
            }

            // ensure RHS is non-negative
           
                for (int j = 0; j < n + 1; j++)
                    newTableau[m, j] *= -1;
            

            // update headers: keep old headers but insert slack before RHS
            string[] newRowHeaders = optimalTab.RowHeaders.Concat(new[] { newConstraintName }).ToArray();
            string[] newColHeaders = optimalTab.ColHeaders
                .Take(n - 1) // old cols except RHS
                .Concat(new[] { $"s{m}" }) // new slack
                .Concat(new[] { "RHS" })   // RHS stays last
                .ToArray();

            TableauTemplate updated = new TableauTemplate(
                newTableau,
                newColHeaders,
                newRowHeaders,
                0, null, null, null
            );

            Console.WriteLine(updated.ToString());
            TableauTemplate result = DualSimplex(updated);
            SimplexResult result1 = SolveFromTableau(result.Tableau, result.ColHeaders, result.RowHeaders);
            foreach (var tab in result1.Tableaus)
            {
                Console.WriteLine(tab.ToString());
            }
            return result1.Tableaus.Last();
        }

        public TableauTemplate AddColumn(double[] fullColumn, string newColumnName)
        {
            int m = optimalTab.Tableau.GetLength(0); 
            int n = optimalTab.Tableau.GetLength(1); 

            if (fullColumn.Length != m)
                throw new ArgumentException("Column length must match tableau row count.");

            
            double[,] newTableau = new double[m, n + 1];

            
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                    newTableau[i, j] = originalTab.Tableau[i, j];

            
            for (int i = 0; i < m; i++)
                newTableau[i, n] = newTableau[i, n - 1];

            
            for (int i = 0; i < m; i++)
            {
                if (i == 0)
                    newTableau[i, n - 1] = -fullColumn[0]; 
                else
                    newTableau[i, n - 1] = fullColumn[i];  
            }

            // Update column headers
            string[] newColHeaders = originalTab.ColHeaders
                .Take(n - 1)                     
                .Concat(new[] { newColumnName }) 
                .Concat(new[] { "RHS" })         
                .ToArray();

            TableauTemplate updated = new TableauTemplate(
                newTableau,
                newColHeaders,
                originalTab.RowHeaders.ToArray(),
                0, null, null, null
            );

            Console.WriteLine(updated.ToString());
            updated = DualSimplex(updated);
            Console.WriteLine(updated.ToString());
            return updated;
        }

        public string display_shadow_Prices()
        {

            string result = "Shadow Prices:\n";
            result += "==============\n";

            // Look for slack/surplus variables in the optimal tableau
            bool foundAny = false;
            int constraintNumber = 1;

            for (int j = 0; j < optimalTab.ColHeaders.Length; j++)
            {
                string varName = optimalTab.ColHeaders[j];

                // Check if it's a slack or surplus variable
                if (varName.ToLower().StartsWith("s") || varName.ToLower().Contains("slack") ||
                    varName.ToLower().Contains("surplus"))
                {
                    double shadowPrice = optimalTab.Tableau[0, j]; // Z-row value
                    result += $"Constraint {constraintNumber} ({varName}): Shadow Price = {shadowPrice:F3}\n";

                    
                    if (Math.Abs(shadowPrice) < 1e-6) 
                    {
                        result += $"Constraint {constraintNumber} is not binding (has slack/surplus)\n";
                    }
                    else
                    {
                        result += $"Increasing RHS of constraint {constraintNumber} by 1 unit changes objective by {shadowPrice:F3}\n";
                    }
                    result += "\n";

                    foundAny = true;
                    constraintNumber++;
                }
            }

            if (!foundAny)
            {
                result += "No slack/surplus variables found in the tableau.\n";
            }

            return result;

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
                for (int i = 1; i < m; i++)
                {
                    double b = tab.Tableau[i, rhs];
                    if (b < mostNeg)
                    {
                        mostNeg = b;
                        pr = i;
                        Console.WriteLine($"Pivot row {pr}");
                    }
                }

                // If none negative -> primal feasible -> done (since z-row is unchanged)
                if (pr == -1) break;

                // 2) entering column via dual ratio test: argmin_j { z_j / (-a_prj) | a_prj < 0 }
                int pc = -1;
                double bestRatio = double.PositiveInfinity;
                double[] dualRatios = new double[rhs];
                for (int j = 0; j < rhs; j++) // exclude RHS
                {
                    double a = tab.Tableau[pr, j];
                    if (a < -eps)
                    {
                        double zj = tab.Tableau[zRow, j];
                        double ratio = zj / (-a);
                        dualRatios[j] = ratio;
                        if (ratio < bestRatio - 1e-15 && ratio != 0)
                        {
                            bestRatio = ratio;
                            pc = j;
                            Console.WriteLine($"Best ratio {bestRatio} and pivot columne {pc}");
                        }
                    }
                }

                if (pc == -1)
                {

                    throw new InvalidOperationException("LP is primal infeasible (dual unbounded).");
                }
                // Create and store PriceOutInfo information for display



                // 3) pivot on (pr, pc)
                tab.Tableau = Pivot(tab.Tableau, pr, pc, eps);
                double pivotElement = tab.Tableau[pr, pc];

                // create and store PivotInfo information for display



            }
            return tab;

        }

        private static double[,] Pivot(double[,] T, int pr, int pc, double eps) // pr = pivot row, pc = pivot column
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

        public double GoldenSectionSearch(string expression, double xlo, double xhi, double tolerance = 1e-6)
    {
        const double GoldenRatio = 0.6180339887498949; // (3 - sqrt(5)) / 2

        if (xlo >= xhi)
            throw new ArgumentException("xlo must be less than xhi");

        try
        {
            double x1 = xhi - GoldenRatio * (xhi - xlo);
            double x2 = xlo + GoldenRatio * (xhi - xlo);

            var expr1 = new Expression(expression);
            expr1.Parameters["x"] = x1;
            double f1 = Convert.ToDouble(expr1.Evaluate());

            var expr2 = new Expression(expression);
            expr2.Parameters["x"] = x2;
            double f2 = Convert.ToDouble(expr2.Evaluate());

            while ((xhi - xlo) > tolerance)
            {
                if (f1 <= f2)
                {
                    xhi = x2;
                    x2 = x1;
                    f2 = f1;
                    x1 = xhi - GoldenRatio * (xhi - xlo);
                    expr1.Parameters["x"] = x1;
                    f1 = Convert.ToDouble(expr1.Evaluate());
                }
                else
                {
                    xlo = x1;
                    x1 = x2;
                    f1 = f2;
                    x2 = xlo + GoldenRatio * (xhi - xlo);
                    expr2.Parameters["x"] = x2;
                    f2 = Convert.ToDouble(expr2.Evaluate());
                }
            }

            return (xlo + xhi) / 2;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid expression: {expression}. Error: {ex.Message}");
        }
    }

        public  static SimplexResult SolveFromTableau(double[,] tableau, string[] colHeaders, string[] rowHeaders,bool isMaximization = true, double eps = 1e-9)
        {
            // Clone the input tableau to avoid modifying the original
            int rows = tableau.GetLength(0);
            int cols = tableau.GetLength(1);
            double[,] T = (double[,])tableau.Clone();
            //print table 
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write($"{T[i, j],8:F2} ");
                }
                Console.WriteLine();
            }
            string[] colHeadersCopy = (string[])colHeaders.Clone();
            string[] rowHeadersCopy = (string[])rowHeaders.Clone();

            // Validate RHS for feasibility (check constraint rows, skip objective row)
            for (int i = 1; i < rows; i++)
            {
                if (T[i, cols - 1] < -eps)
                {
                    return new SimplexResult
                    {
                        IsInfeasible = true,
                        Message = "Infeasible: RHS contains negative values. Phase I required."
                    };
                }
            }

            var result = new SimplexResult();
            int iteration = 0;
            string problemType = isMaximization ? "MAX" : "MIN";
            result.Tableaus.Add(new TableauTemplate(T, colHeadersCopy, rowHeadersCopy, iteration, null, null, $"Initial tableau ({problemType})"));

            while (true)
            {
                iteration++;

                int pivotColumn = -1;
                double bestValue = isMaximization ? 0 : eps; // For MAX: look for most negative, for MIN: look for most positive

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
                    // Extract solution - count decision variables from headers
                    int numDecisionVars = 0;
                    for (int j = 0; j < cols - 1; j++)
                    {
                        if (colHeaders[j].StartsWith("x", StringComparison.OrdinalIgnoreCase))
                            numDecisionVars++;
                    }

                    var x = new double[numDecisionVars];

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
                                if (isBasicVariable && colHeaders[j].StartsWith("x", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Extract variable index from header (e.g., "x1" -> index 0)
                                    if (int.TryParse(colHeaders[j].Substring(1), out int varIndex) && varIndex <= numDecisionVars)
                                    {
                                        x[varIndex - 1] = T[i, cols - 1];
                                    }
                                }
                            }
                        }
                    }

                    double objectiveValue = isMaximization ? T[0, cols - 1] : -T[0, cols - 1];

                    result.OptimalValue = objectiveValue;
                    result.PrimalVariables = x;
                    result.Message = $"Optimal solution found ({problemType}).";
                    result.Tableaus.Add(new TableauTemplate(T, colHeadersCopy, rowHeadersCopy, iteration, null, null, $"Optimal tableau ({problemType})"));
                    break;
                }

                // Find leaving variable (minimum ratio test)
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
                    result.Tableaus.Add(new TableauTemplate(T, colHeadersCopy, rowHeadersCopy, iteration, null, pivotColumn, $"Unbounded ({problemType})"));
                    break;
                }

                // Perform pivot operation
                Pivot(T, pivotRow, pivotColumn, eps);

                // Record this iteration
                result.Tableaus.Add(new TableauTemplate(T, colHeadersCopy, rowHeadersCopy, iteration, pivotRow, pivotColumn, $"Iteration {iteration} ({problemType})"));
            }

            return result;
        }




    }
}
