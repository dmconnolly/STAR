using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Industrial_Project_Interface_Ideas
{
    public partial class VisualisationGeneration : Form
    {
        public VisualisationGeneration()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            VisualisationForm newForm = new VisualisationForm();
            newForm.Show();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TrafficDataForm newForm = new TrafficDataForm();
            newForm.Show();
            Hide();

        }
    }
}
