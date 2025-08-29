namespace MainForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            loadInputFileToolStripMenuItem = new ToolStripMenuItem();
            exportOutputFileToolStripMenuItem = new ToolStripMenuItem();
            solveToolStripMenuItem = new ToolStripMenuItem();
            primalSimplexToolStripMenuItem = new ToolStripMenuItem();
            revisedPrimalSimplexToolStripMenuItem = new ToolStripMenuItem();
            cuttingPlaneToolStripMenuItem = new ToolStripMenuItem();
            branchAndBoundToolStripMenuItem = new ToolStripMenuItem();
            knapsackToolStripMenuItem = new ToolStripMenuItem();
            sensitivityAnalysisToolStripMenuItem = new ToolStripMenuItem();
            rangeOfNonBasicVariablesToolStripMenuItem = new ToolStripMenuItem();
            changeNonBasicVariableToolStripMenuItem = new ToolStripMenuItem();
            rangeOfBasicVariableToolStripMenuItem = new ToolStripMenuItem();
            changeBasicVariableToolStripMenuItem = new ToolStripMenuItem();
            rangeOfConstraintRHSToolStripMenuItem = new ToolStripMenuItem();
            changeConstraintRHSToolStripMenuItem = new ToolStripMenuItem();
            rangeOfVariableInNonBasicColumnToolStripMenuItem = new ToolStripMenuItem();
            changeVariableInNonBasicColumnToolStripMenuItem = new ToolStripMenuItem();
            addNewActivityToolStripMenuItem = new ToolStripMenuItem();
            addNewConstraintToolStripMenuItem = new ToolStripMenuItem();
            displayShadowPricesToolStripMenuItem = new ToolStripMenuItem();
            dualityAnalysisToolStripMenuItem = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            textBoxInput = new TextBox();
            lft_Lbl = new Label();
            tabControl1 = new TabControl();
            tabCan = new TabPage();
            richTextBoxCanonical = new RichTextBox();
            tabIt = new TabPage();
            richTextBoxTableau = new RichTextBox();
            tabOpt = new TabPage();
            richTextBoxOptimal = new RichTextBox();
            tabSens = new TabPage();
            sensitivityPanel = new FlowLayoutPanel();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabCan.SuspendLayout();
            tabIt.SuspendLayout();
            tabOpt.SuspendLayout();
            tabSens.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.DarkSlateGray;
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, solveToolStripMenuItem, sensitivityAnalysisToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(982, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.BackColor = Color.SpringGreen;
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadInputFileToolStripMenuItem, exportOutputFileToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // loadInputFileToolStripMenuItem
            // 
            loadInputFileToolStripMenuItem.BackColor = Color.DarkSlateGray;
            loadInputFileToolStripMenuItem.ForeColor = SystemColors.Control;
            loadInputFileToolStripMenuItem.Name = "loadInputFileToolStripMenuItem";
            loadInputFileToolStripMenuItem.Size = new Size(212, 26);
            loadInputFileToolStripMenuItem.Text = "Load Input File";
            loadInputFileToolStripMenuItem.Click += loadInputFileToolStripMenuItem_Click;
            // 
            // exportOutputFileToolStripMenuItem
            // 
            exportOutputFileToolStripMenuItem.BackColor = Color.DarkSlateGray;
            exportOutputFileToolStripMenuItem.ForeColor = SystemColors.Control;
            exportOutputFileToolStripMenuItem.Name = "exportOutputFileToolStripMenuItem";
            exportOutputFileToolStripMenuItem.Size = new Size(212, 26);
            exportOutputFileToolStripMenuItem.Text = "Export Output File";
            exportOutputFileToolStripMenuItem.Click += exportOutputFileToolStripMenuItem_Click;
            // 
            // solveToolStripMenuItem
            // 
            solveToolStripMenuItem.BackColor = Color.SpringGreen;
            solveToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { primalSimplexToolStripMenuItem, revisedPrimalSimplexToolStripMenuItem, cuttingPlaneToolStripMenuItem, branchAndBoundToolStripMenuItem, knapsackToolStripMenuItem });
            solveToolStripMenuItem.Name = "solveToolStripMenuItem";
            solveToolStripMenuItem.Size = new Size(59, 24);
            solveToolStripMenuItem.Text = "Solve";
            // 
            // primalSimplexToolStripMenuItem
            // 
            primalSimplexToolStripMenuItem.BackColor = Color.DarkSlateGray;
            primalSimplexToolStripMenuItem.ForeColor = SystemColors.Control;
            primalSimplexToolStripMenuItem.Name = "primalSimplexToolStripMenuItem";
            primalSimplexToolStripMenuItem.Size = new Size(246, 26);
            primalSimplexToolStripMenuItem.Text = "Primal Simplex";
            primalSimplexToolStripMenuItem.Click += primalSimplexToolStripMenuItem_Click;
            // 
            // revisedPrimalSimplexToolStripMenuItem
            // 
            revisedPrimalSimplexToolStripMenuItem.BackColor = Color.DarkSlateGray;
            revisedPrimalSimplexToolStripMenuItem.ForeColor = SystemColors.Control;
            revisedPrimalSimplexToolStripMenuItem.Name = "revisedPrimalSimplexToolStripMenuItem";
            revisedPrimalSimplexToolStripMenuItem.Size = new Size(246, 26);
            revisedPrimalSimplexToolStripMenuItem.Text = "Revised Primal Simplex";
            // 
            // cuttingPlaneToolStripMenuItem
            // 
            cuttingPlaneToolStripMenuItem.Name = "cuttingPlaneToolStripMenuItem";
            cuttingPlaneToolStripMenuItem.Size = new Size(246, 26);
            cuttingPlaneToolStripMenuItem.Text = "Cutting Plane";
            cuttingPlaneToolStripMenuItem.Click += cuttingPlaneToolStripMenuItem_Click;
            // 
            // branchAndBoundToolStripMenuItem
            // 
            branchAndBoundToolStripMenuItem.Name = "branchAndBoundToolStripMenuItem";
            branchAndBoundToolStripMenuItem.Size = new Size(246, 26);
            branchAndBoundToolStripMenuItem.Text = "Branch and Bound";
            branchAndBoundToolStripMenuItem.Click += branchAndBoundToolStripMenuItem_Click;
            // 
            // knapsackToolStripMenuItem
            // 
            knapsackToolStripMenuItem.Name = "knapsackToolStripMenuItem";
            knapsackToolStripMenuItem.Size = new Size(246, 26);
            knapsackToolStripMenuItem.Text = "Knapsack";
            knapsackToolStripMenuItem.Click += knapsackToolStripMenuItem_Click;
            // 
            // sensitivityAnalysisToolStripMenuItem
            // 
            sensitivityAnalysisToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { rangeOfNonBasicVariablesToolStripMenuItem, changeNonBasicVariableToolStripMenuItem, rangeOfBasicVariableToolStripMenuItem, changeBasicVariableToolStripMenuItem, rangeOfConstraintRHSToolStripMenuItem, changeConstraintRHSToolStripMenuItem, rangeOfVariableInNonBasicColumnToolStripMenuItem, changeVariableInNonBasicColumnToolStripMenuItem, addNewActivityToolStripMenuItem, addNewConstraintToolStripMenuItem, displayShadowPricesToolStripMenuItem, dualityAnalysisToolStripMenuItem });
            sensitivityAnalysisToolStripMenuItem.Name = "sensitivityAnalysisToolStripMenuItem";
            sensitivityAnalysisToolStripMenuItem.Size = new Size(146, 24);
            sensitivityAnalysisToolStripMenuItem.Text = "Sensitivity Analysis";
            // 
            // rangeOfNonBasicVariablesToolStripMenuItem
            // 
            rangeOfNonBasicVariablesToolStripMenuItem.Name = "rangeOfNonBasicVariablesToolStripMenuItem";
            rangeOfNonBasicVariablesToolStripMenuItem.Size = new Size(353, 26);
            rangeOfNonBasicVariablesToolStripMenuItem.Text = "Range of Non-Basic Variables";
            rangeOfNonBasicVariablesToolStripMenuItem.Click += rangeOfNonBasicVariablesToolStripMenuItem_Click;
            // 
            // changeNonBasicVariableToolStripMenuItem
            // 
            changeNonBasicVariableToolStripMenuItem.Name = "changeNonBasicVariableToolStripMenuItem";
            changeNonBasicVariableToolStripMenuItem.Size = new Size(353, 26);
            changeNonBasicVariableToolStripMenuItem.Text = "Change Non-Basic Variable";
            changeNonBasicVariableToolStripMenuItem.Click += changeNonBasicVariableToolStripMenuItem_Click;
            // 
            // rangeOfBasicVariableToolStripMenuItem
            // 
            rangeOfBasicVariableToolStripMenuItem.Name = "rangeOfBasicVariableToolStripMenuItem";
            rangeOfBasicVariableToolStripMenuItem.Size = new Size(353, 26);
            rangeOfBasicVariableToolStripMenuItem.Text = "Range of Basic Variable";
            rangeOfBasicVariableToolStripMenuItem.Click += rangeOfBasicVariableToolStripMenuItem_Click;
            // 
            // changeBasicVariableToolStripMenuItem
            // 
            changeBasicVariableToolStripMenuItem.Name = "changeBasicVariableToolStripMenuItem";
            changeBasicVariableToolStripMenuItem.Size = new Size(353, 26);
            changeBasicVariableToolStripMenuItem.Text = "Change Basic Variable";
            changeBasicVariableToolStripMenuItem.Click += changeBasicVariableToolStripMenuItem_Click;
            // 
            // rangeOfConstraintRHSToolStripMenuItem
            // 
            rangeOfConstraintRHSToolStripMenuItem.Name = "rangeOfConstraintRHSToolStripMenuItem";
            rangeOfConstraintRHSToolStripMenuItem.Size = new Size(353, 26);
            rangeOfConstraintRHSToolStripMenuItem.Text = "Range of Constraint RHS";
            rangeOfConstraintRHSToolStripMenuItem.Click += rangeOfConstraintRHSToolStripMenuItem_Click;
            // 
            // changeConstraintRHSToolStripMenuItem
            // 
            changeConstraintRHSToolStripMenuItem.Name = "changeConstraintRHSToolStripMenuItem";
            changeConstraintRHSToolStripMenuItem.Size = new Size(353, 26);
            changeConstraintRHSToolStripMenuItem.Text = "Change Constraint RHS";
            changeConstraintRHSToolStripMenuItem.Click += changeConstraintRHSToolStripMenuItem_Click;
            // 
            // rangeOfVariableInNonBasicColumnToolStripMenuItem
            // 
            rangeOfVariableInNonBasicColumnToolStripMenuItem.Name = "rangeOfVariableInNonBasicColumnToolStripMenuItem";
            rangeOfVariableInNonBasicColumnToolStripMenuItem.Size = new Size(353, 26);
            rangeOfVariableInNonBasicColumnToolStripMenuItem.Text = "Range of Variable in Non-Basic Column";
            rangeOfVariableInNonBasicColumnToolStripMenuItem.Click += rangeOfVariableInNonBasicColumnToolStripMenuItem_Click;
            // 
            // changeVariableInNonBasicColumnToolStripMenuItem
            // 
            changeVariableInNonBasicColumnToolStripMenuItem.Name = "changeVariableInNonBasicColumnToolStripMenuItem";
            changeVariableInNonBasicColumnToolStripMenuItem.Size = new Size(353, 26);
            changeVariableInNonBasicColumnToolStripMenuItem.Text = "Change Variable in Non-Basic Column";
            changeVariableInNonBasicColumnToolStripMenuItem.Click += changeVariableInNonBasicColumnToolStripMenuItem_Click;
            // 
            // addNewActivityToolStripMenuItem
            // 
            addNewActivityToolStripMenuItem.Name = "addNewActivityToolStripMenuItem";
            addNewActivityToolStripMenuItem.Size = new Size(353, 26);
            addNewActivityToolStripMenuItem.Text = "Add New Activity";
            addNewActivityToolStripMenuItem.Click += addNewActivityToolStripMenuItem_Click;
            // 
            // addNewConstraintToolStripMenuItem
            // 
            addNewConstraintToolStripMenuItem.Name = "addNewConstraintToolStripMenuItem";
            addNewConstraintToolStripMenuItem.Size = new Size(353, 26);
            addNewConstraintToolStripMenuItem.Text = "Add New Constraint";
            addNewConstraintToolStripMenuItem.Click += addNewConstraintToolStripMenuItem_Click;
            // 
            // displayShadowPricesToolStripMenuItem
            // 
            displayShadowPricesToolStripMenuItem.Name = "displayShadowPricesToolStripMenuItem";
            displayShadowPricesToolStripMenuItem.Size = new Size(353, 26);
            displayShadowPricesToolStripMenuItem.Text = "Display Shadow Prices";
            displayShadowPricesToolStripMenuItem.Click += displayShadowPricesToolStripMenuItem_Click;
            // 
            // dualityAnalysisToolStripMenuItem
            // 
            dualityAnalysisToolStripMenuItem.Name = "dualityAnalysisToolStripMenuItem";
            dualityAnalysisToolStripMenuItem.Size = new Size(353, 26);
            dualityAnalysisToolStripMenuItem.Text = "Duality Analysis";
            dualityAnalysisToolStripMenuItem.Click += dualityAnalysisToolStripMenuItem_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.FixedSingle;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 28);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(textBoxInput);
            splitContainer1.Panel1.Controls.Add(lft_Lbl);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Size = new Size(982, 725);
            splitContainer1.SplitterDistance = 332;
            splitContainer1.TabIndex = 1;
            // 
            // textBoxInput
            // 
            textBoxInput.BackColor = SystemColors.ControlDark;
            textBoxInput.Dock = DockStyle.Fill;
            textBoxInput.Location = new Point(0, 28);
            textBoxInput.Multiline = true;
            textBoxInput.Name = "textBoxInput";
            textBoxInput.ReadOnly = true;
            textBoxInput.ScrollBars = ScrollBars.Vertical;
            textBoxInput.Size = new Size(330, 695);
            textBoxInput.TabIndex = 1;
            // 
            // lft_Lbl
            // 
            lft_Lbl.AutoSize = true;
            lft_Lbl.Dock = DockStyle.Top;
            lft_Lbl.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lft_Lbl.ForeColor = SystemColors.Control;
            lft_Lbl.Location = new Point(0, 0);
            lft_Lbl.Name = "lft_Lbl";
            lft_Lbl.Size = new Size(128, 28);
            lft_Lbl.TabIndex = 0;
            lft_Lbl.Text = "Input Model";
            lft_Lbl.TextAlign = ContentAlignment.TopCenter;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabCan);
            tabControl1.Controls.Add(tabIt);
            tabControl1.Controls.Add(tabOpt);
            tabControl1.Controls.Add(tabSens);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(644, 723);
            tabControl1.TabIndex = 0;
            // 
            // tabCan
            // 
            tabCan.Controls.Add(richTextBoxCanonical);
            tabCan.Location = new Point(4, 29);
            tabCan.Name = "tabCan";
            tabCan.Padding = new Padding(3);
            tabCan.Size = new Size(636, 690);
            tabCan.TabIndex = 0;
            tabCan.Text = "Cannonical Form";
            tabCan.UseVisualStyleBackColor = true;
            // 
            // richTextBoxCanonical
            // 
            richTextBoxCanonical.BackColor = Color.LightGray;
            richTextBoxCanonical.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxCanonical.Dock = DockStyle.Fill;
            richTextBoxCanonical.Location = new Point(3, 3);
            richTextBoxCanonical.Name = "richTextBoxCanonical";
            richTextBoxCanonical.ReadOnly = true;
            richTextBoxCanonical.Size = new Size(630, 684);
            richTextBoxCanonical.TabIndex = 0;
            richTextBoxCanonical.Text = "";
            // 
            // tabIt
            // 
            tabIt.Controls.Add(richTextBoxTableau);
            tabIt.Location = new Point(4, 29);
            tabIt.Name = "tabIt";
            tabIt.Padding = new Padding(3);
            tabIt.Size = new Size(636, 690);
            tabIt.TabIndex = 1;
            tabIt.Text = "Tableau Iterations";
            tabIt.UseVisualStyleBackColor = true;
            // 
            // richTextBoxTableau
            // 
            richTextBoxTableau.BackColor = Color.LightGray;
            richTextBoxTableau.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxTableau.Dock = DockStyle.Fill;
            richTextBoxTableau.Location = new Point(3, 3);
            richTextBoxTableau.Name = "richTextBoxTableau";
            richTextBoxTableau.ReadOnly = true;
            richTextBoxTableau.Size = new Size(630, 684);
            richTextBoxTableau.TabIndex = 0;
            richTextBoxTableau.Text = "";
            // 
            // tabOpt
            // 
            tabOpt.Controls.Add(richTextBoxOptimal);
            tabOpt.Location = new Point(4, 29);
            tabOpt.Name = "tabOpt";
            tabOpt.Padding = new Padding(3);
            tabOpt.Size = new Size(636, 690);
            tabOpt.TabIndex = 2;
            tabOpt.Text = "Optimal Solution";
            tabOpt.UseVisualStyleBackColor = true;
            // 
            // richTextBoxOptimal
            // 
            richTextBoxOptimal.BackColor = Color.LightGray;
            richTextBoxOptimal.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxOptimal.Dock = DockStyle.Fill;
            richTextBoxOptimal.Location = new Point(3, 3);
            richTextBoxOptimal.Name = "richTextBoxOptimal";
            richTextBoxOptimal.ReadOnly = true;
            richTextBoxOptimal.Size = new Size(630, 684);
            richTextBoxOptimal.TabIndex = 0;
            richTextBoxOptimal.Text = "";
            // 
            // tabSens
            // 
            tabSens.Controls.Add(sensitivityPanel);
            tabSens.Location = new Point(4, 29);
            tabSens.Name = "tabSens";
            tabSens.Size = new Size(636, 690);
            tabSens.TabIndex = 3;
            tabSens.Text = "Sensitivity Analysis";
            tabSens.UseVisualStyleBackColor = true;
            // 
            // sensitivityPanel
            // 
            sensitivityPanel.AutoScroll = true;
            sensitivityPanel.Dock = DockStyle.Fill;
            sensitivityPanel.Location = new Point(0, 0);
            sensitivityPanel.Name = "sensitivityPanel";
            sensitivityPanel.Size = new Size(636, 690);
            sensitivityPanel.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkSlateGray;
            ClientSize = new Size(982, 753);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Linear Programming Solver";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabCan.ResumeLayout(false);
            tabIt.ResumeLayout(false);
            tabOpt.ResumeLayout(false);
            tabSens.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadInputFileToolStripMenuItem;
        private ToolStripMenuItem exportOutputFileToolStripMenuItem;
        private ToolStripMenuItem solveToolStripMenuItem;
        private ToolStripMenuItem primalSimplexToolStripMenuItem;
        private ToolStripMenuItem branchAndBoundToolStripMenuItem;
        private ToolStripMenuItem cuttingPlaneToolStripMenuItem;
        private ToolStripMenuItem revisedPrimalSimplexToolStripMenuItem;
        private ToolStripMenuItem sensitivityAnalysisToolStripMenuItem;
        private ToolStripMenuItem rangeOfNonBasicVariablesToolStripMenuItem;
        private ToolStripMenuItem changeNonBasicVariableToolStripMenuItem;
        private ToolStripMenuItem rangeOfBasicVariableToolStripMenuItem;
        private ToolStripMenuItem changeBasicVariableToolStripMenuItem;
        private ToolStripMenuItem rangeOfConstraintRHSToolStripMenuItem;
        private ToolStripMenuItem changeConstraintRHSToolStripMenuItem;
        private ToolStripMenuItem rangeOfVariableInNonBasicColumnToolStripMenuItem;
        private ToolStripMenuItem changeVariableInNonBasicColumnToolStripMenuItem;
        private ToolStripMenuItem addNewActivityToolStripMenuItem;
        private ToolStripMenuItem addNewConstraintToolStripMenuItem;
        private ToolStripMenuItem displayShadowPricesToolStripMenuItem;
        private ToolStripMenuItem dualityAnalysisToolStripMenuItem;
        private SplitContainer splitContainer1;
        private Label lft_Lbl;
        private TextBox textBoxInput;
        private TabControl tabControl1;
        private TabPage tabCan;
        private TabPage tabIt;
        private TabPage tabOpt;
        private RichTextBox richTextBoxCanonical;
        private RichTextBox richTextBoxTableau;
        private RichTextBox richTextBoxOptimal;
        private TabPage tabSens;
        private ToolStripMenuItem knapsackToolStripMenuItem;
        private FlowLayoutPanel sensitivityPanel;
    }
}
