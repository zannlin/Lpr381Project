using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
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

        public (double lowerbound,double upperbound) Range_Of_NonBasic_Variable(int columnIndex)
        {

            Console.WriteLine("Range of Non Basic Variable");

            Console.WriteLine("Optimal Tableau");
            Console.WriteLine(optimalTab.ToString());
            // Cj∗​=cBV​B−1aj​−(cj​+Δ) Formulate for non basic variable
            double cj = originalTab.Tableau[0, columnIndex]*-1;
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

            return (2,2);
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

        private List<int> getBasicVarColumnIndex()
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





        private void range_Of_Basic_Variable(int columnIndex)
        {
            // should return the range of a basic variable
        }

        private void change_nonBasic_Variable_Coefficient(int columnIndex, double newCoefficient)
        {
            // should return the new optimal solution after changing the coefficient of a non basic variable
        }

        private void change_Basic_Variable_Coefficient(int columnIndex, double newCoefficient)
        {
            // should return the new optimal solution after changing the coefficient of a basic variable
        }

        private void range_RightHandSide(int rowIndex)
        {
            // should return the range of a right hand side value of a constraint
        }

        private void change_RightHandSide(int rowIndex, double newValue)
        {
            // should return the new optimal solution after changing the right hand side value of a constraint
        }

        private void add_column(double[] newColumn, string columnName, double newCoefficient)
        {
            // should return the new optimal solution after adding a new variable to the model
        }   
        private void add_row(double[] newRow, string rowName, double newRHS)
        {
            // should return the new optimal solution after adding a new constraint to the model
        }

        private string display_shadow_Prices()
        {

            return "";
            // should return the shadow prices of the constraints
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






    }
}
