using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPModelsLibrary.Models
{
    public class MathematicalSensitivity
    {
        private TableauTemplate tab;
        public MathematicalSensitivity(TableauTemplate tab)
        {
            this.tab = tab;
        }
        private void SensitivityAnalysis()
        {
            // just output the general sensitivity analysis information
        }

        public (double lowerbound,double upperbound) Range_Of_NonBasic_Variable(int columnIndex)
        {
            return (2,2);
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
