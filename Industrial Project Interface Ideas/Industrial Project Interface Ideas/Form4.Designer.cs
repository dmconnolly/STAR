namespace Industrial_Project_Interface_Ideas
{
    partial class VisualisationForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.VisualisationExample = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.VisualisationExample)).BeginInit();
            this.SuspendLayout();
            // 
            // VisualisationExample
            // 
            chartArea2.Name = "ChartArea1";
            this.VisualisationExample.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.VisualisationExample.Legends.Add(legend2);
            this.VisualisationExample.Location = new System.Drawing.Point(104, 35);
            this.VisualisationExample.Name = "VisualisationExample";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.VisualisationExample.Series.Add(series2);
            this.VisualisationExample.Size = new System.Drawing.Size(883, 295);
            this.VisualisationExample.TabIndex = 0;
            this.VisualisationExample.Text = "Visualisation Chart";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(109, 396);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(877, 173);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "<Insert Analysis of Data here. If Error Data is being Visualised, suggest potenti" +
    "al causes>";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(285, 338);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(216, 52);
            this.button1.TabIndex = 2;
            this.button1.Text = "Return to Traffic";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(540, 340);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(221, 49);
            this.button2.TabIndex = 3;
            this.button2.Text = "More Visualisation Options";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // VisualisationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 603);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.VisualisationExample);
            this.Name = "VisualisationForm";
            this.Text = "Visualisation";
            ((System.ComponentModel.ISupportInitialize)(this.VisualisationExample)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart VisualisationExample;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}