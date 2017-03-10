namespace MerkleTreeDemo
{
    partial class Demo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlFlowSharp = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.nudNumLeaves = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbAuditTrail = new System.Windows.Forms.TextBox();
            this.lblAuditPassFail = new System.Windows.Forms.Label();
            this.btnAuditProof = new System.Windows.Forms.Button();
            this.nudAuditProofNodeNumber = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbConsistencyTrail = new System.Windows.Forms.TextBox();
            this.lblConsistencyPassFail = new System.Windows.Forms.Label();
            this.btnConsistencyProof = new System.Windows.Forms.Button();
            this.nudConsistencyProofNumLeaves = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.ckOnlyToOldRoot = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumLeaves)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAuditProofNodeNumber)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudConsistencyProofNumLeaves)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlFlowSharp
            // 
            this.pnlFlowSharp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFlowSharp.Location = new System.Drawing.Point(282, 13);
            this.pnlFlowSharp.Name = "pnlFlowSharp";
            this.pnlFlowSharp.Size = new System.Drawing.Size(610, 475);
            this.pnlFlowSharp.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Number of Leaves:";
            // 
            // nudNumLeaves
            // 
            this.nudNumLeaves.Location = new System.Drawing.Point(116, 11);
            this.nudNumLeaves.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudNumLeaves.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudNumLeaves.Name = "nudNumLeaves";
            this.nudNumLeaves.Size = new System.Drawing.Size(49, 20);
            this.nudNumLeaves.TabIndex = 2;
            this.nudNumLeaves.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudNumLeaves.ValueChanged += new System.EventHandler(this.NumLeavesChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbAuditTrail);
            this.groupBox1.Controls.Add(this.lblAuditPassFail);
            this.groupBox1.Controls.Add(this.btnAuditProof);
            this.groupBox1.Controls.Add(this.nudAuditProofNodeNumber);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(16, 46);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 201);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Audit Proof";
            // 
            // tbAuditTrail
            // 
            this.tbAuditTrail.Location = new System.Drawing.Point(10, 73);
            this.tbAuditTrail.Multiline = true;
            this.tbAuditTrail.Name = "tbAuditTrail";
            this.tbAuditTrail.ReadOnly = true;
            this.tbAuditTrail.Size = new System.Drawing.Size(244, 115);
            this.tbAuditTrail.TabIndex = 7;
            // 
            // lblAuditPassFail
            // 
            this.lblAuditPassFail.AutoSize = true;
            this.lblAuditPassFail.Location = new System.Drawing.Point(203, 54);
            this.lblAuditPassFail.Name = "lblAuditPassFail";
            this.lblAuditPassFail.Size = new System.Drawing.Size(51, 13);
            this.lblAuditPassFail.TabIndex = 6;
            this.lblAuditPassFail.Text = "Pass/Fail";
            // 
            // btnAuditProof
            // 
            this.btnAuditProof.Location = new System.Drawing.Point(10, 44);
            this.btnAuditProof.Name = "btnAuditProof";
            this.btnAuditProof.Size = new System.Drawing.Size(70, 23);
            this.btnAuditProof.TabIndex = 5;
            this.btnAuditProof.Text = "Show Me";
            this.btnAuditProof.UseVisualStyleBackColor = true;
            this.btnAuditProof.Click += new System.EventHandler(this.btnAuditProof_Click);
            // 
            // nudAuditProofNodeNumber
            // 
            this.nudAuditProofNodeNumber.Location = new System.Drawing.Point(59, 18);
            this.nudAuditProofNodeNumber.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudAuditProofNodeNumber.Name = "nudAuditProofNodeNumber";
            this.nudAuditProofNodeNumber.Size = new System.Drawing.Size(49, 20);
            this.nudAuditProofNodeNumber.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Leaf #:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ckOnlyToOldRoot);
            this.groupBox2.Controls.Add(this.tbConsistencyTrail);
            this.groupBox2.Controls.Add(this.lblConsistencyPassFail);
            this.groupBox2.Controls.Add(this.btnConsistencyProof);
            this.groupBox2.Controls.Add(this.nudConsistencyProofNumLeaves);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(16, 253);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(260, 235);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Consistency Proof";
            // 
            // tbConsistencyTrail
            // 
            this.tbConsistencyTrail.Location = new System.Drawing.Point(10, 97);
            this.tbConsistencyTrail.Multiline = true;
            this.tbConsistencyTrail.Name = "tbConsistencyTrail";
            this.tbConsistencyTrail.ReadOnly = true;
            this.tbConsistencyTrail.Size = new System.Drawing.Size(244, 132);
            this.tbConsistencyTrail.TabIndex = 7;
            // 
            // lblConsistencyPassFail
            // 
            this.lblConsistencyPassFail.AutoSize = true;
            this.lblConsistencyPassFail.Location = new System.Drawing.Point(203, 75);
            this.lblConsistencyPassFail.Name = "lblConsistencyPassFail";
            this.lblConsistencyPassFail.Size = new System.Drawing.Size(51, 13);
            this.lblConsistencyPassFail.TabIndex = 6;
            this.lblConsistencyPassFail.Text = "Pass/Fail";
            // 
            // btnConsistencyProof
            // 
            this.btnConsistencyProof.Location = new System.Drawing.Point(10, 44);
            this.btnConsistencyProof.Name = "btnConsistencyProof";
            this.btnConsistencyProof.Size = new System.Drawing.Size(70, 23);
            this.btnConsistencyProof.TabIndex = 5;
            this.btnConsistencyProof.Text = "Show Me";
            this.btnConsistencyProof.UseVisualStyleBackColor = true;
            this.btnConsistencyProof.Click += new System.EventHandler(this.btnConsistencyProof_Click);
            // 
            // nudConsistencyProofNumLeaves
            // 
            this.nudConsistencyProofNumLeaves.Location = new System.Drawing.Point(69, 18);
            this.nudConsistencyProofNumLeaves.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudConsistencyProofNumLeaves.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudConsistencyProofNumLeaves.Name = "nudConsistencyProofNumLeaves";
            this.nudConsistencyProofNumLeaves.Size = new System.Drawing.Size(49, 20);
            this.nudConsistencyProofNumLeaves.TabIndex = 4;
            this.nudConsistencyProofNumLeaves.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "# Leaves:";
            // 
            // ckOnlyToOldRoot
            // 
            this.ckOnlyToOldRoot.AutoSize = true;
            this.ckOnlyToOldRoot.Location = new System.Drawing.Point(10, 74);
            this.ckOnlyToOldRoot.Name = "ckOnlyToOldRoot";
            this.ckOnlyToOldRoot.Size = new System.Drawing.Size(97, 17);
            this.ckOnlyToOldRoot.TabIndex = 8;
            this.ckOnlyToOldRoot.Text = "Only to old root";
            this.ckOnlyToOldRoot.UseVisualStyleBackColor = true;
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 500);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.nudNumLeaves);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlFlowSharp);
            this.Name = "Demo";
            this.Text = "Merkle Tree Demo";
            ((System.ComponentModel.ISupportInitialize)(this.nudNumLeaves)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAuditProofNodeNumber)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudConsistencyProofNumLeaves)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlFlowSharp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudNumLeaves;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAuditProof;
        private System.Windows.Forms.NumericUpDown nudAuditProofNodeNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbAuditTrail;
        private System.Windows.Forms.Label lblAuditPassFail;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbConsistencyTrail;
        private System.Windows.Forms.Label lblConsistencyPassFail;
        private System.Windows.Forms.Button btnConsistencyProof;
        private System.Windows.Forms.NumericUpDown nudConsistencyProofNumLeaves;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox ckOnlyToOldRoot;
    }
}

