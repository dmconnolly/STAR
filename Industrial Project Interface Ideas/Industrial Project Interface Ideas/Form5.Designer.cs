namespace Industrial_Project_Interface_Ideas
{
    partial class Form5
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
            this.ErrorTable = new System.Windows.Forms.TextBox();
            this.ErrorCodes = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ErrorTable
            // 
            this.ErrorTable.Location = new System.Drawing.Point(49, 27);
            this.ErrorTable.Multiline = true;
            this.ErrorTable.Name = "ErrorTable";
            this.ErrorTable.Size = new System.Drawing.Size(900, 230);
            this.ErrorTable.TabIndex = 0;
            this.ErrorTable.Text = "Table Of Errors";
            // 
            // ErrorCodes
            // 
            this.ErrorCodes.AutoSize = true;
            this.ErrorCodes.Location = new System.Drawing.Point(58, 321);
            this.ErrorCodes.Name = "ErrorCodes";
            this.ErrorCodes.Size = new System.Drawing.Size(135, 13);
            this.ErrorCodes.TabIndex = 1;
            this.ErrorCodes.Text = "Error Codes and Meanings:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(350, 389);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(201, 60);
            this.button1.TabIndex = 2;
            this.button1.Text = "System Traffic";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 477);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ErrorCodes);
            this.Controls.Add(this.ErrorTable);
            this.Name = "Form5";
            this.Text = "Errors";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ErrorTable;
        private System.Windows.Forms.Label ErrorCodes;
        private System.Windows.Forms.Button button1;
    }
}