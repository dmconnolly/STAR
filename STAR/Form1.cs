using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STAR {
    public partial class Form1 : Form {
        private Data data;

        public Form1() {
            InitializeComponent();
            data = new Data();
        }

        private void menuBarOpenFile_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Capture files (*.rec)|*.rec|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = false;

            if(openFileDialog.ShowDialog() == DialogResult.OK) {
                // TODO: Use thread so as not to lock up window
                data.processFile(openFileDialog.FileName);
                data.printSummary();
            }
        }
    }
}
