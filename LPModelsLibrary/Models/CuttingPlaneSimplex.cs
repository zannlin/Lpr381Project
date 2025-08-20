using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPModelsLibrary.Models
{
    public class CuttingPlaneSimplex
    {
        public SimplexResult cuttingPlaneResult;
        public TableauTemplate optimalTableau;


        public CuttingPlaneSimplex(SimplexResult result) 
        {
            cuttingPlaneResult = result;
            if (cuttingPlaneResult == null)
            {
                throw new InvalidOperationException("No Simplex Result found in the result.");
            }
            else
            {
                optimalTableau = cuttingPlaneResult.Tableaus.LastOrDefault();
            }


        }

        public string display()
        {
            return cuttingPlaneResult.Tableaus[cuttingPlaneResult.Tableaus.Count-1].ToString() ?? "No tableau available.";
        }


        public string addNewConstraint()
        {
            // identify the rhs value with the closest fractional value to 0.5 excluding Z row
            double closestFractionalValue = double.MaxValue;
            int closestRowIndex = -1;

            for (int i = 0; i < optimalTableau.Tableau.GetLength(0) - 1; i++) // Exclude last row (Z row)
            {
                double rhsValue = optimalTableau.Tableau[i, optimalTableau.Tableau.GetLength(1) - 1];
                rhsValue = rhsValue - Math.Truncate(rhsValue);// removing the integer part to focus on the fractional part
                if (Math.Abs(rhsValue - 0.5) < Math.Abs(closestFractionalValue - 0.5))
                {
                    closestFractionalValue = rhsValue;
                    closestRowIndex = i;
                }
            }
            // once we have the closest row index, we can add a new constraint.

            if (closestRowIndex != -1)
            {
                // Create a new constraint based on the closest row
                double[] oldRow = new double[optimalTableau.Tableau.GetLength(1)];
                for (int j = 0; j < optimalTableau.Tableau.GetLength(1); j++)
                {
                    oldRow[j] = optimalTableau.Tableau[closestRowIndex, j];
                }

                for (int j = 0; j < oldRow.Length; j++)
                {
                    double newValue = oldRow[j] - Math.Truncate(oldRow[j]);
                      

                    // -0.8833  -> -0.833
                    if (oldRow[j] > 0)
                    {
                        oldRow[j] = newValue;
                    }
                    else
                    {
                        newValue = Math.Abs(newValue);
                        oldRow[j] = 1 - newValue;
                    }
                    oldRow[j] *= -1; // Negate the values to create a new constraint

                }

                return $"New constraint added based on row {closestRowIndex + 1}: " +
                       $"{string.Join(", ", oldRow.Select(x => x.ToString("0.###")))}" +
                       $" with a fractional part closest to 0.5.";

            }
            else

            {
                return null;
            }

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

        private static bool integerCheck()
        {
            return false;
        }
    }
}
