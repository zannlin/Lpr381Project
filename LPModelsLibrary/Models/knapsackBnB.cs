using LPModelsLibrary.Models;
using System.Text;

namespace LprProject.Models
{
    internal class knapsackBnB
    {
        public class Item
        {
            public int Index { get; set; }
            public double Value { get; set; }
            public double Weight { get; set; }
            public double Density => Value / Weight;
        }

        public class Node
        {
            public int Level { get; set; } // Current item index (0-based)
            public double CurrentValue { get; set; }
            public double RemainingCapacity { get; set; }
            public double UpperBound { get; set; }
            public List<int> Assignment { get; set; } = new List<int>(); // 1 if taken, 0 if not, up to Level+1
            public string Status { get; set; } = "Active"; // "Active", "Fathomed by bound", "Fathomed by integer", "Infeasible"
            public Node Parent { get; set; }
            public SimplexResult LpResult { get; set; } // Store LP relaxation result
            public char? CandidateLetter { get; set; } = null;
            public string AssignmentString => string.Join(",", Assignment);
        }

        private List<Item> items;
        private double capacity;
        private double bestValue = 0;
        private List<List<int>> optimalAssignments = new List<List<int>>();
        private List<Node> allNodes = new List<Node>();

        public knapsackBnB(double[] values, double[] weights, double capacity)
        {
            if (values.Length != weights.Length)
                throw new ArgumentException("Values and weights must have the same length.");

            this.capacity = capacity;
            items = new List<Item>();
            for (int i = 0; i < values.Length; i++)
            {
                items.Add(new Item { Index = i, Value = values[i], Weight = weights[i] });
            }

            // Sort items by density descending for better bounding
            items = items.OrderByDescending(it => it.Density).ToList();
        }

        public void Solve()
        {
            // Start with root node
            Node root = new Node { Level = -1, CurrentValue = 0, RemainingCapacity = capacity };
            root.UpperBound = CalculateUpperBound(root);
            allNodes.Add(root);

            // DFS stack for backtracking
            Stack<Node> stack = new Stack<Node>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                Node current = stack.Pop();

                if (current.Status != "Active") continue;

                // Fathom by bound
                if (current.UpperBound <= bestValue)
                {
                    current.Status = "Fathomed by bound";
                    continue;
                }

                // Check for integrality if LP was solved
                if (current.LpResult != null)
                {
                    bool is_integer = current.LpResult.PrimalVariables.All(x => Math.Abs(x - Math.Round(x)) < 1e-6 && (Math.Round(x) == 0 || Math.Round(x) == 1));
                    if (is_integer)
                    {
                        double int_value = current.CurrentValue;
                        var free_items = items.Skip(current.Level + 1).ToList();
                        for (int k = 0; k < free_items.Count; k++)
                        {
                            int_value += Math.Round(current.LpResult.PrimalVariables[k]) * free_items[k].Value;
                        }

                        List<int> full_assign = ConstructFullAssignment(current.Assignment, current.LpResult.PrimalVariables);

                        if (int_value > bestValue + 1e-6)
                        {
                            bestValue = int_value;
                            optimalAssignments.Clear();
                        }
                        optimalAssignments.Add(full_assign);
                        current.CandidateLetter = (char)('A' + optimalAssignments.Count - 1);
                        current.Status = "Fathomed by integer";
                        continue;
                    }
                }

                // If at leaf (all items decided)
                if (current.Level == items.Count - 1)
                {
                    if (current.UpperBound > bestValue + 1e-6)
                    {
                        bestValue = current.UpperBound;
                        optimalAssignments.Clear();
                    }
                    optimalAssignments.Add(new List<int>(current.Assignment));
                    current.CandidateLetter = (char)('A' + optimalAssignments.Count - 1);
                    current.Status = "Fathomed by integer";
                    continue;
                }

                // Branch on next item
                int nextItemIndex = current.Level + 1;
                Item nextItem = items[nextItemIndex];

                // Branch 1: Take the item (if feasible)
                if (current.RemainingCapacity >= nextItem.Weight)
                {
                    Node take = new Node
                    {
                        Level = nextItemIndex,
                        CurrentValue = current.CurrentValue + nextItem.Value,
                        RemainingCapacity = current.RemainingCapacity - nextItem.Weight,
                        Assignment = new List<int>(current.Assignment),
                        Parent = current
                    };
                    take.Assignment.Add(1);
                    take.UpperBound = CalculateUpperBound(take);
                    if (!allNodes.Any(n => n.Level == take.Level && n.AssignmentString == take.AssignmentString))
                        allNodes.Add(take);
                    stack.Push(take);
                }

                // Branch 0: Don't take
                Node dontTake = new Node
                {
                    Level = nextItemIndex,
                    CurrentValue = current.CurrentValue,
                    RemainingCapacity = current.RemainingCapacity,
                    Assignment = new List<int>(current.Assignment),
                    Parent = current
                };
                dontTake.Assignment.Add(0);
                dontTake.UpperBound = CalculateUpperBound(dontTake);
                if (!allNodes.Any(n => n.Level == dontTake.Level && n.AssignmentString == dontTake.AssignmentString))
                    allNodes.Add(dontTake);
                stack.Push(dontTake);
            }
        }

