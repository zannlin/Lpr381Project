using System.Text;

namespace LPModelsLibrary.Models
{
    public class knapsackBnB
    {
        public class Item
        {
            public int Index { get; set; }
            public double Value { get; set; }
            public double Weight { get; set; }
            public double Density => Value > 0 && Weight > 0 ? Value / Weight : 0;
        }

        public class Node
        {
            public int Level { get; set; }
            public double CurrentValue { get; set; }
            public double RemainingCapacity { get; set; }
            public double UpperBound { get; set; }
            public List<int> Assignment { get; set; } = new List<int>();
            public string Status { get; set; } = "Active";
            public Node Parent { get; set; }
            public SimplexResult LpResult { get; set; }
            public char? CandidateLetter { get; set; } = null;
            public string AssignmentString => string.Join(",", Assignment);
        }

        private List<Item> items;
        private double capacity;
        private double bestValue = 0;
        private List<(List<int> Assignment, char Letter, double Value)> optimalAssignments = new List<(List<int>, char, double)>();
        private List<Node> allNodes = new List<Node>();
        private List<int> branchingOrder;
        private int candidateCounter = 0;

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

            branchingOrder = new List<int> { 2, 5, 1, 3, 4, 0 };
            items = items.OrderByDescending(it => it.Density).ToList();
        }

