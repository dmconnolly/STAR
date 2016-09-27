namespace STARMAP
{
    partial class Help
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
            this.lstHelpList = new System.Windows.Forms.ListBox();
            this.txtHelp = new System.Windows.Forms.RichTextBox();
            this.picBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lstHelpList
            // 
            this.lstHelpList.FormattingEnabled = true;
            this.lstHelpList.Items.AddRange(new object[] {
            "Introduction",
            "Opening Files",
            "Main Window Overview",
            "Searching and Filtering",
            "Statistics Window Overview",
            "Further Help"});
            this.lstHelpList.Location = new System.Drawing.Point(13, 13);
            this.lstHelpList.Name = "lstHelpList";
            this.lstHelpList.Size = new System.Drawing.Size(217, 290);
            this.lstHelpList.TabIndex = 0;
            this.lstHelpList.SelectedIndexChanged += new System.EventHandler(this.lstHelpList_SelectedIndexChanged);
            // 
            // txtHelp
            // 
            this.txtHelp.Location = new System.Drawing.Point(274, 13);
            this.txtHelp.Name = "txtHelp";
            this.txtHelp.Size = new System.Drawing.Size(392, 236);
            this.txtHelp.TabIndex = 1;
            this.txtHelp.Text = "";
            // 
            // picBox
            // 
            this.picBox.Location = new System.Drawing.Point(274, 268);
            this.picBox.Name = "picBox";
            this.picBox.Size = new System.Drawing.Size(392, 180);
            this.picBox.TabIndex = 2;
            this.picBox.TabStop = false;
            // 
            // Help
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 460);
            this.Controls.Add(this.picBox);
            this.Controls.Add(this.txtHelp);
            this.Controls.Add(this.lstHelpList);
            this.Name = "Help";
            this.Text = "Help";
            this.Load += new System.EventHandler(this.Help_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstHelpList;
        private System.Windows.Forms.RichTextBox txtHelp;
        private System.Windows.Forms.PictureBox picBox;
    }
}