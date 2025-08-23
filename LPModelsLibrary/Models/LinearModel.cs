namespace LPModelsLibrary.Models
{
    public class LinearModel
    {
        private List<string> DummyObjectiveFunction;
        private List<string> DummyConstraint;
        public string problem_ineqaulity = string.Empty;

        public double[] ObjectiveFuntion;
        public double[,] Constraints;
        public double[] RightHandSide;

        public LinearModel(string[] content)
        {

            DummyObjectiveFunction = new List<string>();
            DummyConstraint = new List<string>();
            SplitObjectiveFunctionAndConstraint(content);
            CreateObjectiveFunction();
            createConstraint();
            ApplyBinaryConstraints();

        }

        public void SplitObjectiveFunctionAndConstraint(string[] content)
        {
            int counter = 0;
            foreach (string line in content)
            {

                string[] lineParts = line.Split(' ');
                foreach (string part in lineParts)
                {

                    if (counter == 0)
                    {
                        DummyObjectiveFunction.Add(part);
                    }

                }
                if (counter != 0)
                {
                    DummyConstraint.Add(line);
                }

                counter++;

            }

        }

        public void CreateObjectiveFunction()
        {

            Console.WriteLine($"Objective Function Count {DummyObjectiveFunction.Count}:");
            ObjectiveFuntion = new double[DummyObjectiveFunction.Count];
            DummyObjectiveFunction.RemoveAt(0);



            for (int i = 0; i < DummyObjectiveFunction.Count; i++)
            {
                if (int.TryParse(DummyObjectiveFunction[i], out int value))
                {
                    ObjectiveFuntion[i] = value;
                }
                else
                {
                    Console.WriteLine($"Invalid value in objective function: {DummyObjectiveFunction[i]}");
                }
            }
        }

        public void createConstraint()
        {
            problem_ineqaulity = DummyConstraint[DummyConstraint.Count - 1].Split(' ')[0];
            DummyConstraint.RemoveAt(DummyConstraint.Count - 1); // Remove the last element which is the inequality constraint
            int numberOfConstraints = DummyConstraint.Count;
            Constraints = new double[numberOfConstraints, ObjectiveFuntion.Length - 1];
            int counter = 0;
            RightHandSide = new double[numberOfConstraints];

            // Extract the right-hand side values and the inequality type

            foreach (string constraint in DummyConstraint)
            {

                string[] parts = constraint.Split(' ');
                for (int i = 0; i < Constraints.GetLength(1); i++)
                {

                    if (double.TryParse(parts[i], out double value))
                    {

                        Constraints[counter, i] = value;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid value in constraint: {parts[i]}");
                    }

                }
                Console.WriteLine($"Right Hand Side {parts[parts.Length - 1]}");
                if (double.TryParse(parts[parts.Length - 1].Substring(2), out double rhsValue))
                {
                    RightHandSide[counter] = rhsValue;
                }
                else
                {
                    Console.WriteLine($"Invalid right-hand side value: {parts[parts.Length - 1]}");
                }
                counter++;
            }

            for (int i = 0; i < Constraints.GetLength(0); i++)
            {
                for (int j = 0; j < Constraints.GetLength(1); j++)
                {
                    Console.Write($"{Constraints[i, j]} ");
                }
                Console.Write(RightHandSide[i]);

                Console.WriteLine();
            }
        }

        public void PrintObjectiveFunction()
        {
            for (int i = 0; i < ObjectiveFuntion.Length; i++)
            {
                Console.WriteLine($"Objective Function {i}: {ObjectiveFuntion[i]}");
            }


        }

        public void PrintConstraint()
        {
            for (int i = 0; i < DummyConstraint.Count; i++)
            {
                Console.WriteLine(DummyConstraint[i]);
            }
            Console.WriteLine($"Constraint inequality {problem_ineqaulity}");
        }

        public void ApplyBinaryConstraints()
        {
            if (problem_ineqaulity.ToLower() == "bin")
            {
                int nVars = ObjectiveFuntion.Length;
                int oldConstraints = Constraints.GetLength(0);
                int newConstraints = oldConstraints + nVars;

                double[,] newConstraintsMatrix = new double[newConstraints, nVars];
                double[] newRhs = new double[newConstraints];

                // Copy old constraints to new variables
                for (int i = 0; i < oldConstraints; i++)
                {
                    for (int j = 0; j < nVars-1; j++) // we say minus 1 becuase we dont want to include "max" as part of the count
                    {
                        newConstraintsMatrix[i, j] = Constraints[i, j];
                    }
                    newRhs[i] = RightHandSide[i];
                }

                // Add xi <= 1 constraints
                int row = oldConstraints;
                for (int i = 0; i < nVars; i++)
                {

                    newConstraintsMatrix[row, i] = 1;
                    newRhs[row] = 1;
                    row++;
                }

                // Replace old constraints
                Constraints = newConstraintsMatrix;
                RightHandSide = newRhs;
            }
        }
    }
}
