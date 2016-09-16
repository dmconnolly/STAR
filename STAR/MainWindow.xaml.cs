using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace STAR {
    public partial class MainWindow : Window {
        private LinkCapture capture;
        private OpenFileDialog openFileDialog;

        public MainWindow() {
            InitializeComponent();

            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*|Capture files (*.rec)|*.rec";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = false;

            capture = new LinkCapture();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e) {
            if(openFileDialog.ShowDialog() == true) {
                ((Button)sender).IsEnabled = false;
                OpenFile1Button.IsEnabled = false;
                ClearFilesButton.IsEnabled = true;
                // TODO: Use thread so as not to lock up window?
                capture.processFile(openFileDialog.FileName);
                capture.Stats.print();

                PacketsDataGrid.Items.Clear();
                for(int i=0; i<capture.Packets.Count(); i++) {
                    Packet pkt = capture.Packets[i];
                    if(pkt is DataPacket) {
                        DataPacket dPkt = (DataPacket)pkt;
                        PacketsDataGrid.Items.Add(
                            new {
                                TimeTicks = dPkt.Time,
                                TimeString = dPkt.TimeString,
                                Source = dPkt.Port,
                                Destination = "?",
                                Type = "Data",
                                Message = "Byte data here"
                            }
                        );
                    } else {
                        ErrorPacket ePkt = (ErrorPacket)pkt;
                        PacketsDataGrid.Items.Add(
                            new {
                                TimeTicks = ePkt.Time,
                                TimeString = ePkt.TimeString,
                                Source = ePkt.Port,
                                Destination = "",
                                Type = "Error",
                                Message = ePkt.Message
                            }
                        );
                    }
                }
                sortDataGrid(PacketsDataGrid);
            }
        }

        private void ClearFilesButton_Click(object sender, RoutedEventArgs e) {
            capture = new LinkCapture();
            OpenFile1Button.IsEnabled = true;
            OpenFile2Button.IsEnabled = true;
            ClearFilesButton.IsEnabled = false;
            PacketsDataGrid.Items.Clear();
        }

        // Method modified from http://stackoverflow.com/a/19952233
        private static void sortDataGrid(
                DataGrid dataGrid,
                int columnIndex=0,
                ListSortDirection sortDirection=ListSortDirection.Ascending) {

            DataGridColumn column = dataGrid.Columns[columnIndex];

            // Clear current sort descriptions
            dataGrid.Items.SortDescriptions.Clear();

            // Add the new sort description
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));

            // Apply sort
            foreach(DataGridColumn col in dataGrid.Columns) {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;

            // Refresh items to display sort
            dataGrid.Items.Refresh();
        }
    }
}
