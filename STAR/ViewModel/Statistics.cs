using System;
using System.Collections.Generic;
using STAR.Model;

namespace STAR.ViewModel {
    class Statistics {
        private const double ticksPerSecond = 10000000;

        private long m_packetCount;
        private long m_validPacketCount;
        private long m_invalidPacketCount;
        private long m_errorMessageCount;
        private long m_totalByteCount;
        private double m_measurementTimeSeconds;
        private double m_totalPacketsPerSecond;
        private double m_totalErrorsPerSecond;
        private double m_totalBytesPerSecond;

        // Accessors for class member variables
        public long PacketCount { get { return m_packetCount; }}
        public long ValidPacketCount { get { return ValidPacketCount; }}
        public long InvalidPacketCount { get { return m_invalidPacketCount;  }}
        public long ErrorMessageCount { get { return m_errorMessageCount;  }}
        public long TotalByteCount { get { return m_totalByteCount;  }}
        public double MeasurementTimeSeconds { get { return m_measurementTimeSeconds;  }}
        public double TotalPacketsPerSecond { get { return m_totalPacketsPerSecond; }}
        public double TotalErrorsPerSecond { get { return m_totalErrorsPerSecond; }}
        public double TotalBytesPerSecond { get { return m_totalBytesPerSecond; }}

        public Statistics() {
            Clear();
        }

        // Collect statistics from the entire data set provided
        public void collect(DateTime startTime, DateTime endTime, List<Packet> packets) {
            Clear();

            // No packets
            if(packets.Count == 0) {
                return;
            }

            // Loop through the list of packets
            foreach(Packet packet in packets) {
                if(packet is DataPacket) {
                    // Increment packet count
                    m_packetCount++;

                    // Add the packet cargo bytes to the total byte count
                    m_totalByteCount += (packet as DataPacket).CargoByteCount;

                    // Add to counters of valid or invalid packets
                    if((packet as DataPacket).Valid) {
                        m_validPacketCount++;
                    } else {
                        m_invalidPacketCount++;
                    }
                } else {
                    // Increment error count
                    m_errorMessageCount++;
                }
            }

            // Calculate some stats based on what we've worked out so far
            m_measurementTimeSeconds = (endTime.Ticks - startTime.Ticks) / ticksPerSecond;
            m_totalPacketsPerSecond = m_packetCount / m_measurementTimeSeconds;
            m_totalErrorsPerSecond = m_errorMessageCount / m_measurementTimeSeconds;
            m_totalBytesPerSecond = m_totalByteCount / m_measurementTimeSeconds;

            //Untested code for comparing data obtained to a given baseline in order to find issues with the data 
            //i.e. if there are more errors than usual, or if there is less data characters generated than usual.
            //For statistic quality: 1 is good, 2 is normal, 3 is bad.

            //private int[] m_statisticQuality;
            //private int m_parityErrorCount;
            //private int m_disconnectErrorCount;
            //private float m_percentageValid;
            //private float m_percentageInvalid;
            //private float m_percentageError;
            //private float m_parityPercentage;
            //private float m_disconnectPercentage;
            //public int[] StatisticQuality {get {return m_statisticQuality;}}
            //public int ParityError {get {return m_parityErrorCount;}}
            //public int DisconnectError {get {return m_disconnectErrorCount;}}
            //public float PercentageValid {get {return m_percentageValid;}}
            //public float PercentageInvalid {get {return m_percentageInvalid;}}
            //public float PercentageError {get {return m_percentageError;}}
            //public float ParityPercentage {get {return m_parityPercentage;}}
            //public float DisconnectPercentage {get {return m_disconnectPercentage;}}

            //m_percentageValid = (m_validPacketCount / m_packetCount) * 100; 
            //m_percentageInvalid = (m_invalidPakcetCount / m_packetCount) * 100;
            //m_percentageError = (m_errorMessageCount / m_packetCount) * 100;
            //Console.WriteLine("Valid %: {0}, Invalid %: {1}, Error %: {2}");

            //if statement for statistic quality of packets generated
            //if (m_packetCount > 100)
            //{ 
            //  m_statisticQuality[0] = 1; Console.WriteLine("A lot of packets were generated");
            //} else if (m_packetCount > 50)
            //{
            //  m_statisticQuality[0] = 2; Console.WriteLine("A reasonable amount of packets were generated");
            //} else
            //{
            //  m_statisticQuality[0] = 3;  Console.WriteLine("Not a lot of packets were generated. This is likely due to an early disconnection in the files presented.");
            //}

            //if statement for quality of packets
            //if (m_percentageValid > 80)
            //{
            //  m_statisticQuality[1] = 1; Console.WriteLine("The vast majority of the packets generated were valid.");
            //} else if (m_percentageValid > 50)
            //{
            //  m_statisticQuality[1] = 2; Console.WriteLine("At least half of the packets generated were valid, but there were some invalid ones.");
            //} else if (m_percentageInvalid > m_percentageError)
            //{
            //  m_statisticQuality[1] = 3; Console.WriteLine("There were a lot of non-valid packets generated, and the majority of these were invalid packets. This is likely a result of a high quantity of parity errors.");
            //} else
            //{
            //  m_statisticQuality[1] = 3; Console.WriteLine("There were a lot of non-valid packets generated, and the majority of those were error message packets. This is due to disconnect errors being generated early in the file.");
            //}

            //if statement for quality of the packet rate
            //if (m_totalPacketsPerSecond > 2)
            //{
            //  m_statisticQuality[2] = 1; Console.WriteLine("The rate of packet generation is very high, so there is likely no issues.");
            //}else if (m_totalPacketsPerSecond > 1)
            //{
            //  m_statisticQuality[2] = 2; Console.WriteLine("The rate of packet generation is acceptable, but there may still be a few errors.");
            //}else
            //{
            //  m_statisticQuality[2] = 3; Console.WriteLine("The rate of packet generation is low, which would suggest a lot of disconnection errors.");
            //}

            //if statement for the quality of the error rate
            //if (m_totalErrorsPerSecond > 0.5)
            //{
            //  m_statisQuality[3] = 3; Console.WriteLine("The rate of error generation is quite high. This may indicate a key issue with the system being used.");
            //}else if (m_totalErrorsPerSecond > 0.25)
            //{
            //  m_statisticQuality[3] = 2; Console.WriteLine("The rate of error generation is not too high, but not especially low either. This may indicate some minor issues with the system.")
            //}else
            //{
            //  m_statisticQuality[3] = 1; Console.WriteLine("The rate of error generation is quite low. There does not appear to be any prominent issues with your system.");
            //}

            //if statement for the data character generation rate
            //if (m_totalBytessPerSecond > 2)
            //{
            //  m_statisticQuality[4] = 1; Console.WriteLine("The rate of data character generation is very high, so there is likely no issues.");
            //}else if (m_totalBytesPerSecond > 1)
            //{
            //  m_statisticQuality[4] = 2; Console.WriteLine("The rate of data character generation is acceptable, but there may still be a few errors.");
            //}else
            //{
            //  m_statisticQuality[4] = 3; Console.WriteLine("The rate of data character generation is low, which would suggest a lot of disconnection errors.");
            //}
        }

