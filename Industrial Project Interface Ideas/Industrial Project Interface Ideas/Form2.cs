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
    public partial class PacketDataForm : Form
    {
        public PacketDataForm()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            VisualisationGeneration newForm = new VisualisationGeneration();
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