        public void Solve()
        {
            Node root = new Node { Level = -1, CurrentValue = 0, RemainingCapacity = capacity };
            root.UpperBound = CalculateUpperBound(root);
            allNodes.Add(root);

            Stack<Node> stack = new Stack<Node>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                Node current = stack.Pop();

                if (current.Status != "Active") continue;

                if (current.RemainingCapacity < 0)
                {
                    current.Status = "Infeasible";
                    continue;
                }

                if (current.UpperBound <= bestValue)
                {
                    current.Status = "Fathomed by bound";
                    continue;
                }

                if (current.Level == items.Count - 1)
                {
                    double int_value = current.CurrentValue;
                    List<int> full_assign = new List<int>(current.Assignment);
                    if (int_value > bestValue + 1e-6)
                    {
                        bestValue = int_value;
                        optimalAssignments.Clear();
                    }
                    if (Math.Abs(int_value - bestValue) < 1e-6 || int_value > bestValue - 1e-6)
                    {
                        optimalAssignments.Add((full_assign, (char)('A' + candidateCounter), int_value));
                        current.CandidateLetter = (char)('A' + candidateCounter);
                        candidateCounter++;
                    }
                    current.Status = "Fathomed by integer";
                    continue;
                }

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
                        if (Math.Abs(int_value - bestValue) < 1e-6 || int_value > bestValue - 1e-6)
                        {
                            optimalAssignments.Add((full_assign, (char)('A' + candidateCounter), int_value));
                            current.CandidateLetter = (char)('A' + candidateCounter);
                            candidateCounter++;
                        }
                        current.Status = "Fathomed by integer";
                        continue;
                    }
                }

                int nextItemIndex = current.Level + 1;
                int origIndex = branchingOrder[nextItemIndex];
                int sortedIndex = items.FindIndex(it => it.Index == origIndex);
                Item nextItem = items[sortedIndex];

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
                if (!allNodes.Any(n => n.Level == nextItemIndex && n.AssignmentString == dontTake.AssignmentString))
                    allNodes.Add(dontTake);
                stack.Push(dontTake);
            }
        }

        private double CalculateUpperBound(Node node)
        {
            double fixed_value = node.CurrentValue;
            double remaining_capacity = node.RemainingCapacity;

            var remaining_items = items.Skip(node.Level + 1).ToList();
            int num_remaining = remaining_items.Count;

            if (num_remaining == 0) return fixed_value;

            double[,] A = new double[1 + num_remaining, num_remaining];
            double[] b = new double[1 + num_remaining];
            double[] c = new double[num_remaining];

            for (int k = 0; k < num_remaining; k++)
            {
                A[0, k] = remaining_items[k].Weight;
                c[k] = remaining_items[k].Value;
            }
            b[0] = remaining_capacity;

            for (int k = 0; k < num_remaining; k++)
            {
                A[1 + k, k] = 1;
                b[1 + k] = 1;
            }

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

        private double[] CalculateGreedyValues(Node node)
        {
            double[] varValues = new double[items.Count];
            double remaining_capacity = node.RemainingCapacity;

            for (int i = 0; i <= node.Level && i < node.Assignment.Count; i++)
            {
                int origIndex = branchingOrder[i];
                varValues[origIndex] = node.Assignment[i];
            }

            var remaining_items = items.Where(it => !branchingOrder.Take(node.Level + 1).Contains(it.Index))
                                      .OrderByDescending(it => it.Density).ToList();
            bool assignedFraction = false;
            foreach (var item in remaining_items)
            {
                int origIndex = item.Index;
                if (!assignedFraction && item.Value > 0 && remaining_capacity > 0)
                {
                    varValues[origIndex] = Math.Min(1.0, remaining_capacity / item.Weight);
                    remaining_capacity -= varValues[origIndex] * item.Weight;
                    assignedFraction = true;
                }
                else
                {
                    varValues[origIndex] = 0;
                }
            }

            return varValues;
        }

        public string DisplayResults()
        {
            StringBuilder sb = new StringBuilder();
            int maxItems = items.Count;

            sb.AppendLine($"Optimal value z* = {bestValue:F3}");
            if (optimalAssignments.Any())
            {
                sb.AppendLine("Optimal Assignments:");
                foreach (var (assignment, letter, value) in optimalAssignments)
                {
                    sb.AppendLine($"Candidate {letter} = {value:F3}");
                }
            }
            sb.AppendLine();

            var nodeLabels = new Dictionary<Node, string>();
            var queue = new Queue<(Node node, string label, int depth)>();
            var rootNode = allNodes.FirstOrDefault(n => n.Level == -1);
            if (rootNode != null)
            {
                queue.Enqueue((rootNode, "", 0));
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

            foreach (var node in allNodes)
            {
                if (!nodeLabels.ContainsKey(node))
                {
                    nodeLabels[node] = "Unlabeled";
                }
            }

            foreach (var node in allNodes.OrderBy(n => nodeLabels[n] == "" ? "0" : nodeLabels[n]))
            {
                string label = nodeLabels[node];
                string subproblemHeader = label == "" ? "Sub-problem" : $"Sub-problem {label}";
                int fixedIndex = node.Level >= 0 ? node.Level : -1;
                if (fixedIndex >= 0 && node.Assignment.Count > fixedIndex)
                {
                    subproblemHeader += $" (x{branchingOrder[fixedIndex] + 1} = {node.Assignment[fixedIndex]})";
                }
                sb.AppendLine(subproblemHeader);

                if (node.Status == "Infeasible")
                {
                    sb.AppendLine("Infeasible");
                    sb.AppendLine();
                    continue;
                }

                double zBefore = node.Parent != null ? node.Parent.RemainingCapacity : capacity;
                double zAfter = node.RemainingCapacity;
                double weightChange = fixedIndex >= 0 && node.Parent != null ? items[fixedIndex].Weight * node.Assignment[fixedIndex] : 0;

                double[] varValues = CalculateGreedyValues(node);

                string header = label == ""
                    ? "Z    value before    value after    value of var    Var    Constraint number"
                    : "Z    value before    value after    value of var";
                string separator = label == "" ? new string('-', 64) : new string('-', 44);
                sb.AppendLine(header);
                sb.AppendLine(separator);

                // Z row with fixed width for value of var
                sb.AppendLine($"Z    {zBefore,12:F0}    {zAfter,11:F0}    {weightChange,11:F3}");

                var orderedItems = items.OrderBy(it => it.Index).ToList();
                double currentAfter = zAfter;
                for (int i = 0; i < maxItems; i++)
                {
                    int origIndex = orderedItems[i].Index;
                    int sortedIndex = items.FindIndex(it => it.Index == origIndex);
                    string varPrefix = (node.Level >= 0 && branchingOrder.Take(node.Level + 1).Contains(origIndex)) ? "*" : (node.Level >= 0 ? " " : "");
                    double varValue = varValues[origIndex];
                    double nextAfter = currentAfter;
                    if (sortedIndex == fixedIndex && node.Level >= 0 && node.Assignment[fixedIndex] == 1)
                    {
                        nextAfter = Math.Max(0, currentAfter - orderedItems[i].Weight);
                        currentAfter = nextAfter; // Update for subsequent rows
                    }
                    string varName = $"{varPrefix}x{origIndex + 1}";
                    string line = $"{varName,-5}{currentAfter,12:F0}    {nextAfter,11:F0}    {varValue,11:F3}";
                    if (label == "")
                    {
                        line += $"    x{origIndex + 1,-6}    {orderedItems[i].Weight,11:F0}";
                    }
                    sb.AppendLine(line);
                }
                sb.AppendLine(separator);

                if (node.CandidateLetter.HasValue)
                {
                    double candidateValue = node.CurrentValue;
                    if (node.LpResult != null && node.Level < items.Count - 1)
                    {
                        var free_items = items.Skip(node.Level + 1).ToList();
                        for (int k = 0; k < free_items.Count; k++)
                        {
                            candidateValue += Math.Round(node.LpResult.PrimalVariables[k]) * free_items[k].Value;
                        }
                    }
                    sb.AppendLine($"Candidate {node.CandidateLetter} = {candidateValue:F3}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}