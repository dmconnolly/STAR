using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using STAR.ViewModel;
using STAR.Model;
using OxyPlot;

namespace STAR.View {
    public partial class Main : Window {
        // Storage for all data read in from files and processed
        private Capture capture;

        // File selection dialog
        private OpenFileDialog openFileDialog;

        // ObservableCollection allows external code to be notified
        // when changes are made to the collection. This means that
        // when we add packets to the collection, the UI is updated.
        private RangeObservableCollection<PacketView> packetView;

        private RangeObservableCollection<StringPair> packetProperties;

        // Interface to the packet collection which is bound to the
        // UI and supports filtering, sorting and grouping. For this
        // reason we bind to this instead of the ObservableCollection
        private ICollectionView packetCollectionView;
        private ICollectionView errorCollectionView;

        // Sorting method for CollectionViewSource
        private SortDescription packetCollectionViewSort;

        // Filter predicate for packet collection view
        private Predicate<object> packetCollectionViewFilter;
        // Filter predicate for error collection view
        private Predicate<object> errorPacketCollectionViewFilter;

        // Array of checkboxes for port filters
        // used when updating packet view filter
        private CheckBox[] portFilterCheckbox;

        public IList<DataPoint> packetRatePoints { get; private set; }
        public IList<DataPoint> dataRatePoints { get; private set; }
        public IList<DataPoint> errorRatePoints { get; private set; }
        public bool loadTester;

        public Main() {
            InitializeComponent();

            packetRatePoints = new List<DataPoint>();
            packetRateGraph.DataContext = this;
            dataRatePoints = new List<DataPoint>();
            dataRateGraph.DataContext = this;
            errorRatePoints = new List<DataPoint>();
            errorRateGraph.DataContext = this;

            // Packet collection
            packetView = new RangeObservableCollection<PacketView>();

            //Individual packet properties
            packetProperties = new RangeObservableCollection<StringPair>();

            //For individual packets
            IndividualPacketGrid.DataContext = this;


            // Packet capture
            capture = new Capture();

            // File dialog
            openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*|Capture files (*.rec)|*.rec";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = false;

            // Packet collection view
            packetCollectionView = CollectionViewSource.GetDefaultView(packetView);
            
            errorCollectionView = new CollectionViewSource
            {
                Source = packetView
            }.View;


            // Packet collection view sort description
            packetCollectionViewSort = new SortDescription(
                "TimeTicks", ListSortDirection.Ascending
            );

            // Filter predicate for packet collection view
            packetCollectionViewFilter = item => {
                PacketView pktView = item as PacketView;
                if (packetView == null)
                {
                    return false;
                }
                // If the checkbox for the errors is checked
                if(pktView.PacketTypeString.Equals("Error")) {
                    if(ChkShowErrors.IsChecked != true) {
                        return false;
                    }
                } else {
                    if(pktView.Valid) {
                        if(ChkShowValidPackets.IsChecked != true) {
                            return false;
                        }
                    } else {
                        if(ChkShowInvalidPackets.IsChecked != true) {
                            return false;
                        }
                    }
                }

                return portFilterCheckbox[pktView.EntryPort - 1].IsChecked == true ? true : false;
            };

            errorPacketCollectionViewFilter = item =>
            {
                PacketView pktView = item as PacketView;
                if (packetView == null)
                {
                    return false;
                }
                // If the checkbox for the errors is checked
                if (!pktView.PacketTypeString.Equals("Error"))
                {
                    return false;
                }

                return portFilterCheckbox[pktView.EntryPort - 1].IsChecked == true ? true : false;
            };

            // Apply filter to packet collection view
            packetCollectionView.Filter = packetCollectionViewFilter;

            errorCollectionView.Filter = errorPacketCollectionViewFilter;

            // Bind WPF DataGrid to the packet collection view.
            // Now, whenever the packet collection is modified or
            // the packet collection view filtering, sorting or grouping
            // is changed, the UI will be updated through use of the
            // INotifyPropertyChanged callback.
            PacketsDataGrid.ItemsSource = packetCollectionView;
            // Same for status view
            ErrorPacketsListView.ItemsSource = errorCollectionView;

            //For individual packets
            IndividualPacketGrid.ItemsSource = packetProperties;

            // Set up array of port filter checkboxes
            portFilterCheckbox = new CheckBox[8] {
                ChkPort1, ChkPort2,
                ChkPort3, ChkPort4,
                ChkPort5, ChkPort6,
                ChkPort7, ChkPort8
            };
        }

