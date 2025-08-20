namespace LPR_Form
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
            button1 = new Button();
            CuttingPlane = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            richTextBox1 = new RichTextBox();
            label1 = new Label();
            UploadFilePath = new Button();
            fileDisplay = new TextBox();
            label2 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(197, 254);
            button1.Name = "button1";
            button1.Size = new Size(182, 35);
            button1.TabIndex = 0;
            button1.Text = "PrimalSimplex";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // CuttingPlane
            // 
            CuttingPlane.Location = new Point(196, 321);
            CuttingPlane.Name = "CuttingPlane";
            CuttingPlane.Size = new Size(182, 38);
            CuttingPlane.TabIndex = 1;
            CuttingPlane.Text = "Cutting Plane";
            CuttingPlane.UseVisualStyleBackColor = true;
            CuttingPlane.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(196, 384);
            button3.Name = "button3";
            button3.Size = new Size(182, 38);
            button3.TabIndex = 2;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(197, 448);
            button4.Name = "button4";
            button4.Size = new Size(182, 38);
            button4.TabIndex = 3;
            button4.Text = "button4";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(197, 514);
            button5.Name = "button5";
            button5.Size = new Size(182, 38);
            button5.TabIndex = 4;
            button5.Text = "Branch and Bound Knapsack";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(483, 189);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(625, 363);
            richTextBox1.TabIndex = 5;
            richTextBox1.Text = "";
            richTextBox1.TextChanged += richTextBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(306, 56);
            label1.Name = "label1";
            label1.Size = new Size(130, 15);
            label1.TabIndex = 6;
            label1.Text = "LPR 381 Model Selector";
            label1.Click += label1_Click;
            // 
            // UploadFilePath
            // 
            UploadFilePath.Location = new Point(196, 189);
            UploadFilePath.Name = "UploadFilePath";
            UploadFilePath.Size = new Size(183, 35);
            UploadFilePath.TabIndex = 7;
            UploadFilePath.Text = "UploadFilePath";
            UploadFilePath.UseVisualStyleBackColor = true;
            UploadFilePath.Click += UploadFilePath_Click;
            // 
            // fileDisplay
            // 
            fileDisplay.Location = new Point(550, 599);
            fileDisplay.Name = "fileDisplay";
            fileDisplay.Size = new Size(461, 23);
            fileDisplay.TabIndex = 8;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(728, 572);
            label2.Name = "label2";
            label2.Size = new Size(109, 15);
            label2.TabIndex = 9;
            label2.Text = "Uploaded File  path";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1136, 634);
            Controls.Add(label2);
            Controls.Add(fileDisplay);
            Controls.Add(UploadFilePath);
            Controls.Add(label1);
            Controls.Add(richTextBox1);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(CuttingPlane);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button CuttingPlane;
        private Button button3;
        private Button button4;
        private Button button5;
        private RichTextBox richTextBox1;
        private Label label1;
        private Button UploadFilePath;
        private TextBox fileDisplay;
        private Label label2;
    }
}
