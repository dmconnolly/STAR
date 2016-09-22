using System.Runtime.InteropServices;
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

            Capture statisticCapture = new Capture();
            Statistics statistics = statisticCapture.Stats;
            
             
            
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
