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
    }
}
