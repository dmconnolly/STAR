using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR {
    // Read in all the data in this class
    class Data {
        // Stores the port all the data stream is coming from
        private byte port;

        //Start and end timestamps
        private DateTime startTimeStamp;
        private DateTime endTimeStamp;

        // All the data in string format
        private string fileText;

        // List of packets and error messages
        private List<Message> messages;

        public Data() {
            messages = new List<Message>();
        }

        // Reads and sorts a file
        public void processFile(string path) {
            // Clear anything in the message list
            messages.Clear();

            string[] lines = File.ReadLines(@path).ToArray();

            // First two lines are timestamp for measurement start and port
            startTimeStamp = Message.parseDateString(lines[0]);
            port = Convert.ToByte(lines[1]);

            int lineIndex = 3;
            while(lineIndex < lines.Length-1) {
                string time, startCode, endCode, bytes, errorText;

                // Skip whitespace
                while(String.IsNullOrWhiteSpace(lines[lineIndex])) {
                    ++lineIndex;
                }

                // Next line is a timestamp
                time = lines[lineIndex++].Trim();

                // If blank line is after the timestamp
                if(lineIndex >= lines.Length || String.IsNullOrWhiteSpace(lines[lineIndex])) {
                    // This is end of the file
                    endTimeStamp = Message.parseDateString(time);
                    break;
                }

                startCode = lines[lineIndex].Trim();

                if(startCode.Equals("E", StringComparison.Ordinal)) {
                    // This is an error message
                    errorText = lines[++lineIndex];
                    ErrorMessage errorMessage = new ErrorMessage(time, errorText);
                    messages.Add(errorMessage);
                } else if(startCode.Equals("P", StringComparison.Ordinal)) {
                    // This is a packet
                    bytes = lines[++lineIndex];
                    endCode = lines[++lineIndex];
                    Packet packet = new Packet(time, bytes, endCode);
                    messages.Add(packet);
                } else {
                    // Unknown start code
                    // throw error?
                }

                lineIndex++;
            }
        }

        public void printSummary() {
            if(messages.Count == 0) {
                return;
            }

            int validPacketCount = 0;
            int invalidPacketCount = 0;
            int errorMessageCount = 0;
            foreach(Message message in messages) {
                if(message is Packet) {
                    if(((Packet)message).Valid) {
                        validPacketCount++;
                    } else {
                        invalidPacketCount++;
                    }
                } else {
                    errorMessageCount++;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Port: " + port);
            Console.WriteLine("Start time: " + Message.timeString(startTimeStamp));
            Console.WriteLine("End time: " + Message.timeString(endTimeStamp));
            Console.WriteLine("Valid Packets: " + validPacketCount);
            Console.WriteLine("Invalid Packets: " + invalidPacketCount);
            Console.WriteLine("Errors: " + errorMessageCount);
            Console.WriteLine();
        }
    }
}