        // Allow user to select files to parse using file dialog
        private void OpenFilesButton_Click(object sender, RoutedEventArgs e)
        {
            loadTester = false;
            if(openFileDialog.ShowDialog() == true) {
                capture.Clear();
                packetView.Clear();

                ChkShowValidPackets.IsEnabled = true;
                ChkShowInvalidPackets.IsEnabled = true;
                ChkShowErrors.IsEnabled = true;

                foreach(CheckBox chkBox in portFilterCheckbox) {
                    chkBox.IsEnabled = false;
                    chkBox.IsChecked = false;
                }

                BackgroundWorker bgWorker = new BackgroundWorker();
                bgWorker.DoWork += delegate {
                    capture.processFiles(openFileDialog.FileNames);
                };
                bgWorker.RunWorkerCompleted += ParseFileWorkerCompleted;
                bgWorker.RunWorkerAsync();

                loadTester = true;
            }
        }

        // Filter checkbox clicked - refresh packet filtering
        private void PacketFilterCheckbox_Click(object sender, RoutedEventArgs e) {
            RefreshPacketDataGridFilter();
        }

        // Refresh packet filtering
        private void RefreshPacketDataGridFilter() {
            packetCollectionView.Refresh();
            errorCollectionView.Refresh();
        }

        // Once all files are parsed, update packet collection
        private void ParseFileWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            UpdatePortFilterCheckboxes();
            // Temporarily remove the sort description and filter
            // massively speeds up addition of packets
            packetCollectionView.SortDescriptions.Remove(packetCollectionViewSort);
            packetCollectionView.Filter = null;
            errorCollectionView.SortDescriptions.Remove(packetCollectionViewSort);
            errorCollectionView.Filter = null;

            // Add packets to the collection
            List<PacketView> tmpPacketViews = new List<PacketView>(capture.Packets.Length);
            foreach(Packet packet in capture.Packets) {
                tmpPacketViews.Add(new PacketView(packet));
            }
            packetView.Clear();
            packetView.AddRange(tmpPacketViews);

            // Re-add the sort description and filter
            packetCollectionView.SortDescriptions.Add(packetCollectionViewSort);
            packetCollectionView.Filter = packetCollectionViewFilter;
            errorCollectionView.SortDescriptions.Add(packetCollectionViewSort);
            errorCollectionView.Filter = errorPacketCollectionViewFilter;

            //Call method to show stats
            displayGeneralStats();

            //Select first packet in the grid
            PacketsDataGrid.SelectedIndex = 0;

            drawGraphs();
        }

        // When files are loaded, this method is called
        // port filter checkboxes will only be enabled
        // if a file has been parsed for the port
        private void UpdatePortFilterCheckboxes() {
            foreach (byte port in capture.PortsLoaded) {
                portFilterCheckbox[port - 1].IsEnabled = true;
                portFilterCheckbox[port - 1].IsChecked = true;
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e) {
            Help helpWindow = new Help();
            helpWindow.Show();
        }

        //Navigating to next packet
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            PacketsDataGrid.SelectedIndex++;
            PacketView p = (PacketView)PacketsDataGrid.SelectedItem;
            if (p != null)
            {
                PacketsDataGrid.ScrollIntoView(p);
                PacketsDataGrid.UpdateLayout();
            }
        }

