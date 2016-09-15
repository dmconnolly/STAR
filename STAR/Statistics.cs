using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR {
    class Statistics {
        private const double ticksPerSecond = 10000000;

        private long packetCount = 0;
        private long validPacketCount = 0;
        private long invalidPacketCount = 0;
        private long errorMessageCount = 0;
        private long totalByteCount = 0;
        private double measurementTimeSeconds = 0;
        private double totalPacketsPerSecond = 0;
        private double totalErrorsPerSecond = 0;
        private double totalBytesPerSecond = 0;

        public void collect(byte port, DateTime startTime, DateTime endTime, List<Packet> packets) {
            if(packets.Count == 0) {
                return;
            }

            foreach(Packet packet in packets) {
                if(packet is DataPacket) {
                    packetCount++;
                    totalByteCount += ((DataPacket)packet).CargoBytes.Count();
                    if(((DataPacket)packet).Valid) {
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