        // Clear the statistics
        public void Clear() {
            m_packetCount = 0;
            m_validPacketCount = 0;
            m_invalidPacketCount = 0;
            m_errorMessageCount = 0;
            m_totalByteCount = 0;
            m_measurementTimeSeconds = 0;
            m_totalPacketsPerSecond = 0;
            m_totalErrorsPerSecond = 0;
            m_totalBytesPerSecond = 0;
        }

        public void print() {
            Console.WriteLine();
            Console.WriteLine("Measurement time: " + string.Format("{0:0.000}", m_measurementTimeSeconds) + " seconds");
            Console.WriteLine("Total packets: " + m_packetCount);
            Console.WriteLine("Valid Packets: " + m_validPacketCount);
            Console.WriteLine("Invalid Packets: " + m_validPacketCount);
            Console.WriteLine("Errors: " + m_errorMessageCount);
            Console.WriteLine("Total packet rate: " + string.Format("{0:0.000}", m_totalPacketsPerSecond) + " packets/second");
            Console.WriteLine("Total error rate: " + string.Format("{0:0.000}", m_totalErrorsPerSecond) + " errors/second");
            Console.WriteLine("Bytes transferred: " + m_totalByteCount);
            Console.WriteLine("Data rate: " + string.Format("{0:0.000}", m_totalBytesPerSecond) + " bytes/second");
            Console.WriteLine();
        }
    }
}
