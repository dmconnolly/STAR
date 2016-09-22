using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using STAR.ViewModel;


namespace STAR.View
{
    /// <summary>
    /// Interaction logic for GraphView.xaml
    /// </summary>
    public partial class GraphView : Page
    {
        public GraphView()
        {
            InitializeComponent();

            Statistics stats = new Statistics();

            //int[] errorPacketsPerSecond;
            //int[] dataCharactersPerSecond;
            //int[] packetsPerSecond;
            //DateTime[] secondsForPacket;
            //bool matchedSecond;


            //Pseudo based on tutorial added iterator and dynamic sizing instead of constant.

            //value = new listofvalues <x value, y value>();

            /* Method for generating the statistics for generating a graph
             * foreach (PacketView packetView in Main.PacketCollectionView) {
             *      if (graphInput == "ErrorRate") {
             *           int pointer = 0;
             *           matchedSecond = false;
             *           if (packetView.PacketTypeString.Equals("Error") {
             *              do {
             *                   if (packetView.m_timeInSeconds = secondForPacket[pointer]) {
             *                      errorPacketsPerSecond[pointer]++;
             *                      matchedSecond = true;
             *                   }
             *              } while ((secondForPacket[pointer] != null) && (matchedSecond != true))
             *              if (matchedSecond == false) {
             *                  secondForPacket[pointer] = packetView.m_timeInSeconds;
             *              }
             *          }
             *      }else if (graphInput == "DataRate") {
             *          int pointer = 0;
             *           matchedSecond = false;
             *              do {
             *                   if (packetView.m_timeInSeconds = secondForPacket[pointer]) {
             *                      dataCharactersPerSecond[pointer] = dataCharactersPerSecond[pointer] + packetView.DataBytes.Length();
             *                      matchedSecond = true;
             *                   }
             *              } while ((secondForPacket[pointer] != null) && (matchedSecond != true))
             *              if (matchedSecond == false) {
             *                  secondForPacket[pointer] = packetView.m_timeInSeconds;
             *              }
             *      }else if (graphInput == "PacketRate") {
             *          int pointer = 0;
             *           matchedSecond = false;
             *              do {
             *                   if (packetView.m_timeInSeconds = secondForPacket[pointer]) {
             *                      packetsPerSecond[pointer]++;
             *                      matchedSecond = true;
             *                   }
             *              } while ((secondForPacket[pointer] != null) && (matchedSecond != true))
             *              if (matchedSecond == false) {
             *                  secondForPacket[pointer] = packetView.m_timeInSeconds;
             *              }
             *     }
             * for (i = 0; i < list.size(); i++)
             * value.Add(i, elem.at);
             */

            //Note: Pull statistics from Capture Class, not from Statistics
            Chart graph = this.FindName("XyChart") as Chart;
            //if (graphInput = "Error Rate") {
            //graph.Series["Error Rate"].Points[0].Title = "Number of Seconds";
            //graph.Series["Error Rate"].Points[1].Title = "Number of Errors";
            //}
            //graph.DataSource = value 
            //graph.Series[""].Xvalues = "key";
            //graph.Series[""].Yvalues = "value";


        }
    }
}
