using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<PacketView> packetView;
        private LinkCapture capture;
        private OpenFileDialog openFileDialog;
        private CollectionViewSource packetCVS;
        private SortDescription packetCVSSortDesc;
        private ICollectionView packetICV;

        public MainWindow() {
            InitializeComponent();

            packetView = new ObservableCollection<PacketView>();
            capture = new LinkCapture();
            openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*|Capture files (*.rec)|*.rec";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = false;

            packetCVS = new CollectionViewSource() {
                Source = packetView
            };
            packetCVSSortDesc = new SortDescription(
                "TimeTicks", ListSortDirection.Ascending
            );
            packetICV = packetCVS.View;
            PacketsDataGrid.ItemsSource = packetICV;
        }

        private void OpenFilesButton_Click(object sender, RoutedEventArgs e) {
            if(openFileDialog.ShowDialog() == true) {
                capture.Clear();
                packetView.Clear();

                BackgroundWorker bgWorker = new BackgroundWorker();
                bgWorker.DoWork += delegate {
                    foreach(string filename in openFileDialog.FileNames) {
                        capture.processFile(filename);
                    }
                };
                bgWorker.RunWorkerCompleted += ParseFileWorkerCompleted;
                bgWorker.RunWorkerAsync();
            }
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e) {
            RefreshPacketDataGridFilter();
        }

        private void RefreshPacketDataGridFilter() {
            packetICV.Filter = item => {
                PacketView packetView = item as PacketView;
                if(packetView == null) {
                    return false;
                }
                if(!packetView.Type.Equals("Error")) {
                    return false;
                }
                return true;
            };
        }

        private void ParseFileWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            packetCVS.SortDescriptions.Remove(packetCVSSortDesc);
            foreach(Packet packet in capture.Packets) {
                packetView.Add(new PacketView(packet));
            }
            packetCVS.SortDescriptions.Add(packetCVSSortDesc);
        }
    }
}
