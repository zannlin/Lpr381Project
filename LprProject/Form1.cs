using Lpr381Project;
using static Lpr381Project.PrimalSimplex;
using LPR_Form.Models;

namespace LPR_Form
{
    public partial class Form1 : Form
    {

        internal string[] content;
        private LinearModel model;
        double[] objectiveFunction;
        double[,] constraint;
        double[] rhs;
        public Form1()
        {
            InitializeComponent();


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string output = String.Empty;
            var result = PrimalSimplex.Solve(constraint, rhs, objectiveFunction);
            foreach (var snap in result.Tableaus)
            {
                output += snap.ToString();
            }

            output += "\n\n";
            output += "Optimal value z* = " + result.OptimalValue.ToString("0.###") + "\n";
            richTextBox1.Text = output;

            /*
            Console.WriteLine(result.Message);
            if (!result.IsUnbounded && !result.IsInfeasible)
            {
                Console.WriteLine("Optimal value z* = " + result.OptimalValue.ToString("0.###"));
                Console.WriteLine("x* = [" + string.Join(", ", result.PrimalVariables.Select(v => v.ToString("0.###"))) + "]");
            }*/
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private static string[] ReadFile(string path)
        {
            string filepath = @path;
            string[] content = File.ReadAllLines(filepath);
            return content;

        }

        private void UploadFilePath_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog opd = new OpenFileDialog())
            {
                opd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                try
                {
                    if (opd.ShowDialog() == DialogResult.OK)
                    {
                        string path = opd.FileName;
                        fileDisplay.Text = "File loaded: " + path + "\n";
                        content = ReadFile(path);
                        model = new LinearModel(content);
                        objectiveFunction = model.ObjectiveFuntion;
                        constraint = model.Constraints;
                        rhs = model.RightHandSide;
                    }



                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading file: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var result = PrimalSimplex.Solve(constraint, rhs, objectiveFunction);
            CuttingPlaneSimplex cuttingPlane = new CuttingPlaneSimplex(result);
            richTextBox1.Text = cuttingPlane.addNewConstraint();
        }
    }
}
