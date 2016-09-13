using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR
{
    class Data
    {
        //Read in all the data in this class

        //Stores the port all the data stream is coming from
        int port;

        //Start and end timestamps
        DateTime startTimeStamp;
        DateTime endTimeStamp;

        //All the data in string format
        string fileText;

        //Reads the entirety of the data file into a string
        public void ReadFile(string path)
        {
            FileStream fs = new FileStream(@path, FileMode.Open, FileAccess.Read);
            
            using (var streamReader = new StreamReader(fs, Encoding.UTF8))
            {
                fileText = streamReader.ReadToEnd();
            }

            Console.WriteLine(fileText);
        }

    }

    //Create list of messages 
    //Read in first line and store in startTimeStamp
    //Read in port and store in port
    //ERROR packets to go packet, error messages go to message->errormessage
    //Note: Packets are of type message

    //Ignore blank lines
    //Store temp variables - timestamp
    //If it's a packet 'P' - get cargo on next line, then EOP or EEP or None
    //If it's E, that's an ErrorMessage. Error message on next line (e.g. Disconnect)
    //Add all the data to the message list, with the temp variables passed into the constructor
    //If the line was disconnect, skip a line then store endTimeStamp




}
