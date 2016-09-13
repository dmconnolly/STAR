using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace STAR {
    class Data {
        private const double ticksPerSecond = 10000000;

        private byte port;
        private DateTime startTime;
        private DateTime endTime;
        private List<Message> messages;

        private long packetCount = 0;
        private long validPacketCount = 0;
        private long invalidPacketCount = 0;
        private long errorMessageCount = 0;
        private double measurementTimeSeconds = 0;
        private double totalPacketsPerSecond = 0;
        private double totalErrorsPerSecond = 0;

        public Data() {
            messages = new List<Message>();
        }

        // Reads and processes a file
        public void processFile(string path) {
            // Clear anything in the message list
            messages.Clear();

            string[] lines = File.ReadLines(@path).ToArray();

            // First two lines are timestamp for measurement start and port
            startTime = Message.parseDateString(lines[0].Trim());
            port = Convert.ToByte(lines[1].Trim());

            int lineIndex = 3;
            while(lineIndex < lines.Length-1) {
                string time, startCode, endCode, bytes, errorText;

                // Skip whitespace
                while(string.IsNullOrWhiteSpace(lines[lineIndex])) {
                    ++lineIndex;
                }

                // Next line is a timestamp
                time = lines[lineIndex++].Trim();

                // If blank line is after the timestamp
                if(lineIndex >= lines.Length || string.IsNullOrWhiteSpace(lines[lineIndex])) {
                    // This is end of the file
                    endTime = Message.parseDateString(time);
                    break;
                }

                startCode = lines[lineIndex].Trim();

                if(startCode.Equals("E", StringComparison.Ordinal)) {
                    // This is an error message
                    errorText = lines[++lineIndex].Trim();
                    ErrorMessage errorMessage = new ErrorMessage(time, errorText);
                    messages.Add(errorMessage);
                } else if(startCode.Equals("P", StringComparison.Ordinal)) {
                    // This is a packet
                    bytes = lines[++lineIndex].Trim();
                    endCode = lines[++lineIndex].Trim();
                    Packet packet = new Packet(time, bytes, endCode);
                    messages.Add(packet);
                } else {
                    // Unknown start code
                    // throw error?
                }

                lineIndex++;
            }

            collectStatistics();
        }

        public void collectStatistics() {
            if(messages.Count == 0) {
                return;
            }

            foreach(Message message in messages) {
                if(message is Packet) {
                    packetCount++;
                    if(((Packet)message).Valid) {
                        validPacketCount++;
                    } else {
                        invalidPacketCount++;
                    }
                } else {
                    errorMessageCount++;
                }
            }

            measurementTimeSeconds = endTime.Subtract(startTime).Ticks / ticksPerSecond;
            totalPacketsPerSecond = packetCount / measurementTimeSeconds;
            totalErrorsPerSecond = errorMessageCount / measurementTimeSeconds;
        }

        public void printSummary() {
            Console.WriteLine();
            Console.WriteLine("Port: " + port);
            Console.WriteLine("Start time: " + Message.timeString(startTime));
            Console.WriteLine("End time: " + Message.timeString(endTime));
            Console.WriteLine("Measurement time: " + measurementTimeSeconds + " seconds");
            Console.WriteLine("Total packets: " + packetCount);
            Console.WriteLine("Valid Packets: " + validPacketCount);
            Console.WriteLine("Invalid Packets: " + invalidPacketCount);
            Console.WriteLine("Errors: " + errorMessageCount);
            Console.WriteLine("Total packet rate: " + totalPacketsPerSecond + " packets/second");
            Console.WriteLine("Total error rate: " + totalErrorsPerSecond + " errors/second");
            Console.WriteLine();
        }
    }
}
