namespace Lanedirt.ModelTester
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
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.lblStep1 = new System.Windows.Forms.Label();
            this.pictureBoxOriginal = new System.Windows.Forms.PictureBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lblStep2 = new System.Windows.Forms.Label();
            this.pictureBoxBoundingBoxes = new System.Windows.Forms.PictureBox();
            this.richTextBoxJson = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBoundingBoxes)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoadImage
            // 
            this.btnLoadImage.Location = new System.Drawing.Point(199, 37);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(128, 23);
            this.btnLoadImage.TabIndex = 0;
            this.btnLoadImage.Text = "Load image";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
            // 
            // lblStep1
            // 
            this.lblStep1.AutoSize = true;
            this.lblStep1.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblStep1.Location = new System.Drawing.Point(179, 9);
            this.lblStep1.Name = "lblStep1";
            this.lblStep1.Size = new System.Drawing.Size(171, 25);
            this.lblStep1.TabIndex = 1;
            this.lblStep1.Text = "Load a JPEG image";
            // 
            // pictureBoxOriginal
            // 
            this.pictureBoxOriginal.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBoxOriginal.Location = new System.Drawing.Point(25, 80);
            this.pictureBoxOriginal.Name = "pictureBoxOriginal";
            this.pictureBoxOriginal.Size = new System.Drawing.Size(487, 237);
            this.pictureBoxOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxOriginal.TabIndex = 2;
            this.pictureBoxOriginal.TabStop = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // lblStep2
            // 
            this.lblStep2.AutoSize = true;
            this.lblStep2.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblStep2.Location = new System.Drawing.Point(119, 338);
            this.lblStep2.Name = "lblStep2";
            this.lblStep2.Size = new System.Drawing.Size(316, 25);
            this.lblStep2.TabIndex = 4;
            this.lblStep2.Text = "Detected objects (annotated image):";
            // 
            // pictureBoxBoundingBoxes
            // 
            this.pictureBoxBoundingBoxes.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBoxBoundingBoxes.Location = new System.Drawing.Point(29, 382);
            this.pictureBoxBoundingBoxes.Name = "pictureBoxBoundingBoxes";
            this.pictureBoxBoundingBoxes.Size = new System.Drawing.Size(483, 237);
            this.pictureBoxBoundingBoxes.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxBoundingBoxes.TabIndex = 5;
            this.pictureBoxBoundingBoxes.TabStop = false;
            // 
            // richTextBoxJson
            // 
            this.richTextBoxJson.Location = new System.Drawing.Point(547, 80);
            this.richTextBoxJson.Name = "richTextBoxJson";
            this.richTextBoxJson.Size = new System.Drawing.Size(423, 539);
            this.richTextBoxJson.TabIndex = 6;
            this.richTextBoxJson.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(678, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 25);
            this.label1.TabIndex = 7;
            this.label1.Text = "Detected objects (JSON):";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(995, 641);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBoxJson);
            this.Controls.Add(this.pictureBoxBoundingBoxes);
            this.Controls.Add(this.lblStep2);
            this.Controls.Add(this.pictureBoxOriginal);
            this.Controls.Add(this.lblStep1);
            this.Controls.Add(this.btnLoadImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Lanedirt ONNX Model Tester";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBoundingBoxes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnLoadImage;
        private Label lblStep1;
        private PictureBox pictureBoxOriginal;
        private OpenFileDialog openFileDialog;
        private Label lblStep2;
        private PictureBox pictureBoxBoundingBoxes;
        private RichTextBox richTextBoxJson;
        private Label label1;
    }
}