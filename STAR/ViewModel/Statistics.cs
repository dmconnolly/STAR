using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR {
    class Statistics {
        private const double ticksPerSecond = 10000000;

        private long packetCount;
        private long validPacketCount;
        private long invalidPacketCount;
        private long errorMessageCount;
        private long totalByteCount;
        private double measurementTimeSeconds;
        private double totalPacketsPerSecond;
        private double totalErrorsPerSecond;
        private double totalBytesPerSecond;

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
