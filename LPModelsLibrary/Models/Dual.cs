using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPModelsLibrary.Models
{
    public class Dual
    {


        public TableauTemplate SolveDualFromPrimal(double[] c, double[,] A, double[] b, double eps = 1e-9)
        {
            int m = b.Length;               // number of primal constraints
            int n = c.Length;               // number of primal variables

            
            double[,] tableau = new double[n + 1, m + 1]; 

            
            for (int j = 0; j < m; j++)
            {
                tableau[0, j] = b[j];
            }

           
            for (int i = 0; i < n; i++) 
            {
                for (int j = 0; j < m; j++)
                {
                    tableau[i + 1, j] = A[j, i]; 
                }
                tableau[i + 1, m] = -1*c[i]; 
            }

            
            for (int i = 1; i <= n; i++)
            {
                tableau[i, m] = -1*tableau[i, m];
            }

            
            string[] colHeaders = new string[m + 1];
            for (int j = 0; j < m; j++)
                colHeaders[j] = $"y{j + 1}";
            colHeaders[m] = "RHS";

            string[] rowHeaders = new string[n + 1];
            rowHeaders[0] = "z";
            for (int i = 0; i < n; i++)
                rowHeaders[i + 1] = $"x{i + 1}";

            TableauTemplate dualTableau = new TableauTemplate
            (
                tableau,
                colHeaders,
                rowHeaders, 0, null, null
            );

            
            return DualSimplex(dualTableau, eps);
        }

        public TableauTemplate BuildDualTableau(double[] c, double[,] A, double[] b)
        {
            int m = b.Length;               // number of primal constraints
            int n = c.Length;               // number of primal variables

           
            double[,] dualA = new double[n, m];  // dual constraint matrix
            double[] dualB = new double[n];      // RHS of dual
            double[] dualC = new double[m];      // objective of dual

            
            for (int i = 0; i < m; i++)
                dualC[i] = -1*b[i];

            
            for (int j = 0; j < n; j++)
                dualB[j] = -1*c[j];

            
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                    dualA[i, j] = -1 * A[j, i];
            }

           
            int rows = n + 1;               
            int cols = m + n + 1;           
            double[,] tableau = new double[rows, cols];

           
            for (int j = 0; j < m; j++)
                tableau[0, j] = -dualC[j];   

           
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                    tableau[i + 1, j] = dualA[i, j];

                tableau[i + 1, m + i] = 1;                 
            }

            
            string[] colHeaders = new string[m + n + 1];
            for (int j = 0; j < m; j++)
                colHeaders[j] = "y" + (j + 1);
            for (int j = 0; j < n; j++)
                colHeaders[m + j] = "e" + (j + 1);
            colHeaders[cols - 1] = "RHS";

            
            string[] rowHeaders = new string[rows];
            rowHeaders[0] = "Z";
            for (int i = 0; i < n; i++)
                rowHeaders[i + 1] = "Constraint " + (i + 1);

            return new TableauTemplate
            (
                tableau,
                colHeaders,
                rowHeaders,0,null,null
            );
        }




        public TableauTemplate DualSimplex(TableauTemplate tab, double eps = 1e-9)
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
