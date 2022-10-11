namespace NeuralNetworkToCount
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
            this.components = new System.ComponentModel.Container();
            this.pictureBox7SegmentDisplayDigits10to15 = new System.Windows.Forms.PictureBox();
            this.pictureBox7SegmentDisplayDigits0to9 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labelTraining = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7SegmentDisplayDigits10to15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7SegmentDisplayDigits0to9)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox7SegmentDisplayDigits10to15
            // 
            this.pictureBox7SegmentDisplayDigits10to15.BackColor = System.Drawing.Color.Black;
            this.pictureBox7SegmentDisplayDigits10to15.Location = new System.Drawing.Point(3, 3);
            this.pictureBox7SegmentDisplayDigits10to15.Name = "pictureBox7SegmentDisplayDigits10to15";
            this.pictureBox7SegmentDisplayDigits10to15.Size = new System.Drawing.Size(257, 340);
            this.pictureBox7SegmentDisplayDigits10to15.TabIndex = 12;
            this.pictureBox7SegmentDisplayDigits10to15.TabStop = false;
            // 
            // pictureBox7SegmentDisplayDigits0to9
            // 
            this.pictureBox7SegmentDisplayDigits0to9.BackColor = System.Drawing.Color.Black;
            this.pictureBox7SegmentDisplayDigits0to9.Location = new System.Drawing.Point(265, 3);
            this.pictureBox7SegmentDisplayDigits0to9.Name = "pictureBox7SegmentDisplayDigits0to9";
            this.pictureBox7SegmentDisplayDigits0to9.Size = new System.Drawing.Size(257, 340);
            this.pictureBox7SegmentDisplayDigits0to9.TabIndex = 13;
            this.pictureBox7SegmentDisplayDigits0to9.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // labelTraining
            // 
            this.labelTraining.AutoSize = true;
            this.labelTraining.Font = new System.Drawing.Font("Segoe UI Variable Display", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelTraining.Location = new System.Drawing.Point(80, 129);
            this.labelTraining.Name = "labelTraining";
            this.labelTraining.Size = new System.Drawing.Size(341, 64);
            this.labelTraining.TabIndex = 14;
            this.labelTraining.Text = "Training 2xAND,7x XOR Gates...\r\nPlease Wait...\r\n";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 346);
            this.Controls.Add(this.labelTraining);
            this.Controls.Add(this.pictureBox7SegmentDisplayDigits0to9);
            this.Controls.Add(this.pictureBox7SegmentDisplayDigits10to15);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Neural Network Count";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7SegmentDisplayDigits10to15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7SegmentDisplayDigits0to9)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox pictureBox7SegmentDisplayDigits10to15;
        private PictureBox pictureBox7SegmentDisplayDigits0to9;
        private System.Windows.Forms.Timer timer1;
        private Label labelTraining;
    }
}