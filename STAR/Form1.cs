using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STAR
{
    public partial class Form1 : Form
    {
        private Data data;

        public Form1()
        {
            InitializeComponent();
            //Reading data
            data = new Data();
        }

        private void btnReadData_Click(object sender, EventArgs e)
        {
            data.ReadFile();
        }
    }
}