        private double CalculateUpperBound(Node node)
        {
            double fixed_value = node.CurrentValue;
            double remaining_capacity = node.RemainingCapacity;

            // Get remaining items
            var remaining_items = items.Skip(node.Level + 1).ToList();
            int num_remaining = remaining_items.Count;

            if (num_remaining == 0) return fixed_value;

            // Build LP relaxation: maximize sum(v_i * x_i) s.t. sum(w_i * x_i) <= remaining_capacity, 0 <= x_i <= 1
            double[,] A = new double[1 + num_remaining, num_remaining]; // 1 for capacity constraint, num_remaining for x_i <= 1
            double[] b = new double[1 + num_remaining];
            double[] c = new double[num_remaining];

            // Capacity constraint
            for (int k = 0; k < num_remaining; k++)
            {
                A[0, k] = remaining_items[k].Weight;
                c[k] = remaining_items[k].Value;
            }
            b[0] = remaining_capacity;

            // Upper bound constraints: x_i <= 1
            for (int k = 0; k < num_remaining; k++)
            {
                A[1 + k, k] = 1;
                b[1 + k] = 1;
            }

            // Solve LP relaxation
            SimplexResult res = PrimalSimplex.Solve(A, b, c);
            node.LpResult = res;

            if (res.IsInfeasible)
            {
                node.Status = "Infeasible";
                return double.NegativeInfinity;
            }
            if (res.IsUnbounded) return double.PositiveInfinity;

            return fixed_value + res.OptimalValue;
        }

        private List<int> ConstructFullAssignment(List<int> partial, double[] free_x)
        {
            List<int> full = new List<int>(partial);
            for (int k = 0; k < free_x.Length; k++)
            {
                full.Add((int)Math.Round(free_x[k]));
            }
            return full;
        }

