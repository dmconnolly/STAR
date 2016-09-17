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

        public MainWindow() {
            InitializeComponent();

            packetView = new ObservableCollection<PacketView>();
            capture = new LinkCapture();
            openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*|Capture files (*.rec)|*.rec";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = false;
        }

        private void OpenFilesButton_Click(object sender, RoutedEventArgs e) {
            if(openFileDialog.ShowDialog() == true) {
                capture.Clear();
                packetView.Clear();

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += delegate {
                    foreach(string filename in openFileDialog.FileNames) {
                        capture.processFile(filename);
                    }
                };
                worker.RunWorkerCompleted += ParseFileWorkerCompleted;
                worker.RunWorkerAsync();
            }
        }

        private void ParseFileWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            foreach(Packet packet in capture.Packets) {
                packetView.Add(new PacketView(packet));
            }

            CollectionViewSource packetSourceList = new CollectionViewSource() {
                Source = packetView
            };
            ICollectionView packetListView = packetSourceList.View;
            packetListView.SortDescriptions.Add(
                new SortDescription("TimeTicks", ListSortDirection.Ascending)
            );
            PacketsDataGrid.ItemsSource = packetListView;
        }
    }
}
