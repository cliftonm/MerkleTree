namespace MerkleTreeDemo
{
    partial class Form1
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
            ((System.ComponentModel.ISupportInitialize)(this.nudNumLeaves)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlFlowSharp
            // 
            this.pnlFlowSharp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFlowSharp.Location = new System.Drawing.Point(12, 100);
            this.pnlFlowSharp.Name = "pnlFlowSharp";
            this.pnlFlowSharp.Size = new System.Drawing.Size(707, 388);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 500);
            this.Controls.Add(this.nudNumLeaves);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlFlowSharp);
            this.Name = "Form1";
            this.Text = "Merkle Tree Demo";
            ((System.ComponentModel.ISupportInitialize)(this.nudNumLeaves)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlFlowSharp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudNumLeaves;
    }
}

