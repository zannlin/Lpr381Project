using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPModelsLibrary.Models
{
    public class Dual
    {
        public double[] ObjectiveFunction { get; set; }   // Dual objective coefficients
        public double[,] Constraints { get; set; }        // Dual constraint matrix
        public double[] RightHandSide { get; set; }
        public Dual ConvertToDual(double[] objectiveFunction, double[,] constraints, double[] rhs,int objectiveFunctionCount)
        {
            Console.WriteLine(objectiveFunctionCount);
            int m = rhs.Length;               
            int n = objectiveFunction.Length;

           
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"x{i+1}  = {objectiveFunction[i]}");
            }

            for(int  i = 0; i < m; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    Console.Write($"{constraints[i,j]} ");
                }
                Console.WriteLine();
            }

            for (int i = 0; i < m; i++)
            {
                Console.WriteLine($"RHS {i} = {rhs[i]}");
            }



            // we need to also remove the slack variables from the objective function


            // Dual Objective Function = RHS of primal
            double[] dualObjective = new double[m];
            for (int i = 0; i < m; i++)
                dualObjective[i] = rhs[i];

            // Dual Constraints = transpose of A
            double[,] dualConstraints = new double[n, m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    dualConstraints[i, j] = constraints[j, i];

            // Dual RHS = primal objective function
            double[] dualRHS = new double[n];
            for (int i = 0; i < n; i++)
                dualRHS[i] = objectiveFunction[i];


            Console.WriteLine("\n\n\n");
            for(int i = 0; i < n; i++)
            {
                Console.WriteLine($"Dual RHS {i} = {dualRHS[i]}");
            }

            for(int i = 0; i < m; i++)
            {
                Console.WriteLine($"Dual Objective {i} = {dualObjective[i]}");
            }

            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < m; j++)
                {
                    Console.Write($"{dualConstraints[i,j]} ");
                }
                Console.WriteLine();
            }

            return new Dual
            {
                ObjectiveFunction = dualObjective,
                Constraints = dualConstraints,
                RightHandSide = dualRHS
            };
        }
    }
}
