using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
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
            //Get the current topic
            string helpTopic = (string)lstHelpList.SelectedItem;


            
            Console.WriteLine(helpTopic);

            //Switch rtf file based on selection
            //I don't like doing it this way either, believe me. The alternative is a nuts though
            switch (helpTopic)
            {
                case "Introduction":
                    txtHelp.Rtf = Properties.Resources.Introduction;
                    break;
                case "Main Window Overview":
                    txtHelp.Rtf = Properties.Resources.Main_Window_Overview;
                    break;
                case "Opening Files":
                    txtHelp.Rtf = Properties.Resources.Opening_Files;
                    break;
                case "Searching and Filtering":
                    txtHelp.Rtf = Properties.Resources.Searching_and_Filtering;
                    break;
                case "Further Help":
                    txtHelp.Rtf = Properties.Resources.Further_Help;
                    break;
                default:
                    break;
            }

        }
    }
}
