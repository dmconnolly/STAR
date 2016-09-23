using OxyPlot;
using STAR.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR.Model {
    class Graphing {
        public static List<DataPoint> getPacketRatePoints(Capture capture) {
            Packet[] packets = capture.Packets;
            List<DataPoint> points = new List<DataPoint>();
            points.Add(new DataPoint(0, 5));
            points.Add(new DataPoint(1, 7));
            points.Add(new DataPoint(2, 8));
            points.Add(new DataPoint(3, 4));
            points.Add(new DataPoint(4, 11));
            return points;
        }

        public static List<DataPoint> getDataRatePoints(Capture capture) {
            Packet[] packets = capture.Packets;
            List<DataPoint> points = new List<DataPoint>();
            points.Add(new DataPoint(0, 5));
            points.Add(new DataPoint(1, 7));
            points.Add(new DataPoint(2, 8));
            points.Add(new DataPoint(3, 4));
            points.Add(new DataPoint(4, 11));
            return points;
        }

        public static List<DataPoint> getErrorRatePoints(Capture capture) {
            Packet[] packets = capture.Packets;
            List<DataPoint> points = new List<DataPoint>();
            points.Add(new DataPoint(0, 5));
            points.Add(new DataPoint(1, 7));
            points.Add(new DataPoint(2, 8));
            points.Add(new DataPoint(3, 4));
            points.Add(new DataPoint(4, 11));
            return points;
        }
    }
}
