using LPModelsLibrary;
using LPModelsLibrary.Models;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace MainForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private TableauTemplate optimalTableau;
        private TableauTemplate originalTableau;

        private void StoreTableaus(SimplexResult result)
        {
            optimalTableau = result.Tableaus.Last();
            originalTableau = result.Tableaus.First();
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
            richTextBoxCanonical.AppendText("Knapsack Problem:\n");
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

        private void DisplayRevisedPrimalSimplexOutput(SimplexResult result)
        {
            // Clear existing content
            richTextBoxCanonical.Clear();
            richTextBoxTableau.Clear();
            richTextBoxOptimal.Clear();

            // 1. Canonical Form (Objective Function and Initial Insight)
            richTextBoxCanonical.AppendText("Revised Primal Simplex - Canonical Form:\n");
            if (result == null)
            {
                richTextBoxCanonical.AppendText("No result data available.\n");
                return;
            }

            if (result.IsInfeasible || result.IsUnbounded)
            {
                richTextBoxCanonical.AppendText($"Status: {result.Message ?? "Unknown status"}\n");
            }
            else if (result.PrimalVariables != null)
            {
                richTextBoxCanonical.AppendText("Initial Variables: " + string.Join(" ", result.PrimalVariables.Select((v, i) => $"x{i + 1}={v:0.###}")) + "\n");
                if (result.SlackExcessVariables != null)
                {
                    richTextBoxCanonical.AppendText("Slack Variables: " + string.Join(" ", result.SlackExcessVariables.Select((v, i) => $"s{i + 1}={v:0.###}")) + "\n");
                }
            }

            // 2. Tableau Iterations
            richTextBoxTableau.AppendText("Revised Primal Simplex - Tableau Iterations:\n");
            if (result.Tableaus != null && result.Tableaus.Count > 0)
            {
                foreach (var tableau in result.Tableaus)
                {
                    richTextBoxTableau.AppendText(tableau.ToString());
                    richTextBoxTableau.AppendText("\n");
                }
            }
            else
            {
                richTextBoxTableau.AppendText("No tableau data available.\n");
            }

            // 3. Optimal Solution
            richTextBoxOptimal.AppendText("Revised Primal Simplex - Optimal Solution:\n");
            if (result.IsInfeasible || result.IsUnbounded)
            {
                richTextBoxOptimal.AppendText($"Status: {result.Message ?? "Unknown status"}\n");
            }
            else if (result.PrimalVariables != null && !double.IsNaN(result.OptimalValue))
            {
                richTextBoxOptimal.AppendText($"Optimal Value: {result.OptimalValue:0.###}\n");
                for (int i = 0; i < result.PrimalVariables.Length; i++)
                    richTextBoxOptimal.AppendText($"x{i + 1} = {result.PrimalVariables[i]:0.###}\n");
                if (result.SlackExcessVariables != null)
                {
                    for (int i = 0; i < result.SlackExcessVariables.Length; i++)
                        richTextBoxOptimal.AppendText($"s{i + 1} = {result.SlackExcessVariables[i]:0.###}\n");
                }
            }
            else
            {
                richTextBoxOptimal.AppendText("No optimal solution data available.\n");
            }

            // Store tableaus for sensitivity analysis
            if (result.Tableaus != null && result.Tableaus.Count > 0)
            {
                StoreTableaus(result);
            }
        }

        private void DisplayBranchAndBoundResults(BranchAndBoundResult result)
        {
            // 1. Canonical / Objective
            richTextBoxCanonical.Clear();
            richTextBoxCanonical.AppendText("Branch and Bound Objective Function\n");
            if (result.OptimalSolution != null)
            {
                richTextBoxCanonical.AppendText("Objective Function:\n");
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

        private void ConfigureSensitivityControls(string methodName)
        {
            if (optimalTableau == null || originalTableau == null)
            {
                sensitivityPanel.Controls.Clear();
                sensitivityPanel.Controls.Add(new Label { Text = "Run an algorithm first to generate tableaus.", AutoSize = true, Margin = new Padding(5) });
                return;
            }

            sensitivityPanel.Controls.Clear();
            MathematicalSensitivity sensitivity = new MathematicalSensitivity(optimalTableau, originalTableau);

            switch (methodName)
            {
                case "Range of Non-Basic Variable":
                    Label nbLabel = new Label { Text = "Select Non-Basic Variable Index:", AutoSize = true, Margin = new Padding(5) };
                    ComboBox nbCombo = new ComboBox
                    {
                        Name = "cbNonBasic",
                        Margin = new Padding(5),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    for (int i = 0; i < originalTableau.ColHeaders.Length - 1; i++) // Exclude RHS
                        nbCombo.Items.Add(i);
                    Button nbButton = new Button { Text = "Calculate Range", Margin = new Padding(5) };
                    nbButton.Click += (s, e) =>
                    {
                        if (nbCombo.SelectedItem != null)
                        {

                            int index = (int)nbCombo.SelectedItem;
                            string range = sensitivity.Range_Of_NonBasic_Variable(index);
                            sensitivityPanel.Controls.Add(new Label { Text = range, AutoSize = true, Margin = new Padding(5) });
                        }
                    };
                    sensitivityPanel.Controls.Add(nbLabel);
                    sensitivityPanel.Controls.Add(nbCombo);
                    sensitivityPanel.Controls.Add(nbButton);
                    break;

                case "Change Non-Basic Variable":
                    Label nbChangeLabel = new Label { Text = "Select Non-Basic Variable Index:", AutoSize = true, Margin = new Padding(5) };
                    ComboBox nbChangeCombo = new ComboBox
                    {
                        Name = "cbNonBasicChange",
                        Margin = new Padding(5),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    for (int i = 0; i < originalTableau.ColHeaders.Length - 1; i++)
                        nbChangeCombo.Items.Add(i);
                    Label nbNewValueLabel = new Label { Text = "New Coefficient:", AutoSize = true, Margin = new Padding(5) };
                    TextBox nbNewValue = new TextBox { Name = "txtNewValue", Margin = new Padding(5), Width = 100 };
                    Button nbChangeButton = new Button { Text = "Apply Change", Margin = new Padding(5) };
                    nbChangeButton.Click += (s, e) =>
                    {
                        if (double.TryParse(nbNewValue.Text, out double newCoef) && nbChangeCombo.SelectedItem != null)
                        {
                            int index = (int)nbChangeCombo.SelectedItem;
                            string result = sensitivity.change_nonBasic_Variable_Coefficient(index, newCoef);
                            sensitivityPanel.Controls.Add(new Label { Text = result, AutoSize = true, Margin = new Padding(5) });
                        }
                    };
                    sensitivityPanel.Controls.Add(nbChangeLabel);
                    sensitivityPanel.Controls.Add(nbChangeCombo);
                    sensitivityPanel.Controls.Add(nbNewValueLabel);
                    sensitivityPanel.Controls.Add(nbNewValue);
                    sensitivityPanel.Controls.Add(nbChangeButton);
                    break;

                case "Range of Basic Variable":
                    Label bvLabel = new Label { Text = "Select Basic Variable Index:", AutoSize = true, Margin = new Padding(5) };
                    ComboBox bvCombo = new ComboBox
                    {
                        Name = "cbBasic",
                        Margin = new Padding(5),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    var basicIndices = sensitivity.getBasicVarColumnIndex();
                    foreach (var index in basicIndices)
                        bvCombo.Items.Add(index);
                    Button bvButton = new Button { Text = "Calculate Range", Margin = new Padding(5) };
                    bvButton.Click += (s, e) =>
                    {
                        if (bvCombo.SelectedItem != null)
                        {
                            int index = (int)bvCombo.SelectedItem;
                            var (upper, lower) = sensitivity.range_Of_Basic_Variable(index);
                            sensitivityPanel.Controls.Add(new Label { Text = $"Upper Range: {upper}\nLower Range: {lower}", AutoSize = true, Margin = new Padding(5) });
                        }
                    };
                    sensitivityPanel.Controls.Add(bvLabel);
                    sensitivityPanel.Controls.Add(bvCombo);
                    sensitivityPanel.Controls.Add(bvButton);
                    break;

                case "Change Basic Variable":
                    Label bvChangeLabel = new Label { Text = "Select Basic Variable Index:", AutoSize = true, Margin = new Padding(5) };
                    ComboBox bvChangeCombo = new ComboBox
                    {
                        Name = "cbBasicChange",
                        Margin = new Padding(5),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    // Compute basicIndices within this case
                    var basicIndicesBV = sensitivity.getBasicVarColumnIndex();
                    foreach (var index in basicIndicesBV)
                        bvChangeCombo.Items.Add(index);
                    Label bvNewValueLabel = new Label { Text = "New Coefficient:", AutoSize = true, Margin = new Padding(5) };
                    TextBox bvNewValue = new TextBox { Name = "txtBVNewValue", Margin = new Padding(5), Width = 100 };
                    Button bvChangeButton = new Button { Text = "Apply Change", Margin = new Padding(5) };
                    bvChangeButton.Click += (s, e) =>
                    {
                        if (double.TryParse(bvNewValue.Text, out double newCoef) && bvChangeCombo.SelectedItem != null)
                        {
                            int index = (int)bvChangeCombo.SelectedItem;
                            double[] newZRow = sensitivity.change_Basic_Variable_Coefficient(index, newCoef);
                            sensitivityPanel.Controls.Add(new Label { Text = "New Z-Row: " + string.Join(", ", newZRow.Select(x => x.ToString("F3"))), AutoSize = true, Margin = new Padding(5) });
                        }
                    };
                    sensitivityPanel.Controls.Add(bvChangeLabel);
                    sensitivityPanel.Controls.Add(bvChangeCombo);
                    sensitivityPanel.Controls.Add(bvNewValueLabel);
                    sensitivityPanel.Controls.Add(bvNewValue);
                    sensitivityPanel.Controls.Add(bvChangeButton);
                    break;

                case "Range of Constraint RHS":
                    Label rhsLabel = new Label { Text = "Select Constraint Row Index:", AutoSize = true, Margin = new Padding(5) };
                    ComboBox rhsCombo = new ComboBox
                    {
                        Name = "cbRHS",
                        Margin = new Padding(5),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    for (int i = 1; i < optimalTableau.Tableau.GetLength(0); i++) // Exclude objective row
                        rhsCombo.Items.Add(i);
                    Button rhsButton = new Button { Text = "Calculate Range", Margin = new Padding(5) };
                    rhsButton.Click += (s, e) =>
                    {
                        if (rhsCombo.SelectedItem != null)
                        {
                            int index = (int)rhsCombo.SelectedItem;
                            var (lower, upper) = sensitivity.range_RightHandSide(index);
                            sensitivityPanel.Controls.Add(new Label { Text = $"Lower Range: {lower}\nUpper Range: {upper}", AutoSize = true, Margin = new Padding(5) });
                        }
                    };
                    sensitivityPanel.Controls.Add(rhsLabel);
                    sensitivityPanel.Controls.Add(rhsCombo);
                    sensitivityPanel.Controls.Add(rhsButton);
                    break;

                case "Change Constraint RHS":
                    Label rhsChangeLabel = new Label { Text = "Select Constraint Row Index:", AutoSize = true, Margin = new Padding(5) };
                    ComboBox rhsChangeCombo = new ComboBox
                    {
                        Name = "cbRHSChange",
                        Margin = new Padding(5),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    for (int i = 1; i < optimalTableau.Tableau.GetLength(0); i++)
                        rhsChangeCombo.Items.Add(i);
                    Label rhsNewValueLabel = new Label { Text = "New RHS Value:", AutoSize = true, Margin = new Padding(5) };
                    TextBox rhsNewValue = new TextBox { Name = "txtRHSNewValue", Margin = new Padding(5), Width = 100 };
                    Button rhsChangeButton = new Button { Text = "Apply Change", Margin = new Padding(5) };
                    rhsChangeButton.Click += (s, e) =>
                    {
                        if (double.TryParse(rhsNewValue.Text, out double newValue) && rhsChangeCombo.SelectedItem != null)
                        {
                            int index = (int)rhsChangeCombo.SelectedItem;
                            var (bOptimal, optValue) = sensitivity.change_RightHandSide(index, newValue);
                            sensitivityPanel.Controls.Add(new Label { Text = $"New Optimal b: {string.Join(", ", bOptimal.Select(x => x.ToString("F3")))}\nNew Optimal Value: {optValue:F3}", AutoSize = true, Margin = new Padding(5) });
                        }
                    };
                    sensitivityPanel.Controls.Add(rhsChangeLabel);
                    sensitivityPanel.Controls.Add(rhsChangeCombo);
                    sensitivityPanel.Controls.Add(rhsNewValueLabel);
                    sensitivityPanel.Controls.Add(rhsNewValue);
                    sensitivityPanel.Controls.Add(rhsChangeButton);
                    break;

                case "Add New Activity":
                    Label activityLabel = new Label { Text = "Enter New Activity (e.g., 20 0 0 1 for 4 rows):", AutoSize = true, Margin = new Padding(5) };
                    TextBox activityInput = new TextBox { Name = "txtActivityInput", Margin = new Padding(5), Width = 300, Multiline = true, Height = 50 };
                    Label activityNameLabel = new Label { Text = "New Activity Name (e.g., x5):", AutoSize = true, Margin = new Padding(5) };
                    TextBox activityName = new TextBox { Name = "txtActivityName", Margin = new Padding(5), Width = 100 };
                    Button activityButton = new Button { Text = "Add Activity", Margin = new Padding(5) };
                    activityButton.Click += (s, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(activityInput.Text) && !string.IsNullOrWhiteSpace(activityName.Text))
                        {
                            string[] parts = activityInput.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(p => p.TrimStart('+'))
                                .ToArray();
                            int expectedRows = optimalTableau.Tableau.GetLength(0);
                            if (parts.Length != expectedRows)
                            {
                                sensitivityPanel.Controls.Add(new Label { Text = $"Invalid input length. Expected {expectedRows} values to match the tableau row count.", AutoSize = true, Margin = new Padding(5) });
                                return;
                            }

                            try
                            {
                                double[] fullColumn = parts.Select(p => double.Parse(p)).ToArray();
                                TableauTemplate newTableau = sensitivity.AddColumn(fullColumn, activityName.Text);
                                sensitivityPanel.Controls.Add(new Label { Text = "Optimal Tableau After Adding Activity: " + newTableau.ToString(), AutoSize = true, Margin = new Padding(5) });
                            }
                            catch (ArgumentException ex)
                            {
                                sensitivityPanel.Controls.Add(new Label { Text = $"Error: {ex.Message}", AutoSize = true, Margin = new Padding(5) });
                            }
                            catch (FormatException)
                            {
                                sensitivityPanel.Controls.Add(new Label { Text = "Error: Invalid number format. Use numeric values only.", AutoSize = true, Margin = new Padding(5) });
                            }
                        }
                    };
                    sensitivityPanel.Controls.Add(activityLabel);
                    sensitivityPanel.Controls.Add(activityInput);
                    sensitivityPanel.Controls.Add(activityNameLabel);
                    sensitivityPanel.Controls.Add(activityName);
                    sensitivityPanel.Controls.Add(activityButton);
                    break;

                case "Add New Constraint":
                    Label constraintLabel = new Label { Text = "Enter New Constraint (e.g., 1 0 0 0 0 1 32):", AutoSize = true, Margin = new Padding(5) };
                    TextBox constraintInput = new TextBox { Name = "txtConstraintInput", Margin = new Padding(5), Width = 300, Multiline = true, Height = 50 };
                    Label constraintNameLabel = new Label { Text = "New Constraint Name (e.g., 4):", AutoSize = true, Margin = new Padding(5) };
                    TextBox constraintName = new TextBox { Name = "txtConstraintName", Margin = new Padding(5), Width = 100 };
                    Button constraintButton = new Button { Text = "Add Constraint", Margin = new Padding(5) };
                    constraintButton.Click += (s, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(constraintInput.Text) && !string.IsNullOrWhiteSpace(constraintName.Text))
                        {
                            string[] parts = constraintInput.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(p => p.TrimStart('+'))
                                .ToArray();
                            int numVars = optimalTableau.ColHeaders.Length - 2; // 6 variables (excluding slack and RHS)
                            if (parts.Length != numVars + 1) // 6 coeffs + 1 RHS
                                sensitivityPanel.Controls.Add(new Label { Text = $"Invalid input length. Expected {numVars + 1} values (6 coefficients + 1 RHS).", AutoSize = true, Margin = new Padding(5) });
                            else
                            {
                                double[] newConstraintRow = parts.Select(p => double.Parse(p)).ToArray();
                                TableauTemplate newTableau = sensitivity.AddRow(newConstraintRow, constraintName.Text);
                                sensitivityPanel.Controls.Add(new Label { Text = "New Tableau Updated: " + newTableau.ToString(), AutoSize = true, Margin = new Padding(5) });
                            }
                        }
                    };
                    sensitivityPanel.Controls.Add(constraintLabel);
                    sensitivityPanel.Controls.Add(constraintInput);
                    sensitivityPanel.Controls.Add(constraintNameLabel);
                    sensitivityPanel.Controls.Add(constraintName);
                    sensitivityPanel.Controls.Add(constraintButton);
                    break;

                case "Display Shadow Prices":
                    sensitivityPanel.Controls.Add(new Label { Text = sensitivity.display_shadow_Prices(), AutoSize = true, Margin = new Padding(5) });
                    break;

                case "Duality Analysis":
                    // Input data as provided
                    string[] input = textBoxInput.Lines;

                    LinearModel linearModel = new LinearModel(input);

                    double[] c = linearModel.ObjectiveFuntion;
                    double[,] A = linearModel.Constraints;
                    double[] b = linearModel.RightHandSide;

                    Duality duality = new Duality();
                    TableauTemplate canonicalDual = duality.BuildDualTableau(c, A, b);
                    TableauTemplate solvedDual = null;

                    try
                    {
                        solvedDual = duality.SolveDualFromPrimal(c, A, b);
                        sensitivityPanel.Controls.Add(new Label { Text = "Canonical Dual Tableau:\n" + canonicalDual.ToString(), AutoSize = true, Margin = new Padding(5) });
                        sensitivityPanel.Controls.Add(new Label { Text = "Solved Dual Tableau:\n" + solvedDual.ToString(), AutoSize = true, Margin = new Padding(5) });
                    }
                    catch (InvalidOperationException ex)
                    {
                        sensitivityPanel.Controls.Add(new Label { Text = $"Error: {ex.Message}\nNote: The primal LP may be infeasible due to integer constraints or tight bounds. Consider adjusting constraints.", AutoSize = true, Margin = new Padding(5) });
                        sensitivityPanel.Controls.Add(new Label { Text = "Canonical Dual Tableau:\n" + canonicalDual.ToString(), AutoSize = true, Margin = new Padding(5) });
                    }

                    break;
            }
        }



        //Don't use this event, it's broken 
        private void revisedSimplexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBoxCanonical.AppendText("Debug: Starting Revised Primal Simplex\n");
                // Get content from the Input textbox
                string[] inputLines = textBoxInput.Lines;
                richTextBoxCanonical.AppendText($"Debug: Input lines: {string.Join(", ", inputLines)}\n");

                // Parse LP from file content
                var model = new LinearModel(inputLines);
                richTextBoxCanonical.AppendText($"Debug: Model parsed - Constraints: {model.Constraints?.Length}, RHS: {model.RightHandSide?.Length}, Objective: {model.ObjectiveFuntion?.Length}\n");

                // Solve with revised primal simplex
                var result = RevisedPrimalSimplex.Solve(
                    model.Constraints,
                    model.RightHandSide,
                    model.ObjectiveFuntion // Using existing typo as-is
                );
                richTextBoxCanonical.AppendText($"Debug: Solve completed - Result: {result != null}\n");

                // Display results
                DisplayRevisedPrimalSimplexOutput(result);
            }
            catch (Exception ex)
            {
                richTextBoxCanonical.AppendText($"Debug: Exception caught: {ex.Message}\n");
                MessageBox.Show("Error: " + ex.Message);
            }
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
                StoreTableaus(result);
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

        private void rangeOfNonBasicVariablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureSensitivityControls("Range of Non-Basic Variable");
        }

        private void changeNonBasicVariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureSensitivityControls("Change Non-Basic Variable");
        }

        private void rangeOfBasicVariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureSensitivityControls("Range of Basic Variable");

        }

        private void changeBasicVariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureSensitivityControls("Change Basic Variable");
        }

        private void rangeOfConstraintRHSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureSensitivityControls("Range of Constraint RHS");
        }

        private void changeConstraintRHSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureSensitivityControls("Change Constraint RHS");
        }

        private void rangeOfVariableInNonBasicColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void changeVariableInNonBasicColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void addNewActivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureSensitivityControls("Add New Activity");
        }

        private void addNewConstraintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureSensitivityControls("Add New Constraint");
        }

        private void displayShadowPricesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureSensitivityControls("Display Shadow Prices");
        }

        private void dualityAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureSensitivityControls("Duality Analysis");
        }

        private void revisedPrimalSimplexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Get content from the Input textbox
                string[] inputLines = textBoxInput.Lines;

                // Manually parse input
                if (inputLines.Length < 2)
                    throw new Exception("Input must contain at least an objective and a constraint.");
                string objectiveLine = inputLines[0].Trim(); // e.g., "max +2 +3 +3 +5 +2 +4"
                var constraintLines = inputLines.Skip(1).Take(inputLines.Length - 2).Select(l => l.Trim()); // Constraints
                string[] binLines = inputLines.Length > 2 ? inputLines.Skip(inputLines.Length - 1).Take(1).ToArray() : new string[0]; // "int bin bin bin bin bin"

                // Parse objective (c)
                string[] objParts = objectiveLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (objParts.Length < 2 || !objParts[0].ToLower().StartsWith("max"))
                    throw new Exception("Invalid objective format. Expected 'max' followed by coefficients.");
                double[] c = objParts.Skip(1).Select(p => double.Parse(p.TrimStart('+'))).ToArray(); // [2, 3, 3, 5, 2, 4]

                // Parse constraints (A and b) without pre-adding slack
                int numConstraints = constraintLines.Count();
                int numVars = c.Length;
                double[,] A = new double[numConstraints, numVars]; // No slack column, let library handle it
                double[] b = new double[numConstraints];

                int constraintIdx = 0;
                foreach (string constraint in constraintLines)
                {
                    string[] parts = constraint.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < numVars + 1) // +1 for RHS
                        throw new Exception($"Constraint '{constraint}' has insufficient coefficients. Expected {numVars + 1} parts, got {parts.Length}.");
                    for (int i = 0; i < numVars; i++)
                    {
                        A[constraintIdx, i] = double.Parse(parts[i].TrimStart('+')); // Coefficients
                    }
                    b[constraintIdx] = double.Parse(parts[parts.Length - 1].TrimStart("<= ".ToCharArray())); // RHS
                    constraintIdx++;
                }

                // Handle binary constraints if "int bin" is present
                if (binLines.Length > 0 && binLines[0].StartsWith("int"))
                {
                    string[] binParts = binLines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int binCount = binParts.Length - 1; // Count "bin" occurrences
                    if (binCount == numVars)
                    {
                        int origConstraints = numConstraints;
                        numConstraints += numVars;

                        // Resize A manually without slack
                        double[,] newA = new double[numConstraints, numVars];
                        for (int i = 0; i < origConstraints; i++)
                            for (int j = 0; j < numVars; j++)
                                newA[i, j] = A[i, j];
                        for (int i = origConstraints; i < numConstraints; i++)
                        {
                            int varIdx = i - origConstraints;
                            if (varIdx >= 0 && varIdx < numVars)
                            {
                                newA[i, varIdx] = 1.0; // x_i <= 1
                            }
                        }
                        A = newA;

                        // Resize b
                        Array.Resize(ref b, numConstraints);
                        for (int i = origConstraints; i < numConstraints; i++)
                            b[i] = 1.0; // RHS for binary constraints
                    }
                }

                // Solve with revised primal simplex (passing A, b, c)
                var result = RevisedPrimalSimplex.Solve(A, b, c);

                // Display results
                DisplayRevisedPrimalSimplexOutput(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }

        private void textBoxInput_TextChanged(object sender, EventArgs e)
        {

        }

        private void goldenSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Clear the optimal result box
            richTextBoxOptimal.Clear();

            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtFunct.Text) || string.IsNullOrWhiteSpace(txtLow.Text) || string.IsNullOrWhiteSpace(txtUpp.Text))
            {
                richTextBoxOptimal.AppendText("Error: All fields are required.\n");
                return;
            }

            if (!double.TryParse(txtLow.Text, out double xlo) || !double.TryParse(txtUpp.Text, out double xhi))
            {
                richTextBoxOptimal.AppendText("Error: Invalid number format for bounds.\n");
                return;
            }

            if (xlo >= xhi)
            {
                richTextBoxOptimal.AppendText("Error: xlo must be less than xhi.\n");
                return;
            }

            // Add debugging information
            richTextBoxOptimal.AppendText($"Debug: Expression: {txtFunct.Text}, xlo: {xlo}, xhi: {xhi}\n");

            try
            {
                MathematicalSensitivity sensitivity = new MathematicalSensitivity(optimalTableau, originalTableau); // Assuming form-level variables
                string result = sensitivity.GoldenSectionSearch(txtFunct.Text, xlo, xhi);
                richTextBoxOptimal.AppendText($"Optimal Solution: Minimum occurs at x = {result:F6}\n");
            }
            catch (ArgumentException ex)
            {
                richTextBoxOptimal.AppendText($"Error: {ex.Message}\n");
                richTextBoxOptimal.AppendText($"Debug: Full Error Details: {ex.ToString()}\n");
            }
        }

        private void btnClearInput_Click(object sender, EventArgs e)
        {
            txtFunct.Clear();
            txtLow.Clear();
            txtUpp.Clear();
            textBoxInput.Clear();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            txtFunct.Clear();
            txtLow.Clear();
            txtUpp.Clear();
            textBoxInput.Clear();
            richTextBoxOptimal.Clear();
            richTextBoxCanonical.Clear();
            richTextBoxCanonical.Clear();
            richTextBoxTableau.Clear();
            sensitivityPanel.Controls.Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void steepestAscentDescentToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