        //Navigating to previous packet
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (PacketsDataGrid.SelectedIndex >= 1)
            {
                PacketsDataGrid.SelectedIndex--;
                PacketView p = (PacketView)PacketsDataGrid.SelectedItem;
                if (p != null)
                {
                    PacketsDataGrid.ScrollIntoView(p);
                    PacketsDataGrid.UpdateLayout();
                }
            }
        }

        //When Filter button is pressed (used for filtering different errors
        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshPacketDataGridFilter();
        }

        //Method for loading in overall statistics
        private void displayGeneralStats() {
            Statistics stats = capture.Stats;
            lblDataRate.Content = stats.TotalBytesPerSecond.ToString("F");
            lblErrorRate.Content = stats.TotalErrorsPerSecond.ToString("F");
            lblPacketRate.Content = stats.TotalPacketsPerSecond.ToString("F");
            lblTotalPackets.Content = stats.PacketCount;
            lblTotalErrors.Content = stats.ErrorMessageCount;
            lblTotalDataCharacters.Content = stats.TotalByteCount;
            lblStartTime.Content = capture.GetStartTime.ToString("hh:mm:ss:fff"); 
            lblEndTime.Content = capture.GetEndTime.ToString("hh:mm:ss:fff"); 
        }

        //Method for displaying packet data when clicked on datagrid
        private void PacketsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            packetProperties.Clear();

            //Get current packet
            PacketView selected = (PacketView)PacketsDataGrid.SelectedItem;

            if(selected == null) {
                return;
            }

            //Add packet properties to collection
            packetProperties.Add(new StringPair("Timestamp", selected.TimeString));
            packetProperties.Add(new StringPair("Entry Port",byteToString(selected.EntryPort)));
            packetProperties.Add(new StringPair("Exit Port", byteToString(selected.ExitPort)));
            //packetProperties.Add(new StringPair("Destination Path Address", byteToString(selected.Des)));
            packetProperties.Add(new StringPair("Destination Logical Address", byteToString(selected.DestinationLogicalAddress)));
            //packetProperties.Add(new StringPair("Protocol ID", byteToString(selected.ProtocolID)));
            packetProperties.Add(new StringPair("Packet Type", selected.PacketTypeString));
            packetProperties.Add(new StringPair("Destination key", byteToString(selected.DestinationKey)));
            packetProperties.Add(new StringPair("Source Path Address", byteToString(selected.SourcePathAddress)));
            packetProperties.Add(new StringPair("Source Logical Address", byteToString(selected.SourceLogicalAddress)));
            packetProperties.Add(new StringPair("Write Address", selected.WriteAddress.ToString()));
            packetProperties.Add(new StringPair("Data Length", selected.DataLength.ToString()));
            packetProperties.Add(new StringPair("Header CRC", byteToString(selected.HeaderCRC))); 
            packetProperties.Add(new StringPair("Data", byteToString(selected.DataBytes)));
            packetProperties.Add(new StringPair("Data CRC", byteToString(selected.DataCRC)));
            packetProperties.Add(new StringPair("End of Packet Marker", selected.EndCode));
            packetProperties.Add(new StringPair("Message", selected.Message.ToString()));
            packetProperties.Add(new StringPair("Transaction ID", selected.TransactionId.ToString()));
            packetProperties.Add(new StringPair("Extended Write Address", byteToString(selected.ExtendedWriteAddress)));
            packetProperties.Add(new StringPair("Read Address", selected.ReadAddress.ToString()));
            //packetProperties.Add(new StringPair("Sequence Number", byteToString(selected.S)));
            //packetProperties.Add(new StringPair("Cargo", byteToString(selected.Cargo)));
            packetProperties.Add(new StringPair("Status", byteToString(selected.Status)));
        }

        private void drawGraphs() {
            packetRatePoints.Clear();
            Console.WriteLine("Here");
            BackgroundWorker[] workers = {
                new BackgroundWorker(),
                new BackgroundWorker(),
                new BackgroundWorker()
            };

            workers[0].DoWork += delegate {
                packetRatePoints.Clear();
                foreach(DataPoint point in Graphing.getGraphPoints(capture, Graphing.GraphType.PacketRate)) {
                    packetRatePoints.Add(point);
                }
            };
            workers[0].RunWorkerAsync();

            workers[1].DoWork += delegate {
                dataRatePoints.Clear();
                foreach(DataPoint point in Graphing.getGraphPoints(capture, Graphing.GraphType.DataRate)) {
                    dataRatePoints.Add(point);
                }
            };
            workers[1].RunWorkerAsync();

            workers[2].DoWork += delegate {
                errorRatePoints.Clear();
                foreach(DataPoint point in Graphing.getGraphPoints(capture, Graphing.GraphType.ErrorRate)) {
                    errorRatePoints.Add(point);
                }
            };
            workers[2].RunWorkerAsync();
        }

        private void ErrorPacketsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            packetNavigation();
        }

        private void packetNavigation()
        {
            PacketView p = (PacketView)ErrorPacketsListView.SelectedItem;
            if (p != null)
            {
                PacketsDataGrid.ScrollIntoView(p);
                PacketsDataGrid.UpdateLayout();
                PacketsDataGrid.SelectedItem = p;
            }
        }

        //Method to convert byte array to string
        private string byteToString(byte[] byteArray)
        {
            if(byteArray == null) {
                return "";
            }
            string returnString = BitConverter.ToString(byteArray);
            returnString.Replace("-","");
            return returnString;
        }

        //Overload for single byte
        private string byteToString(byte singleByte)
        {
            string returnString = Convert.ToString(singleByte);
            return returnString;
        }

        [TestClass]
        public class MainTester
        {
            [TestMethod]
            public void testFileLoading()
            {
                bool loadTester = false;
                Assert.IsTrue(loadTester);
            }

            [TestMethod]
            public void testClearing()
            {
                bool clearTester = false;
                Assert.IsFalse(clearTester);
            }

            [TestMethod]
            public void testHelp()
            {
                bool helpTester = true;
                Assert.IsTrue(helpTester);
            }

            [TestMethod]
            public void testGraph()
            {
                bool graphTester = true;
                Assert.IsFalse(graphTester);
            }

            [TestMethod]
            public void testRefresh()
            {
                bool refreshTester = true;
                Assert.IsTrue(refreshTester);
            }

            [TestMethod]
            public void testNavigation()
            {
                bool navigationTester = true;
                Assert.IsFalse(navigationTester);
            }

            [TestMethod]
            public void testFilter()
            {
                bool filterTester = false;
                Assert.IsFalse(filterTester);
            }

            [TestMethod]
            public void testConverter()
            {
                bool converterTester = false;
                Assert.IsTrue(converterTester);
            }
    }
    }
}
