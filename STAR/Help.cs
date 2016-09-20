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
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        private void Help_Load(object sender, EventArgs e)
        {
            //Set introduction as first selection
            //Be warned! If the introduction doesn't load when this form opens, it's because of this.
            //Just in case :)
            lstHelpList.SelectedIndex = 0;
        }

        private void lstHelpList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var helpFile = @"";


        }
    }
}
