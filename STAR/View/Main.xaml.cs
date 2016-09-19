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

namespace STAR.View {
    public partial class Main : Window {
        private LinkCapture capture;
        private OpenFileDialog openFileDialog;
        private ObservableCollection<PacketView> packetView;
        private SortDescription packetCVSSortDesc;
        private CollectionViewSource packetCVS;
        private ICollectionView packetICV;
        private CheckBox[] portFilterCheckbox;

        public Main() {
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

            portFilterCheckbox = new CheckBox[8] {
                ChkPort1, ChkPort2,
                ChkPort3, ChkPort4,
                ChkPort5, ChkPort6,
                ChkPort7, ChkPort8
            };
        }

        private void OpenFilesButton_Click(object sender, RoutedEventArgs e) {
            if(openFileDialog.ShowDialog() == true) {
                capture.Clear();
                packetView.Clear();

                foreach(CheckBox chkBox in portFilterCheckbox) {
                    chkBox.IsEnabled = false;
                    chkBox.IsChecked = false;
                }

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
            UpdatePortFilterCheckboxes();
            packetCVS.SortDescriptions.Remove(packetCVSSortDesc);
            foreach(Packet packet in capture.Packets) {
                packetView.Add(new PacketView(packet));
            }
            packetCVS.SortDescriptions.Add(packetCVSSortDesc);
        }

        private void UpdatePortFilterCheckboxes() {
            foreach(byte port in capture.PortsLoaded) {
                portFilterCheckbox[port].IsEnabled = true;
                portFilterCheckbox[port].IsChecked = true;
            }
        }
    }
}
