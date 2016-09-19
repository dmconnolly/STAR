using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR {
    class Statistics {
        private const double ticksPerSecond = 10000000;

        public long packetCount;
        public long validPacketCount;
        public long invalidPacketCount;
        public long errorMessageCount;
        public long totalByteCount;
        public double measurementTimeSeconds;
        public double totalPacketsPerSecond;
        public double totalErrorsPerSecond;
        public double totalBytesPerSecond;
        public string[] statisticQuality; //Used to change the colour of the statistics settings

        public Statistics() {
            Clear();
        }

        public void collect(DateTime startTime, DateTime endTime, List<Packet> packets) {
            Clear();

            if(packets.Count == 0) {
                return;
            }

            foreach(Packet packet in packets) {
                if(packet is DataPacket) {
                    packetCount++;
                    totalByteCount += (packet as DataPacket).characterCount;
                    if((packet as DataPacket).Valid) {
                        validPacketCount++;

                    } else {
                        invalidPacketCount++;
                    }
                } else {
                    errorMessageCount++;
                }
            }

            measurementTimeSeconds = (endTime.Ticks - startTime.Ticks) / ticksPerSecond;
            totalPacketsPerSecond = packetCount / measurementTimeSeconds;
            totalErrorsPerSecond = errorMessageCount / measurementTimeSeconds;
            totalBytesPerSecond = totalByteCount / measurementTimeSeconds;

                if (packetCount > 100) //Getting statistic quality of packet count
                {
                    statisticQuality[0] = "Good";
                } else if (packetCount > 50)
                {
                    statisticQuality[0] = "Normal";
                }
                else
                {
                    statisticQuality[0] = "Bad";
                }
            if (validPacketCount / packetCount > 0.75) //Getting statistic quality of valid packet count
            {
                statisticQuality[1] = "Good";
            }
            else if (validPacketCount / packetCount > 0.5)
            {
                statisticQuality[1] = "Normal";
            }
            else
            {
                statisticQuality[1] = "Bad";
            }
            if (invalidPacketCount / packetCount < 0.25) //Getting statistic quality of invalid packet count
            {
                statisticQuality[2] = "Good";
            }
            else if (invalidPacketCount / packetCount < 0.5)
            {
                statisticQuality[2] = "Normal";
            }
            else
            {
                statisticQuality[2] = "Bad";
            }
            if (errorMessageCount < 3) //Getting statistic quality of error message count
            {
                statisticQuality[3] = "Good";
            }
            else if (errorMessageCount < 7)
            {
                statisticQuality[3] = "Normal";
            }
            else
            {
                statisticQuality[3] = "Bad";
            }
            if (totalByteCount > 1000) //Getting statistic quality of byte count
            {
                statisticQuality[4] = "Good";
            }
            else if (totalByteCount > 500)
            {
                statisticQuality[4] = "Normal";
            }
            else
            {
                statisticQuality[4] = "Bad";
            }
            if (totalPacketsPerSecond > 20) //Getting statistic quality of packets per second
            {
                statisticQuality[5] = "Good";
            }
            else if (totalPacketsPerSecond > 10)
            {
                statisticQuality[5] = "Normal";
            }
            else
            {
                statisticQuality[5] = "Bad";
            }
            if (totalErrorsPerSecond < 1) //Getting statistic quality of errors per second
            {
                statisticQuality[6] = "Good";
            }
            else if (totalErrorsPerSecond < 2)
            {
                statisticQuality[6] = "Normal";
            }
            else
            {
                statisticQuality[6] = "Bad";
            }
            if (totalBytesPerSecond > 200) //Getting statistic quality of characters per second
            {
                statisticQuality[7] = "Good";
            }
            else if (totalBytesPerSecond > 100)
            {
                statisticQuality[7] = "Normal";
            }
            else
            {
                statisticQuality[7] = "Bad";
            }


        }

        public void Clear() {
            packetCount = 0;
            validPacketCount = 0;
            invalidPacketCount = 0;
            errorMessageCount = 0;
            totalByteCount = 0;
            measurementTimeSeconds = 0;
            totalPacketsPerSecond = 0;
            totalErrorsPerSecond = 0;
            totalBytesPerSecond = 0;
        }

        public void print() {
            Console.WriteLine();
            Console.WriteLine("Measurement time: " + string.Format("{0:0.000}", measurementTimeSeconds) + " seconds");
            Console.WriteLine("Total packets: " + packetCount);
            Console.WriteLine("Valid Packets: " + validPacketCount);
            Console.WriteLine("Invalid Packets: " + invalidPacketCount);
            Console.WriteLine("Errors: " + errorMessageCount);
            Console.WriteLine("Total packet rate: " + string.Format("{0:0.000}", totalPacketsPerSecond) + " packets/second");
            Console.WriteLine("Total error rate: " + string.Format("{0:0.000}", totalErrorsPerSecond) + " errors/second");
            Console.WriteLine("Bytes transferred: " + totalByteCount);
            Console.WriteLine("Data rate: " + string.Format("{0:0.000}", totalBytesPerSecond) + " bytes/second");
            Console.WriteLine();
        }
    }
}
