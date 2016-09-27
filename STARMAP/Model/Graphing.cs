using System.Collections.Generic;
using OxyPlot;
using System;

namespace STARMAP.Model {
    class Graphing {
        public enum GraphType { PacketRate, DataRate, ErrorRate }

        public static List<DataPoint> getGraphPoints(Packet[] packets, GraphType type) {
            switch(type) {
                case GraphType.PacketRate:
                    return getPacketRatePoints(packets);
                case GraphType.DataRate:
                    return getDataRatePoints(packets);
                case GraphType.ErrorRate:
                    return getErrorRatePoints(packets);
                default:
                    return null;
            }
        }

        private static List<DataPoint> getPacketRatePoints(Packet[] packets) {
            List<DataPoint> points = new List<DataPoint>();

            int dataPoints = 20;

            if(packets.Length == 0) {
                return points;
            }

            int numPackets = 0;
            int prevPackets = 0;
            DateTime startTime = packets[0].TimeStamp;
            DateTime lastTime = startTime;
            double timeDiffReq = ((packets[packets.Length-1].TimeStamp - startTime).TotalSeconds)/dataPoints;

            for(int i=0; i<packets.Length; ++i) {
                DateTime curTime = packets[i].TimeStamp;
                double timeGap = (curTime - lastTime).TotalSeconds;
                ++numPackets;
                if(timeGap >= timeDiffReq) {
                    points.Add(new DataPoint((curTime-startTime).TotalSeconds-(timeGap/2), (((double)numPackets+prevPackets)/2)/(timeGap)));
                    points.Add(new DataPoint((curTime-startTime).TotalSeconds, numPackets/timeGap));
                    prevPackets = numPackets;
                    lastTime = curTime;
                    numPackets = 0;
                }
            }

            return points;
        }

        private static List<DataPoint> getDataRatePoints(Packet[] packets) {
            List<DataPoint> points = new List<DataPoint>();

            List<DataPacket> dataPackets = new List<DataPacket>(packets.Length);
            foreach(Packet packet in packets) {
                if(packet is DataPacket) {
                    dataPackets.Add(packet as DataPacket);
                }
            }

            int dataPoints = 20;

            if(dataPackets.Count < 2) {
                return points;
            }

            long numBytes = dataPackets[0].CargoByteCount;
            long prevBytes = 0;
            DateTime startTime = dataPackets[0].TimeStamp;
            DateTime lastTime = startTime;
            double timeDiffReq = ((dataPackets[dataPackets.Count-1].TimeStamp - startTime).TotalSeconds)/dataPoints;

            for(int i=0; i<dataPackets.Count; ++i) {
                DateTime curTime = dataPackets[i].TimeStamp;
                double timeGap = (curTime - lastTime).TotalSeconds;
                numBytes += dataPackets[i].CargoByteCount;
                if(timeGap >= timeDiffReq) {
                    points.Add(new DataPoint((curTime-startTime).TotalSeconds-(timeGap/2), (((double)numBytes+prevBytes)/2)/(timeGap)));
                    points.Add(new DataPoint((curTime-startTime).TotalSeconds, numBytes/timeGap));
                    prevBytes = numBytes;
                    lastTime = curTime;
                    numBytes = 0;
                }
            }

            return points;
        }

        private static List<DataPoint> getErrorRatePoints(Packet[] packets) {
            List<DataPoint> points = new List<DataPoint>();

            int dataPoints = 10;

            if(packets.Length < 2) {
                return points;
            }

            int numErrors = 0;
            int prevErrors = 0;
            DateTime startTime = packets[0].TimeStamp;
            DateTime lastTime = startTime;
            double timeDiffReq = ((packets[packets.Length-1].TimeStamp - startTime).TotalSeconds)/dataPoints;

            for(int i=0; i<packets.Length; ++i) {
                DateTime curTime = packets[i].TimeStamp;
                double timeGap = (curTime - lastTime).TotalSeconds;
                if(packets[i] is ErrorPacket || (packets[i] is DataPacket && ((DataPacket)packets[i]).Error)) {
                    ++numErrors;
                }
                if(timeGap >= timeDiffReq || i==(packets.Length-1)) {
                    points.Add(new DataPoint((curTime-startTime).TotalSeconds-(timeGap/2), (((double)numErrors+prevErrors)/2)/(timeGap)));
                    points.Add(new DataPoint((curTime-startTime).TotalSeconds, numErrors/timeGap));
                    prevErrors = numErrors;
                    lastTime = curTime;
                    numErrors = 0;
                }
            }

            return points;
        }

        // Modified from http://stackoverflow.com/a/3354915
        private static double unixTime(DateTime date) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}
