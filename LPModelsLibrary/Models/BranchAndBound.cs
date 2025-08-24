using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPModelsLibrary.Models
{

    public class BranchAndBoundNode
    {
        public int NodeId { get; set; }
        public int? ParentId { get; set; }
        public double[] LowerBounds { get; set; }
        public double[] UpperBounds { get; set; }
        public SimplexResult Result { get; set; }
        public string Status { get; set; }
        public double[,] Constraints { get; set; }
        public double[] RHS { get; set; }
        public double[] Objective { get; set; }
        public char[] ConstraintTypes { get; set; }
    }

    public class BranchAndBoundResult
    {
        public double OptimalValue { get; set; }
        public double[] OptimalSolution { get; set; }
        public string Message { get; set; } = "";
        public List<BranchAndBoundNode> Nodes { get; set; } = new List<BranchAndBoundNode>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Message);
            if (OptimalSolution != null)
            {
                sb.AppendLine($"Optimal Solution: [{string.Join(", ", OptimalSolution.Select(x => x.ToString("F3")))}]");
                sb.AppendLine($"Optimal Value: {OptimalValue:F3}");
            }
            sb.AppendLine("\nBranch and Bound Tree:");
            foreach (var node in Nodes)
            {
                sb.AppendLine($"Node {node.NodeId} (Parent: {node.ParentId?.ToString() ?? "None"})");
                sb.AppendLine($"Status: {node.Status}");
                sb.AppendLine($"Bounds: [{string.Join(", ", Enumerable.Range(0, node.LowerBounds.Length).Select(j => $"x{j + 1}: [{node.LowerBounds[j]:F3}, {(node.UpperBounds[j] >= 1e308 ? "∞" : node.UpperBounds[j].ToString("F3"))}]"))}]");
                if (node.Result != null)
                {
                    sb.AppendLine($"LP Result: {node.Result.Message}");
                    if (node.Result.PrimalVariables.Any())
                        sb.AppendLine($"Solution: [{string.Join(", ", node.Result.PrimalVariables.Select(x => x.ToString("F3")))}]");
                    foreach (var tableau in node.Result.Tableaus)
                        sb.AppendLine(tableau.ToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
    public partial class BranchAndBound
    {
        public static BranchAndBoundResult TestRootNode(double[,] A, double[] b, double[] c, char[] constraintTypes, bool[] isInteger, double eps = 1e-9)
        {
            var result = new BranchAndBoundResult();
            int n = A.GetLength(1);

            // Create root node
            var root = new BranchAndBoundNode
            {
                NodeId = 0,
                ParentId = null,
                LowerBounds = new double[n].Select(_ => 0.0).ToArray(),
                UpperBounds = new double[n].Select(_ => double.PositiveInfinity).ToArray(),
                Status = "Root",
                Constraints = (double[,])A.Clone(),
                RHS = (double[])b.Clone(),
                Objective = (double[])c.Clone(),
                ConstraintTypes = (char[])constraintTypes.Clone()
            };

            // Solve LP relaxation
            root.Result = DualSimplex.Solve(root.Constraints, root.RHS, root.Objective, root.ConstraintTypes, isMaximization: true, eps);
            result.Nodes.Add(root);

            if (root.Result.IsInfeasible)
                result.Message = "Root node infeasible.";
            else if (root.Result.IsUnbounded)
                result.Message = "Root node unbounded.";
            else
                result.Message = "Root node solved.";

            return result;
        }

        public static BranchAndBoundResult TestBranching(double[,] A, double[] b, double[] c, char[] constraintTypes, bool[] isInteger, double eps = 1e-9)
        {
            var result = TestRootNode(A, b, c, constraintTypes, isInteger, eps);
            var root = result.Nodes[0];
            int n = A.GetLength(1);
            int nodeCounter = 1;

            if (root.Result.IsInfeasible || root.Result.IsUnbounded)
                return result;

            bool isIntegerSolution = true;
            int fractionalIndex = -1;
            double fractionalValue = 0;
            for (int j = 0; j < n; j++)
            {
                if (isInteger[j] && Math.Abs(root.Result.PrimalVariables[j] - Math.Round(root.Result.PrimalVariables[j])) > eps)
                {
                    isIntegerSolution = false;
                    fractionalIndex = j;
                    fractionalValue = root.Result.PrimalVariables[j];
                    break;
                }
            }

            if (isIntegerSolution)
            {
                root.Status = "Integer solution";
                result.OptimalValue = root.Result.OptimalValue;
                result.OptimalSolution = root.Result.PrimalVariables;
                result.Message = "Optimal integer solution found at root.";
                return result;
            }

            root.Status = $"Branched on x{fractionalIndex + 1} = {fractionalValue:F3}";

            var leftNode = new BranchAndBoundNode
            {
                NodeId = nodeCounter++,
                ParentId = root.NodeId,
                LowerBounds = root.LowerBounds.ToArray(),
                UpperBounds = root.UpperBounds.ToArray(),
                Status = $"Left child (x{fractionalIndex + 1} ≤ {Math.Floor(fractionalValue)})",
                Constraints = (double[,])A.Clone(),
                RHS = (double[])b.Clone(),
                Objective = (double[])c.Clone(),
                ConstraintTypes = (char[])constraintTypes.Clone()
            };
            leftNode.UpperBounds[fractionalIndex] = Math.Floor(fractionalValue);

            var rightNode = new BranchAndBoundNode
            {
                NodeId = nodeCounter++,
                ParentId = root.NodeId,
                LowerBounds = root.LowerBounds.ToArray(),
                UpperBounds = root.UpperBounds.ToArray(),
                Status = $"Right child (x{fractionalIndex + 1} ≥ {Math.Ceiling(fractionalValue)})",
                Constraints = (double[,])A.Clone(),
                RHS = (double[])b.Clone(),
                Objective = (double[])c.Clone(),
                ConstraintTypes = (char[])constraintTypes.Clone()
            };
            rightNode.LowerBounds[fractionalIndex] = Math.Ceiling(fractionalValue);

            result.Nodes.Add(leftNode);
            result.Nodes.Add(rightNode);
            result.Message = "Branched at root node.";

            return result;
        }

        public static BranchAndBoundResult TestChildNodes(double[,] A, double[] b, double[] c, char[] constraintTypes, bool[] isInteger, double eps = 1e-9)
        {
            var result = TestBranching(A, b, c, constraintTypes, isInteger, eps);
            var nodesToSolve = result.Nodes.Where(n => n.Status.Contains("child")).ToList();

            foreach (var node in nodesToSolve)
            {
                // Count additional constraints from bounds
                int n = A.GetLength(1);
                int extraConstraints = 0;
                for (int j = 0; j < n; j++)
                {
                    if (node.LowerBounds[j] > 0.0 + eps) extraConstraints++;
                    if (node.UpperBounds[j] < double.PositiveInfinity - eps) extraConstraints++;
                }

                // Create new constraint matrix and RHS
                int m = A.GetLength(0);
                double[,] newA = new double[m + extraConstraints, n];
                double[] newB = new double[m + extraConstraints];
                char[] newConstraintTypes = new char[m + extraConstraints];

                // Copy original constraints
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++) newA[i, j] = A[i, j];
                    newB[i] = b[i];
                    newConstraintTypes[i] = constraintTypes[i];
                }

                // Add bound constraints
                int currentRow = m;
                for (int j = 0; j < n; j++)
                {
                    if (node.LowerBounds[j] > 0.0 + eps)
                    {
                        newA[currentRow, j] = 1.0;
                        newB[currentRow] = node.LowerBounds[j];
                        newConstraintTypes[currentRow] = 'G';
                        currentRow++;
                    }
                    if (node.UpperBounds[j] < double.PositiveInfinity - eps)
                    {
                        newA[currentRow, j] = 1.0;
                        newB[currentRow] = node.UpperBounds[j];
                        newConstraintTypes[currentRow] = 'L';
                        currentRow++;
                    }
                }

                // Solve LP with bounds
                node.Result = DualSimplex.Solve(newA, newB, node.Objective, newConstraintTypes, isMaximization: true, eps);
                node.Status = node.Result.IsInfeasible ? "Infeasible" :
                              node.Result.IsUnbounded ? "Unbounded" :
                              $"Solved (z = {node.Result.OptimalValue:F3})";
            }

            result.Message = "Child nodes solved.";
            return result;
        }
    }
}
