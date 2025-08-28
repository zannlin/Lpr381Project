using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LPModelsLibrary;
using LPModelsLibrary.Models;

namespace MainForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void DisplayResults(SimplexResult result)

        {
            // 1. Canonical form
            richTextBoxCanonical.Clear();
            richTextBoxCanonical.AppendText("Objective Function:\n");
            richTextBoxCanonical.AppendText(string.Join(" ", result.PrimalVariables.Select((v, i) => $"x{i + 1}={v:0.###}")) + "\n");

            // 2. Tableau iterations
            richTextBoxTableau.Clear();
            foreach (var tableau in result.Tableaus)
            {
                richTextBoxTableau.AppendText(tableau.ToString());
                richTextBoxTableau.AppendText("\n\n");
            }

            // 3. Optimal solution
            richTextBoxOptimal.Clear();
            richTextBoxOptimal.AppendText($"Optimal Value: {result.OptimalValue:0.###}\n");
            for (int i = 0; i < result.PrimalVariables.Length; i++)
                richTextBoxOptimal.AppendText($"x{i + 1} = {result.PrimalVariables[i]:0.###}\n");
        }

        private void DisplayKnapsackResults(knapsackBnB solver, double[] values, double[] weights, double capacity)
        {
            // 1) Canonical: summarize the input
            richTextBoxCanonical.Clear();
            richTextBoxCanonical.AppendText("Knapsack Problem (parsed from input):\n");
            richTextBoxCanonical.AppendText($"Values:   {string.Join(", ", values.Select(v => v.ToString("0.###", CultureInfo.InvariantCulture)))}\n");
            richTextBoxCanonical.AppendText($"Weights:  {string.Join(", ", weights.Select(w => w.ToString("0.###", CultureInfo.InvariantCulture)))}\n");
            richTextBoxCanonical.AppendText($"Capacity: {capacity.ToString("0.###", CultureInfo.InvariantCulture)}\n");

            // 2) Tableau (here: full branch-and-bound textual log from knapsackBnD.cs -> Zander)
            string full = solver.DisplayResults() ?? string.Empty;
            richTextBoxTableau.Clear();
            richTextBoxTableau.AppendText(full);

            // 3) Optimal: extract the top summary block (until first "Sub-problem" or "Error:")
            var lines = full
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            int splitIdx = lines.FindIndex(l => l.StartsWith("Sub-problem", StringComparison.OrdinalIgnoreCase)
                                              || l.StartsWith("Error:", StringComparison.OrdinalIgnoreCase)
                                              || l.StartsWith("Z    ", StringComparison.OrdinalIgnoreCase));

            string summary;
            if (splitIdx == -1)
            {
                // if nothing found — show entire output
                summary = string.Join(Environment.NewLine, lines);
            }
            else
            {
                summary = string.Join(Environment.NewLine, lines.Take(splitIdx));
            }

            richTextBoxOptimal.Clear();
            richTextBoxOptimal.AppendText(summary);
        }

        private void DisplayBranchAndBoundResults(BranchAndBoundResult result)
        {
            // 1. Canonical / Objective
            richTextBoxCanonical.Clear();
            richTextBoxCanonical.AppendText("Branch and Bound Result\n");
            if (result.OptimalSolution != null)
            {
                richTextBoxCanonical.AppendText("Optimal Solution:\n");
                richTextBoxCanonical.AppendText(
                    string.Join(" ", result.OptimalSolution.Select((v, i) => $"x{i + 1}={v:0.###}"))
                );
                richTextBoxCanonical.AppendText($"\nOptimal Value: {result.OptimalValue:0.###}\n");
            }
            else
            {
                richTextBoxCanonical.AppendText(result.Message + "\n");
            }

            // 2. Tree of nodes
            richTextBoxTableau.Clear();
            foreach (var node in result.Nodes)
            {
                richTextBoxTableau.AppendText($"Node {node.NodeId} (Parent: {node.ParentId?.ToString() ?? "None"})\n");
                richTextBoxTableau.AppendText($"Status: {node.Status}\n");
                richTextBoxTableau.AppendText($"Bounds: {string.Join(", ", node.LowerBounds.Select((lb, j) =>
                    $"x{j + 1} ∈ [{lb:0.###}, {(node.UpperBounds[j] >= 1e308 ? "∞" : node.UpperBounds[j].ToString("0.###"))}]"))}\n");

                if (node.Result != null)
                {
                    richTextBoxTableau.AppendText($"LP z = {node.Result.OptimalValue:0.###}\n");
                    richTextBoxTableau.AppendText(
                        $"Solution: {string.Join(", ", node.Result.PrimalVariables.Select((v, i) => $"x{i + 1}={v:0.###}"))}\n"
                    );

                    foreach (var tableau in node.Result.Tableaus)
                        richTextBoxTableau.AppendText(tableau.ToString() + "\n");
                }
                richTextBoxTableau.AppendText("\n");
            }

            // 3. Final message
            richTextBoxOptimal.Clear();
            richTextBoxOptimal.AppendText(result.Message + "\n");
            if (result.OptimalSolution != null)
            {
                richTextBoxOptimal.AppendText($"Optimal Value: {result.OptimalValue:0.###}\n");
                for (int i = 0; i < result.OptimalSolution.Length; i++)
                    richTextBoxOptimal.AppendText($"x{i + 1} = {result.OptimalSolution[i]:0.###}\n");
            }
        }

        private void revisedSimplexToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void primalSimplexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Get content from the Input textbox
                string[] inputLines = textBoxInput.Lines;

                // Parse LP from file content
                var model = new LinearModel(inputLines);

                // Solve with simplex
                var result = PrimalSimplex.Solve(
                    model.Constraints,
                    model.RightHandSide,
                    model.ObjectiveFuntion
                );

                // Display results
                DisplayResults(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void loadInputFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files|*.txt|All Files|*.*";
                ofd.Title = "Load Input Model";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string fileContent = File.ReadAllText(ofd.FileName);
                        textBoxInput.Text = fileContent;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading file: " + ex.Message);
                    }
                }
            }
        }

        private void exportOutputFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text Files|*.txt|All Files|*.*";
                sfd.Title = "Export Results";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {

                        File.WriteAllText(sfd.FileName, richTextBoxOptimal.Text);
                        MessageBox.Show("File saved successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving file: " + ex.Message);
                    }
                }
            }
        }

        private void branchAndBoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Get LP model from input textbox
                string[] inputLines = textBoxInput.Lines;
                var model = new LinearModel(inputLines);

                // Infer constraint types 
                char[] constraintTypes = new char[model.Constraints.GetLength(0)];
                for (int i = 0; i < constraintTypes.Length; i++)
                    constraintTypes[i] = 'L'; // assume <=

                // Infer integer/binary variables from the last line of the file
                bool[] isInteger = new bool[model.ObjectiveFuntion.Length];
                string lastLine = inputLines[inputLines.Length - 1].ToLower();

                if (lastLine.Contains("bin") || lastLine.Contains("int"))
                {
                    string[] tokens = lastLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < tokens.Length && i < isInteger.Length; i++)
                    {
                        if (tokens[i] == "bin" || tokens[i] == "int")
                            isInteger[i] = true;
                    }
                }

                // Solve with Branch and Bound
                var result = BranchAndBound.Solve(
                    model.Constraints,
                    model.RightHandSide,
                    model.ObjectiveFuntion,
                    constraintTypes,
                    isInteger
                );

                DisplayBranchAndBoundResults(result);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void cuttingPlaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Get content from the Input textbox
                string[] inputLines = textBoxInput.Lines;

                // 2. Parse LP
                var model = new LinearModel(inputLines);

                // 3. Solve LP relaxation using primal simplex
                var lpResult = PrimalSimplex.Solve(
                    model.Constraints,
                    model.RightHandSide,
                    model.ObjectiveFuntion
                );

                // 4. Pass the result into CuttingPlaneSimplex
                var cuttingPlaneSolver = new CuttingPlaneSimplex(lpResult);
                cuttingPlaneSolver.solveCuttingPlane();

                // 5. Display final results
                DisplayResults(cuttingPlaneSolver.cuttingPlaneResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error (Cutting Plane): " + ex.Message);
            }
        }

        private void knapsackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // read non-empty lines
                string[] inputLines = textBoxInput.Lines
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .ToArray();

                if (inputLines.Length < 2)
                    throw new InvalidOperationException("Input must contain at least the objective line and the weights line.");

                // Parse values from first line, skip "max" or "min"
                var valueTokens = inputLines[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (valueTokens.Count == 0)
                    throw new InvalidOperationException("Objective line is empty.");

                if (valueTokens[0].Equals("max", StringComparison.OrdinalIgnoreCase) ||
                    valueTokens[0].Equals("min", StringComparison.OrdinalIgnoreCase))
                    valueTokens.RemoveAt(0);

                double[] values = valueTokens.Select(t => double.Parse(t.Replace("+", ""), CultureInfo.InvariantCulture)).ToArray();

                // Parse weights line and capacity (handles "<=40" or "<=" "40")
                var weightParts = inputLines[1].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (weightParts.Count == 0)
                    throw new InvalidOperationException("Weights line is empty.");

                int capIndex = weightParts.FindIndex(p => p.Contains("<=") || p.Equals("<="));
                if (capIndex == -1)
                    throw new InvalidOperationException("Weights line must contain '<=' followed by capacity (e.g. ... <=40).");

                // If token contains ex. "<=40" split it
                if (weightParts[capIndex].Contains("<=") && weightParts[capIndex] != "<=")
                {
                    var token = weightParts[capIndex];
                    // replace the token with two tokens: the part before (if any is weight) and capacity part
                    // but simpler: separate the capacity token
                    string capToken = token.Replace("<=", "");
                    // weights are everything before capIndex
                    var weightTokens = weightParts.Take(capIndex).ToArray();
                    double[] weights = weightTokens.Select(t => double.Parse(t.Replace("+", ""), CultureInfo.InvariantCulture)).ToArray();
                    double capacity = double.Parse(capToken, CultureInfo.InvariantCulture);

                    // validate lengths
                    if (weights.Length != values.Length)
                        throw new InvalidOperationException($"Number of weights ({weights.Length}) does not match number of values ({values.Length}).");

                    // run solver
                    var solver = new knapsackBnB(values, weights, capacity);
                    solver.Solve();

                    // display
                    DisplayKnapsackResults(solver, values, weights, capacity);
                    return;
                }
                else
                {
                    // cap is either next token or the token after "<="
                    int capTokenIndex = (weightParts[capIndex] == "<=") ? capIndex + 1 : capIndex;
                    if (capTokenIndex >= weightParts.Count)
                        throw new InvalidOperationException("Capacity token missing after '<='.");

                    double capacity = double.Parse(weightParts[capTokenIndex].Replace("+", ""), CultureInfo.InvariantCulture);
                    var weightTokens = weightParts.Take(capIndex).ToArray();
                    double[] weights = weightTokens.Select(t => double.Parse(t.Replace("+", ""), CultureInfo.InvariantCulture)).ToArray();

                    if (weights.Length != values.Length)
                        throw new InvalidOperationException($"Number of weights ({weights.Length}) does not match number of values ({values.Length}).");

                    var solver = new knapsackBnB(values, weights, capacity);
                    solver.Solve();

                    DisplayKnapsackResults(solver, values, weights, capacity);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Branch & Bound error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
