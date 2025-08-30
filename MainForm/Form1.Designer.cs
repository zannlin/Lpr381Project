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
            goldenSearchToolStripMenuItem = new ToolStripMenuItem();
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
            btnClearInput = new Button();
            btnClearAll = new Button();
            btnClose = new Button();
            tabControl2 = new TabControl();
            tabLinear = new TabPage();
            textBoxInput = new TextBox();
            tabNonLinear = new TabPage();
            txtUpp = new TextBox();
            txtLow = new TextBox();
            txtFunct = new TextBox();
            lblUpper = new Label();
            lblLow = new Label();
            lblFunct = new Label();
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
            steepestAscentDescentToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl2.SuspendLayout();
            tabLinear.SuspendLayout();
            tabNonLinear.SuspendLayout();
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
            fileToolStripMenuItem.BackColor = Color.LawnGreen;
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadInputFileToolStripMenuItem, exportOutputFileToolStripMenuItem });
            fileToolStripMenuItem.ForeColor = SystemColors.ControlText;
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
            solveToolStripMenuItem.BackColor = Color.LawnGreen;
            solveToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { primalSimplexToolStripMenuItem, revisedPrimalSimplexToolStripMenuItem, cuttingPlaneToolStripMenuItem, branchAndBoundToolStripMenuItem, knapsackToolStripMenuItem, goldenSearchToolStripMenuItem, steepestAscentDescentToolStripMenuItem });
            solveToolStripMenuItem.Name = "solveToolStripMenuItem";
            solveToolStripMenuItem.Size = new Size(59, 24);
            solveToolStripMenuItem.Text = "Solve";
            // 
            // primalSimplexToolStripMenuItem
            // 
            primalSimplexToolStripMenuItem.BackColor = Color.DarkSlateGray;
            primalSimplexToolStripMenuItem.ForeColor = SystemColors.Control;
            primalSimplexToolStripMenuItem.Name = "primalSimplexToolStripMenuItem";
            primalSimplexToolStripMenuItem.Size = new Size(256, 26);
            primalSimplexToolStripMenuItem.Text = "Primal Simplex";
            primalSimplexToolStripMenuItem.Click += primalSimplexToolStripMenuItem_Click;
            // 
            // revisedPrimalSimplexToolStripMenuItem
            // 
            revisedPrimalSimplexToolStripMenuItem.BackColor = Color.DarkSlateGray;
            revisedPrimalSimplexToolStripMenuItem.ForeColor = SystemColors.Control;
            revisedPrimalSimplexToolStripMenuItem.Name = "revisedPrimalSimplexToolStripMenuItem";
            revisedPrimalSimplexToolStripMenuItem.Size = new Size(256, 26);
            revisedPrimalSimplexToolStripMenuItem.Text = "Revised Primal Simplex";
            revisedPrimalSimplexToolStripMenuItem.Click += revisedPrimalSimplexToolStripMenuItem_Click;
            // 
            // cuttingPlaneToolStripMenuItem
            // 
            cuttingPlaneToolStripMenuItem.BackColor = Color.DarkSlateGray;
            cuttingPlaneToolStripMenuItem.ForeColor = SystemColors.Control;
            cuttingPlaneToolStripMenuItem.Name = "cuttingPlaneToolStripMenuItem";
            cuttingPlaneToolStripMenuItem.Size = new Size(256, 26);
            cuttingPlaneToolStripMenuItem.Text = "Cutting Plane";
            cuttingPlaneToolStripMenuItem.Click += cuttingPlaneToolStripMenuItem_Click;
            // 
            // branchAndBoundToolStripMenuItem
            // 
            branchAndBoundToolStripMenuItem.BackColor = Color.DarkSlateGray;
            branchAndBoundToolStripMenuItem.ForeColor = SystemColors.Control;
            branchAndBoundToolStripMenuItem.Name = "branchAndBoundToolStripMenuItem";
            branchAndBoundToolStripMenuItem.Size = new Size(256, 26);
            branchAndBoundToolStripMenuItem.Text = "Branch and Bound";
            branchAndBoundToolStripMenuItem.Click += branchAndBoundToolStripMenuItem_Click;
            // 
            // knapsackToolStripMenuItem
            // 
            knapsackToolStripMenuItem.BackColor = Color.DarkSlateGray;
            knapsackToolStripMenuItem.ForeColor = SystemColors.Control;
            knapsackToolStripMenuItem.Name = "knapsackToolStripMenuItem";
            knapsackToolStripMenuItem.Size = new Size(256, 26);
            knapsackToolStripMenuItem.Text = "Knapsack";
            knapsackToolStripMenuItem.Click += knapsackToolStripMenuItem_Click;
            // 
            // goldenSearchToolStripMenuItem
            // 
            goldenSearchToolStripMenuItem.BackColor = Color.DarkSlateGray;
            goldenSearchToolStripMenuItem.ForeColor = SystemColors.Control;
            goldenSearchToolStripMenuItem.Name = "goldenSearchToolStripMenuItem";
            goldenSearchToolStripMenuItem.Size = new Size(256, 26);
            goldenSearchToolStripMenuItem.Text = "Golden Search";
            goldenSearchToolStripMenuItem.Click += goldenSearchToolStripMenuItem_Click;
            // 
            // sensitivityAnalysisToolStripMenuItem
            // 
            sensitivityAnalysisToolStripMenuItem.BackColor = Color.LawnGreen;
            sensitivityAnalysisToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { rangeOfNonBasicVariablesToolStripMenuItem, changeNonBasicVariableToolStripMenuItem, rangeOfBasicVariableToolStripMenuItem, changeBasicVariableToolStripMenuItem, rangeOfConstraintRHSToolStripMenuItem, changeConstraintRHSToolStripMenuItem, rangeOfVariableInNonBasicColumnToolStripMenuItem, changeVariableInNonBasicColumnToolStripMenuItem, addNewActivityToolStripMenuItem, addNewConstraintToolStripMenuItem, displayShadowPricesToolStripMenuItem, dualityAnalysisToolStripMenuItem });
            sensitivityAnalysisToolStripMenuItem.Name = "sensitivityAnalysisToolStripMenuItem";
            sensitivityAnalysisToolStripMenuItem.Size = new Size(146, 24);
            sensitivityAnalysisToolStripMenuItem.Text = "Sensitivity Analysis";
            // 
            // rangeOfNonBasicVariablesToolStripMenuItem
            // 
            rangeOfNonBasicVariablesToolStripMenuItem.BackColor = Color.DarkSlateGray;
            rangeOfNonBasicVariablesToolStripMenuItem.ForeColor = SystemColors.Control;
            rangeOfNonBasicVariablesToolStripMenuItem.Name = "rangeOfNonBasicVariablesToolStripMenuItem";
            rangeOfNonBasicVariablesToolStripMenuItem.Size = new Size(353, 26);
            rangeOfNonBasicVariablesToolStripMenuItem.Text = "Range of Non-Basic Variables";
            rangeOfNonBasicVariablesToolStripMenuItem.Click += rangeOfNonBasicVariablesToolStripMenuItem_Click;
            // 
            // changeNonBasicVariableToolStripMenuItem
            // 
            changeNonBasicVariableToolStripMenuItem.BackColor = Color.DarkSlateGray;
            changeNonBasicVariableToolStripMenuItem.ForeColor = SystemColors.Control;
            changeNonBasicVariableToolStripMenuItem.Name = "changeNonBasicVariableToolStripMenuItem";
            changeNonBasicVariableToolStripMenuItem.Size = new Size(353, 26);
            changeNonBasicVariableToolStripMenuItem.Text = "Change Non-Basic Variable";
            changeNonBasicVariableToolStripMenuItem.Click += changeNonBasicVariableToolStripMenuItem_Click;
            // 
            // rangeOfBasicVariableToolStripMenuItem
            // 
            rangeOfBasicVariableToolStripMenuItem.BackColor = Color.DarkSlateGray;
            rangeOfBasicVariableToolStripMenuItem.ForeColor = SystemColors.Control;
            rangeOfBasicVariableToolStripMenuItem.Name = "rangeOfBasicVariableToolStripMenuItem";
            rangeOfBasicVariableToolStripMenuItem.Size = new Size(353, 26);
            rangeOfBasicVariableToolStripMenuItem.Text = "Range of Basic Variable";
            rangeOfBasicVariableToolStripMenuItem.Click += rangeOfBasicVariableToolStripMenuItem_Click;
            // 
            // changeBasicVariableToolStripMenuItem
            // 
            changeBasicVariableToolStripMenuItem.BackColor = Color.DarkSlateGray;
            changeBasicVariableToolStripMenuItem.ForeColor = SystemColors.Control;
            changeBasicVariableToolStripMenuItem.Name = "changeBasicVariableToolStripMenuItem";
            changeBasicVariableToolStripMenuItem.Size = new Size(353, 26);
            changeBasicVariableToolStripMenuItem.Text = "Change Basic Variable";
            changeBasicVariableToolStripMenuItem.Click += changeBasicVariableToolStripMenuItem_Click;
            // 
            // rangeOfConstraintRHSToolStripMenuItem
            // 
            rangeOfConstraintRHSToolStripMenuItem.BackColor = Color.DarkSlateGray;
            rangeOfConstraintRHSToolStripMenuItem.ForeColor = SystemColors.Control;
            rangeOfConstraintRHSToolStripMenuItem.Name = "rangeOfConstraintRHSToolStripMenuItem";
            rangeOfConstraintRHSToolStripMenuItem.Size = new Size(353, 26);
            rangeOfConstraintRHSToolStripMenuItem.Text = "Range of Constraint RHS";
            rangeOfConstraintRHSToolStripMenuItem.Click += rangeOfConstraintRHSToolStripMenuItem_Click;
            // 
            // changeConstraintRHSToolStripMenuItem
            // 
            changeConstraintRHSToolStripMenuItem.BackColor = Color.DarkSlateGray;
            changeConstraintRHSToolStripMenuItem.ForeColor = SystemColors.Control;
            changeConstraintRHSToolStripMenuItem.Name = "changeConstraintRHSToolStripMenuItem";
            changeConstraintRHSToolStripMenuItem.Size = new Size(353, 26);
            changeConstraintRHSToolStripMenuItem.Text = "Change Constraint RHS";
            changeConstraintRHSToolStripMenuItem.Click += changeConstraintRHSToolStripMenuItem_Click;
            // 
            // rangeOfVariableInNonBasicColumnToolStripMenuItem
            // 
            rangeOfVariableInNonBasicColumnToolStripMenuItem.BackColor = Color.DarkSlateGray;
            rangeOfVariableInNonBasicColumnToolStripMenuItem.ForeColor = SystemColors.Control;
            rangeOfVariableInNonBasicColumnToolStripMenuItem.Name = "rangeOfVariableInNonBasicColumnToolStripMenuItem";
            rangeOfVariableInNonBasicColumnToolStripMenuItem.Size = new Size(353, 26);
            rangeOfVariableInNonBasicColumnToolStripMenuItem.Text = "Range of Variable in Non-Basic Column";
            rangeOfVariableInNonBasicColumnToolStripMenuItem.Click += rangeOfVariableInNonBasicColumnToolStripMenuItem_Click;
            // 
            // changeVariableInNonBasicColumnToolStripMenuItem
            // 
            changeVariableInNonBasicColumnToolStripMenuItem.BackColor = Color.DarkSlateGray;
            changeVariableInNonBasicColumnToolStripMenuItem.ForeColor = SystemColors.Control;
            changeVariableInNonBasicColumnToolStripMenuItem.Name = "changeVariableInNonBasicColumnToolStripMenuItem";
            changeVariableInNonBasicColumnToolStripMenuItem.Size = new Size(353, 26);
            changeVariableInNonBasicColumnToolStripMenuItem.Text = "Change Variable in Non-Basic Column";
            changeVariableInNonBasicColumnToolStripMenuItem.Click += changeVariableInNonBasicColumnToolStripMenuItem_Click;
            // 
            // addNewActivityToolStripMenuItem
            // 
            addNewActivityToolStripMenuItem.BackColor = Color.DarkSlateGray;
            addNewActivityToolStripMenuItem.ForeColor = SystemColors.Control;
            addNewActivityToolStripMenuItem.Name = "addNewActivityToolStripMenuItem";
            addNewActivityToolStripMenuItem.Size = new Size(353, 26);
            addNewActivityToolStripMenuItem.Text = "Add New Activity";
            addNewActivityToolStripMenuItem.Click += addNewActivityToolStripMenuItem_Click;
            // 
            // addNewConstraintToolStripMenuItem
            // 
            addNewConstraintToolStripMenuItem.BackColor = Color.DarkSlateGray;
            addNewConstraintToolStripMenuItem.ForeColor = SystemColors.Control;
            addNewConstraintToolStripMenuItem.Name = "addNewConstraintToolStripMenuItem";
            addNewConstraintToolStripMenuItem.Size = new Size(353, 26);
            addNewConstraintToolStripMenuItem.Text = "Add New Constraint";
            addNewConstraintToolStripMenuItem.Click += addNewConstraintToolStripMenuItem_Click;
            // 
            // displayShadowPricesToolStripMenuItem
            // 
            displayShadowPricesToolStripMenuItem.BackColor = Color.DarkSlateGray;
            displayShadowPricesToolStripMenuItem.ForeColor = SystemColors.Control;
            displayShadowPricesToolStripMenuItem.Name = "displayShadowPricesToolStripMenuItem";
            displayShadowPricesToolStripMenuItem.Size = new Size(353, 26);
            displayShadowPricesToolStripMenuItem.Text = "Display Shadow Prices";
            displayShadowPricesToolStripMenuItem.Click += displayShadowPricesToolStripMenuItem_Click;
            // 
            // dualityAnalysisToolStripMenuItem
            // 
            dualityAnalysisToolStripMenuItem.BackColor = Color.DarkSlateGray;
            dualityAnalysisToolStripMenuItem.ForeColor = SystemColors.Control;
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
            splitContainer1.Panel1.Controls.Add(btnClearInput);
            splitContainer1.Panel1.Controls.Add(btnClearAll);
            splitContainer1.Panel1.Controls.Add(btnClose);
            splitContainer1.Panel1.Controls.Add(tabControl2);
            splitContainer1.Panel1.Controls.Add(lft_Lbl);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Size = new Size(982, 725);
            splitContainer1.SplitterDistance = 332;
            splitContainer1.TabIndex = 1;
            // 
            // btnClearInput
            // 
            btnClearInput.BackColor = Color.LawnGreen;
            btnClearInput.ForeColor = SystemColors.ActiveCaptionText;
            btnClearInput.Location = new Point(114, 513);
            btnClearInput.Name = "btnClearInput";
            btnClearInput.Size = new Size(94, 29);
            btnClearInput.TabIndex = 5;
            btnClearInput.Text = "Clear Input";
            btnClearInput.UseVisualStyleBackColor = false;
            btnClearInput.Click += btnClearInput_Click;
            // 
            // btnClearAll
            // 
            btnClearAll.BackColor = Color.LawnGreen;
            btnClearAll.ForeColor = SystemColors.ControlText;
            btnClearAll.Location = new Point(114, 569);
            btnClearAll.Name = "btnClearAll";
            btnClearAll.Size = new Size(94, 29);
            btnClearAll.TabIndex = 4;
            btnClearAll.Text = "Clear All";
            btnClearAll.UseVisualStyleBackColor = false;
            btnClearAll.Click += btnClearAll_Click;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.LawnGreen;
            btnClose.Location = new Point(114, 626);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(94, 29);
            btnClose.TabIndex = 3;
            btnClose.Text = "Exit";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(tabLinear);
            tabControl2.Controls.Add(tabNonLinear);
            tabControl2.Dock = DockStyle.Top;
            tabControl2.Location = new Point(0, 28);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(330, 396);
            tabControl2.TabIndex = 2;
            // 
            // tabLinear
            // 
            tabLinear.BackColor = Color.LawnGreen;
            tabLinear.BorderStyle = BorderStyle.FixedSingle;
            tabLinear.Controls.Add(textBoxInput);
            tabLinear.Location = new Point(4, 29);
            tabLinear.Name = "tabLinear";
            tabLinear.Padding = new Padding(3);
            tabLinear.Size = new Size(322, 363);
            tabLinear.TabIndex = 0;
            tabLinear.Text = "Linear Model";
            // 
            // textBoxInput
            // 
            textBoxInput.BackColor = Color.DarkSlateGray;
            textBoxInput.Dock = DockStyle.Fill;
            textBoxInput.ForeColor = SystemColors.Control;
            textBoxInput.Location = new Point(3, 3);
            textBoxInput.Multiline = true;
            textBoxInput.Name = "textBoxInput";
            textBoxInput.ReadOnly = true;
            textBoxInput.Size = new Size(314, 355);
            textBoxInput.TabIndex = 1;
            textBoxInput.TextChanged += textBoxInput_TextChanged;
            // 
            // tabNonLinear
            // 
            tabNonLinear.BackColor = Color.DarkSlateGray;
            tabNonLinear.BorderStyle = BorderStyle.FixedSingle;
            tabNonLinear.Controls.Add(txtUpp);
            tabNonLinear.Controls.Add(txtLow);
            tabNonLinear.Controls.Add(txtFunct);
            tabNonLinear.Controls.Add(lblUpper);
            tabNonLinear.Controls.Add(lblLow);
            tabNonLinear.Controls.Add(lblFunct);
            tabNonLinear.ForeColor = SystemColors.Control;
            tabNonLinear.Location = new Point(4, 29);
            tabNonLinear.Name = "tabNonLinear";
            tabNonLinear.Padding = new Padding(3);
            tabNonLinear.Size = new Size(322, 363);
            tabNonLinear.TabIndex = 1;
            tabNonLinear.Text = "Non-Linear Model";
            // 
            // txtUpp
            // 
            txtUpp.Location = new Point(140, 177);
            txtUpp.Name = "txtUpp";
            txtUpp.Size = new Size(125, 27);
            txtUpp.TabIndex = 5;
            // 
            // txtLow
            // 
            txtLow.Location = new Point(140, 115);
            txtLow.Name = "txtLow";
            txtLow.Size = new Size(125, 27);
            txtLow.TabIndex = 4;
            // 
            // txtFunct
            // 
            txtFunct.Location = new Point(140, 52);
            txtFunct.Name = "txtFunct";
            txtFunct.Size = new Size(125, 27);
            txtFunct.TabIndex = 3;
            // 
            // lblUpper
            // 
            lblUpper.AutoSize = true;
            lblUpper.Location = new Point(27, 177);
            lblUpper.Name = "lblUpper";
            lblUpper.Size = new Size(66, 20);
            lblUpper.TabIndex = 2;
            lblUpper.Text = "Upper/y:";
            // 
            // lblLow
            // 
            lblLow.AutoSize = true;
            lblLow.Location = new Point(27, 115);
            lblLow.Name = "lblLow";
            lblLow.Size = new Size(65, 20);
            lblLow.TabIndex = 1;
            lblLow.Text = "Lower/x:";
            // 
            // lblFunct
            // 
            lblFunct.AutoSize = true;
            lblFunct.Location = new Point(27, 52);
            lblFunct.Name = "lblFunct";
            lblFunct.Size = new Size(68, 20);
            lblFunct.TabIndex = 0;
            lblFunct.Text = "Function:";
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
            tabCan.BackColor = Color.LawnGreen;
            tabCan.BorderStyle = BorderStyle.FixedSingle;
            tabCan.Controls.Add(richTextBoxCanonical);
            tabCan.Cursor = Cursors.Hand;
            tabCan.Location = new Point(4, 29);
            tabCan.Name = "tabCan";
            tabCan.Padding = new Padding(3);
            tabCan.Size = new Size(636, 690);
            tabCan.TabIndex = 0;
            tabCan.Text = "Cannonical Form";
            // 
            // richTextBoxCanonical
            // 
            richTextBoxCanonical.BackColor = Color.DarkSlateGray;
            richTextBoxCanonical.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxCanonical.Dock = DockStyle.Fill;
            richTextBoxCanonical.ForeColor = SystemColors.Control;
            richTextBoxCanonical.Location = new Point(3, 3);
            richTextBoxCanonical.Name = "richTextBoxCanonical";
            richTextBoxCanonical.ReadOnly = true;
            richTextBoxCanonical.Size = new Size(628, 682);
            richTextBoxCanonical.TabIndex = 0;
            richTextBoxCanonical.Text = "";
            // 
            // tabIt
            // 
            tabIt.BackColor = Color.LawnGreen;
            tabIt.BorderStyle = BorderStyle.FixedSingle;
            tabIt.Controls.Add(richTextBoxTableau);
            tabIt.Location = new Point(4, 29);
            tabIt.Name = "tabIt";
            tabIt.Padding = new Padding(3);
            tabIt.Size = new Size(636, 690);
            tabIt.TabIndex = 1;
            tabIt.Text = "Tableau Iterations";
            // 
            // richTextBoxTableau
            // 
            richTextBoxTableau.BackColor = Color.DarkSlateGray;
            richTextBoxTableau.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxTableau.Dock = DockStyle.Fill;
            richTextBoxTableau.ForeColor = SystemColors.Control;
            richTextBoxTableau.Location = new Point(3, 3);
            richTextBoxTableau.Name = "richTextBoxTableau";
            richTextBoxTableau.ReadOnly = true;
            richTextBoxTableau.Size = new Size(628, 682);
            richTextBoxTableau.TabIndex = 0;
            richTextBoxTableau.Text = "";
            // 
            // tabOpt
            // 
            tabOpt.BackColor = Color.LawnGreen;
            tabOpt.BorderStyle = BorderStyle.FixedSingle;
            tabOpt.Controls.Add(richTextBoxOptimal);
            tabOpt.Location = new Point(4, 29);
            tabOpt.Name = "tabOpt";
            tabOpt.Padding = new Padding(3);
            tabOpt.Size = new Size(636, 690);
            tabOpt.TabIndex = 2;
            tabOpt.Text = "Optimal Solution";
            // 
            // richTextBoxOptimal
            // 
            richTextBoxOptimal.BackColor = Color.DarkSlateGray;
            richTextBoxOptimal.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxOptimal.Dock = DockStyle.Fill;
            richTextBoxOptimal.ForeColor = SystemColors.Control;
            richTextBoxOptimal.Location = new Point(3, 3);
            richTextBoxOptimal.Name = "richTextBoxOptimal";
            richTextBoxOptimal.ReadOnly = true;
            richTextBoxOptimal.Size = new Size(628, 682);
            richTextBoxOptimal.TabIndex = 0;
            richTextBoxOptimal.Text = "";
            // 
            // tabSens
            // 
            tabSens.BackColor = Color.LawnGreen;
            tabSens.BorderStyle = BorderStyle.FixedSingle;
            tabSens.Controls.Add(sensitivityPanel);
            tabSens.Location = new Point(4, 29);
            tabSens.Name = "tabSens";
            tabSens.Size = new Size(636, 690);
            tabSens.TabIndex = 3;
            tabSens.Text = "Sensitivity Analysis";
            // 
            // sensitivityPanel
            // 
            sensitivityPanel.AutoScroll = true;
            sensitivityPanel.BackColor = Color.DarkSlateGray;
            sensitivityPanel.BorderStyle = BorderStyle.FixedSingle;
            sensitivityPanel.Dock = DockStyle.Fill;
            sensitivityPanel.ForeColor = SystemColors.Control;
            sensitivityPanel.Location = new Point(0, 0);
            sensitivityPanel.Name = "sensitivityPanel";
            sensitivityPanel.Size = new Size(634, 688);
            sensitivityPanel.TabIndex = 0;
            // 
            // steepestAscentDescentToolStripMenuItem
            // 
            steepestAscentDescentToolStripMenuItem.BackColor = Color.DarkSlateGray;
            steepestAscentDescentToolStripMenuItem.ForeColor = SystemColors.Control;
            steepestAscentDescentToolStripMenuItem.Name = "steepestAscentDescentToolStripMenuItem";
            steepestAscentDescentToolStripMenuItem.Size = new Size(256, 26);
            steepestAscentDescentToolStripMenuItem.Text = "Steepest Ascent/Descent";
            steepestAscentDescentToolStripMenuItem.Click += steepestAscentDescentToolStripMenuItem_Click;
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
            tabControl2.ResumeLayout(false);
            tabLinear.ResumeLayout(false);
            tabLinear.PerformLayout();
            tabNonLinear.ResumeLayout(false);
            tabNonLinear.PerformLayout();
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
        private TabControl tabControl2;
        private TabPage tabLinear;
        private TabPage tabNonLinear;
        private TextBox txtFunct;
        private Label lblUpper;
        private Label lblLow;
        private Label lblFunct;
        private TextBox txtLow;
        private Button btnClearInput;
        private Button btnClearAll;
        private Button btnClose;
        private TextBox txtUpp;
        private ToolStripMenuItem goldenSearchToolStripMenuItem;
        private ToolStripMenuItem steepestAscentDescentToolStripMenuItem;
    }
}
