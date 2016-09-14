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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(578, 374);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(216, 52);
            this.button1.TabIndex = 2;
            this.button1.Text = "System Traffic";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(264, 377);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(221, 49);
            this.button2.TabIndex = 3;
            this.button2.Text = "Back";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(347, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Title of Graph:";
            // 
            // VisualisationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 603);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.VisualisationExample);
            this.Name = "VisualisationForm";
            this.Text = "Graphical Analysis";
            ((System.ComponentModel.ISupportInitialize)(this.VisualisationExample)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart VisualisationExample;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
    }
}