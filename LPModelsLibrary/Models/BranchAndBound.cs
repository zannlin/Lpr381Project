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
        public static BranchAndBoundResult SolveNode(double[,] A, double[] b, double[] c, char[] constraintTypes, bool[] isInteger, double eps = 1e-9)
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

        public static BranchAndBoundResult Solve(double[,] A, double[] b, double[] c, char[] constraintTypes, bool[] isInteger, double eps = 1e-9)
        {
            var result = SolveNode(A, b, c, constraintTypes, isInteger, eps);
            int n = A.GetLength(1);
            int nodeCounter = 1;
            double bestValue = double.NegativeInfinity;
            double[] bestSolution = null;

            // Queue for nodes to process
            var queue = new Queue<BranchAndBoundNode>();
            queue.Enqueue(result.Nodes[0]);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                // Skip if infeasible or unbounded, but still include in nodes for completeness
                if (node.Result.IsInfeasible || node.Result.IsUnbounded)
                    continue;

                // Check if the solution is integer
                bool isIntegerSolution = true;
                int fractionalIndex = -1;
                double fractionalValue = 0;
                for (int j = 0; j < n; j++)
                {
                    if (isInteger[j] && Math.Abs(node.Result.PrimalVariables[j] - Math.Round(node.Result.PrimalVariables[j])) > eps)
                    {
                        isIntegerSolution = false;
                        fractionalIndex = j;
                        fractionalValue = node.Result.PrimalVariables[j];
                        break;
                    }
                }

                if (isIntegerSolution)
                {
                    node.Status = "Integer solution";
                    if (node.Result.OptimalValue > bestValue)
                    {
                        bestValue = node.Result.OptimalValue;
                        bestSolution = node.Result.PrimalVariables.ToArray();
                    }
                    continue;
                }

                // Branch on the fractional variable
                node.Status = $"Branched on x{fractionalIndex + 1} = {fractionalValue:F3}";

                // Create left child (x_j <= floor(value))
                var leftNode = new BranchAndBoundNode
                {
                    NodeId = nodeCounter++,
                    ParentId = node.NodeId,
                    LowerBounds = node.LowerBounds.ToArray(),
                    UpperBounds = node.UpperBounds.ToArray(),
                    Status = $"Left child (x{fractionalIndex + 1} ≤ {Math.Floor(fractionalValue)})",
                    Constraints = (double[,])A.Clone(),
                    RHS = (double[])b.Clone(),
                    Objective = (double[])c.Clone(),
                    ConstraintTypes = (char[])constraintTypes.Clone()
                };
                leftNode.UpperBounds[fractionalIndex] = Math.Floor(fractionalValue);

                // Create right child (x_j >= ceil(value))
                var rightNode = new BranchAndBoundNode
                {
                    NodeId = nodeCounter++,
                    ParentId = node.NodeId,
                    LowerBounds = node.LowerBounds.ToArray(),
                    UpperBounds = node.UpperBounds.ToArray(),
                    Status = $"Right child (x{fractionalIndex + 1} ≥ {Math.Ceiling(fractionalValue)})",
                    Constraints = (double[,])A.Clone(),
                    RHS = (double[])b.Clone(),
                    Objective = (double[])c.Clone(),
                    ConstraintTypes = (char[])constraintTypes.Clone()
                };
                rightNode.LowerBounds[fractionalIndex] = Math.Ceiling(fractionalValue);

                // Add bound constraints and solve LP for each child
                foreach (var childNode in new[] { leftNode, rightNode })
                {
                    // Count additional constraints from bounds
                    int extraConstraints = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (childNode.LowerBounds[j] > 0.0 + eps) extraConstraints++;
                        if (childNode.UpperBounds[j] < double.PositiveInfinity - eps) extraConstraints++;
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
                        if (childNode.LowerBounds[j] > 0.0 + eps)
                        {
                            newA[currentRow, j] = 1.0;
                            newB[currentRow] = childNode.LowerBounds[j];
                            newConstraintTypes[currentRow] = 'G';
                            currentRow++;
                        }
                        if (childNode.UpperBounds[j] < double.PositiveInfinity - eps)
                        {
                            newA[currentRow, j] = 1.0;
                            newB[currentRow] = childNode.UpperBounds[j];
                            newConstraintTypes[currentRow] = 'L';
                            currentRow++;
                        }
                    }

                    // Solve LP with bounds
                    childNode.Result = DualSimplex.Solve(newA, newB, childNode.Objective, newConstraintTypes, isMaximization: true, eps);
                    childNode.Status = childNode.Result.IsInfeasible ? "Infeasible" :
                                       childNode.Result.IsUnbounded ? "Unbounded" :
                                       $"Solved (z = {childNode.Result.OptimalValue:F3})";

                    result.Nodes.Add(childNode);
                    queue.Enqueue(childNode);
                }
            }

            if (bestSolution != null)
            {
                result.OptimalValue = bestValue;
                result.OptimalSolution = bestSolution;
                result.Message = "Optimal integer solution found.";
            }
            else
            {
                result.Message = "No feasible integer solution found.";
            }

            return result;
        }
    }
}
