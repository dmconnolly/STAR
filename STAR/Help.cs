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

            //Load the file with that topic as its name
            //txtHelp.LoadFile(helpTopic, RichTextBoxStreamType.RichText);


            //I don't like doing it this way either, believe me. The alternative is a nuts though
            switch (helpTopic)
            {
                case "Introduction":
                    //txtHelp.LoadFile(Properties.Resources.Introduction, RichTextBoxStreamType.RichText);
                    break;
                case "Main Window Overview":
                    //txtHelp.LoadFile(Properties.Resources.Main_Window_Overview, RichTextBoxStreamType.RichText);
                    break;
                case "Opening Files":
                    //txtHelp.LoadFile(Properties.Resources.Opening_Files, RichTextBoxStreamType.RichText);
                    break;
                case "Quitting the Program":
                    //txtHelp.LoadFile(Properties.Resources.Quitting_the_Program, RichTextBoxStreamType.RichText);
                    break;
                case "Searching and Filtering":
                    //txtHelp.LoadFile(Properties.Resources.Searching_and_Filtering, RichTextBoxStreamType.RichText);
                    break;
                default:
                    break;
            }

        }
    }
}
