using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;

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

            

            //Pseudo based on tutorial added iterator and dynamic sizing instead of constant.

            //value = new listofvalues <x value, y value>();
            /*
             * for (i = 0; i < list.size(); i++)
             * value.Add(i, elem.at);
             */


            Chart graph = this.FindName("XyChart") as Chart;
            //graph.DataSource = value 
            //graph.Series[""].Xvalues = "key";
            //graph.Series[""].Yvalues = "value";
        }
    }
}
