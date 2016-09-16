using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace Industrial_Project_Interface_Ideas
{
    public partial class TrafficDataForm : Form
    {

        public TrafficDataForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            PacketDataForm newForm = new PacketDataForm();
            newForm.Show();
            Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ErrorForm newForm = new ErrorForm();
            newForm.Show();
            Hide();
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            int counter = 0;
            string line = null;
            DateTime startTime = new DateTime();
            string[] packet = new string[1234567890];
            DateTime endTime = new DateTime();
            string[] errorLocation = new string[1234567890];
            bool[] packetInFilter = new bool[1234567890];
            string[] filteredPackets = new string[1234567890];
            bool[][] errorType = new bool[3][];
            bool errorPresent = false;
            int pointer = 0;
            string packetSelection = null;


            for (pointer = 0; pointer < errorLocation.Length; pointer++)
            {
                for (int innerPointer = 0; innerPointer < 3; innerPointer++)
                {
                    errorType[innerPointer][pointer] = false;
                }
            }

            // Read the file and display it line by line.
            StreamReader file = new StreamReader("c:\\test.txt");
            line = file.ReadLine();
            do
            {
                while ((line = file.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    counter++;
                    if (line == "E")
                    {
                        errorType[0][counter] = true;
                        errorLocation[counter] = line;
                        errorPresent = true;
                    } else if (line == "EEP")
                    {
                        errorType[1][counter] = true;
                        errorLocation[counter] = line;
                        errorPresent = true;
                    } else if (line == "None")
                    {
                        errorType[2][counter] = true;
                        errorLocation[counter] = line;
                        errorPresent = true;
                    }
                }
            } while (errorPresent == false);

            file.Close();


            // Suspend the screen.
            Console.ReadLine();

            //Searches for every error of a given type
            bool[] errorSelection = new bool[3];
            errorSelection[0] = true;
            errorSelection[1] = true;
            errorSelection[2] = true;
            packetSelection = "E";

            if (errorSelection[0] == true)
            {
                for (pointer = 0; pointer < errorLocation.Length; pointer++)
                {
                    if (errorType[0][pointer] == true)
                    {
                        packetInFilter[pointer] = true;
                    }
                }
            }
            if (errorSelection[1] == true)
            {
                for (pointer = 0; pointer < errorLocation.Length; pointer++)
                {
                    if (errorType[1][pointer] == true)
                    {
                        packetInFilter[pointer] = true;
                    }
                }
            }
            if (errorSelection[2] == true)
            {
                for (pointer = 0; pointer < errorLocation.Length; pointer++)
                {
                    if (errorType[2][pointer] == true)
                    {
                        packetInFilter[pointer] = true;
                    }
                }
            }
            

            counter = 0;

            for (pointer = 0; pointer < packetInFilter.Length; pointer++)
            {
                if (packet[pointer] == packetSelection)
                {
                    packetInFilter[pointer] = true;
                }
                if (packetInFilter[pointer] == true)
                {
                    filteredPackets[counter] = packet[pointer];
                    counter++;
                }
            }
            for (pointer = 0; pointer < filteredPackets.Length; pointer++)
            {
                while (filteredPackets[pointer] != null)
                {
                    Console.Write(filteredPackets[pointer]);
                }
            }
        }
    }
}