        public string DisplayResults()
        {
            StringBuilder sb = new StringBuilder();
            int maxItems = items.Count;

            // Display best value and all optimal assignments at the top
            sb.AppendLine($"Optimal value z* = {bestValue.ToString("0.###")}");
            if (optimalAssignments.Any())
            {
                sb.AppendLine("Optimal Assignments:");
                for (int i = 0; i < optimalAssignments.Count; i++)
                {
                    sb.AppendLine($"Candidate {(char)('A' + i)} = {bestValue.ToString("0.###")}");
                }
            }
            sb.AppendLine();

            // Assign hierarchical labels using BFS
            var nodeLabels = new Dictionary<Node, string>();
            var queue = new Queue<(Node node, string label, int depth)>();
            var rootNode = allNodes.FirstOrDefault(n => n.Level == -1);
            if (rootNode != null)
            {
                queue.Enqueue((rootNode, "", 0)); // Root has no number
            }
            else
            {
                sb.AppendLine("Error: No root node found.");
                return sb.ToString();
            }

            while (queue.Count > 0)
            {
                var (currentNode, currentLabel, currentDepth) = queue.Dequeue();
                nodeLabels[currentNode] = currentLabel;

                if (currentDepth >= maxItems) continue;

                var children = allNodes.Where(n => n.Parent == currentNode)
                                      .OrderByDescending(n => n.Assignment.LastOrDefault()).ToList();
                for (int i = 0; i < children.Count && i < 2; i++)
                {
                    string childLabel = currentLabel == "" ? $"{i + 1}" : $"{currentLabel}.{i + 1}";
                    queue.Enqueue((children[i], childLabel, currentDepth + 1));
                }
            }

            // Fallback for any unlabeled nodes
            foreach (var node in allNodes)
            {
                if (!nodeLabels.ContainsKey(node))
                {
                    nodeLabels[node] = "Unlabeled";
                }
            }

            // Display all nodes with Excel-like format
            foreach (var node in allNodes.OrderBy(n => nodeLabels[n] == "" ? "0" : nodeLabels[n]))
            {
                string label = nodeLabels[node];
                string subproblemHeader = label == "" ? "Sub-problem" : $"Sub-problem {label}";
                int fixedIndex = node.Level >= 0 ? node.Level : -1;
                if (fixedIndex >= 0 && node.Assignment.Count > fixedIndex)
                {
                    subproblemHeader += $" (x{items[fixedIndex].Index + 1} = {node.Assignment[fixedIndex]})";
                }
                sb.AppendLine(subproblemHeader);

                if (node.Status == "Infeasible")
                {
                    sb.AppendLine("Infeasible");
                    sb.AppendLine();
                    continue;
                }

                // Prepare table data
                double zBefore = node.Parent != null ? node.Parent.RemainingCapacity : capacity;
                double zAfter = node.RemainingCapacity;
                double weightChange = fixedIndex >= 0 && node.Parent != null ? items[fixedIndex].Weight * node.Assignment[fixedIndex] : 0;

                // Header
                string header = "Z\tvalue before\tvalue after\tvalue of var";
                string separator = new string('-', 44);
                string zLineExtra = "";
                string varLineExtra = "";
                if (label == "")
                {
                    header += "\tVar\tConstraint number";
                    separator = new string('-', 60);
                    zLineExtra = "\t\t";
                }

                sb.AppendLine(header);
                sb.AppendLine(separator);

                // Z row
                sb.AppendLine($"Z\t{zBefore,11:0.###}\t{zAfter,11:0.###}\t{weightChange,11:0.###}{zLineExtra}");

                // Variable rows, using original indices
                var orderedItems = items.OrderBy(it => it.Index).ToList();
                for (int i = 0; i < maxItems; i++)
                {
                    int origIndex = orderedItems[i].Index;
                    int sortedIndex = items.FindIndex(it => it.Index == origIndex);
                    string varPrefix = (sortedIndex == fixedIndex && node.Level >= 0) ? "*" : " ";
                    int assignment = sortedIndex < node.Assignment.Count ? node.Assignment[sortedIndex] : 0;
                    double varValue;
                    if (node.LpResult != null && sortedIndex > node.Level)
                    {
                        varValue = node.LpResult.PrimalVariables[sortedIndex - (node.Level + 1)];
                    }
                    else
                    {
                        varValue = assignment;
                    }
                    double varWeightBefore = zBefore;
                    double varWeightAfter = zAfter + (sortedIndex == fixedIndex ? orderedItems[i].Weight * assignment : 0);
                    string line = $"{varPrefix}x{origIndex + 1}\t{varWeightBefore,11:0.###}\t{varWeightAfter,11:0.###}\t{varValue,11:0.###}";
                    if (label == "")
                    {
                        line += $"\tx{origIndex + 1}\t{orderedItems[i].Weight.ToString("0.###")}";
                    }
                    sb.AppendLine(line);
                }
                sb.AppendLine(separator);

                if (node.CandidateLetter.HasValue)
                {
                    sb.AppendLine($"Candidate {node.CandidateLetter} = {bestValue.ToString("0.###")}");
                }
                if (node.Status == "Infeasible")
                {
                    sb.AppendLine("Infeasible");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}