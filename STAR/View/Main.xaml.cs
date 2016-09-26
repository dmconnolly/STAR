using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using STAR.ViewModel;
using STAR.Model;
using OxyPlot;
using System.Linq;

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

        // Sorting method for CollectionViewSource
        private SortDescription packetCollectionViewSort;

        // Filter predicate for packet collection view
        private Predicate<object> packetCollectionViewFilter;
        // Filter predicate for error collection view
        private Predicate<object> errorPacketCollectionViewFilter;

        // Array of checkboxes for port filters
        // used when updating packet view filter
        private CheckBox[] portFilterCheckbox;

        // Interface to the packet collection which is bound to the
        // UI and supports filtering, sorting and grouping. For this
        // reason we bind to this instead of the ObservableCollection
        public ICollectionView packetCollectionView;
        public ICollectionView errorCollectionView;

        public IList<DataPoint> packetRatePoints { get; set; }
        public IList<DataPoint> dataRatePoints { get; set; }
        public IList<DataPoint> errorRatePoints { get; set; }

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

            errorCollectionView = new CollectionViewSource {
                Source = packetView
            }.View;


            // Packet collection view sort description
            packetCollectionViewSort = new SortDescription(
                "TimeTicks", ListSortDirection.Ascending
            );

            // Filter predicate for packet collection view
            packetCollectionViewFilter = item => {
                PacketView pktView = item as PacketView;
                if(packetView == null) {
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

                return portFilterCheckbox[byte.Parse(pktView.EntryPort) - 1].IsChecked == true ? true : false;
            };

            errorPacketCollectionViewFilter = item => {
                PacketView pktView = item as PacketView;
                if(packetView == null) {
                    return false;
                }
                // If the checkbox for the errors is checked
                if(!pktView.PacketTypeString.Equals("Error")) {
                    return false;
                }

                return portFilterCheckbox[byte.Parse(pktView.EntryPort) - 1].IsChecked == true ? true : false;
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
        private void OpenFilesButton_Click(object sender, RoutedEventArgs e) {
            if(openFileDialog.ShowDialog() == true) {
                capture.clear();
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
            foreach(byte port in capture.PortsLoaded) {
                portFilterCheckbox[port - 1].IsEnabled = true;
                portFilterCheckbox[port - 1].IsChecked = true;
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e) {
            Help helpWindow = new Help();
            helpWindow.Show();
        }

        //Navigating to next packet
        private void btnNext_Click(object sender, RoutedEventArgs e) {
            PacketsDataGrid.SelectedIndex++;
            PacketView p = (PacketView)PacketsDataGrid.SelectedItem;
            if(p != null) {
                PacketsDataGrid.ScrollIntoView(p);
                PacketsDataGrid.UpdateLayout();
            }
        }

        //Navigating to previous packet
        private void btnPrevious_Click(object sender, RoutedEventArgs e) {
            if(PacketsDataGrid.SelectedIndex >= 1) {
                PacketsDataGrid.SelectedIndex--;
                PacketView p = (PacketView)PacketsDataGrid.SelectedItem;
                if(p != null) {
                    PacketsDataGrid.ScrollIntoView(p);
                    PacketsDataGrid.UpdateLayout();
                }
            }
        }

        //When Filter button is pressed (used for filtering different errors
        private void Button_OnClick(object sender, RoutedEventArgs e) {
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
            lblStartTime.Content = capture.StartTime.ToString("hh:mm:ss:fff");
            lblEndTime.Content = capture.EndTime.ToString("hh:mm:ss:fff");
        }

        //Method for displaying packet data when clicked on datagrid
        private void PacketsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            packetProperties.Clear();

            const int maxStringLength = 24;

            // Get current packet
            PacketView selected = PacketsDataGrid.SelectedItem as PacketView;

            if(selected == null) {
                return;
            }

            // Add packet properties to collection
            packetProperties.AddRange(new StringPair[] {
                new StringPair("Timestamp", selected.TimeString),
                new StringPair("Entry Port", selected.EntryPort),
                new StringPair("Exit Port", selected.ExitPort),
                new StringPair("Packet Type", selected.PacketTypeString),
                string.IsNullOrEmpty(selected.Message) ? new StringPair("", "") : new StringPair("Info", selected.Message)
            });

            if(selected.PacketType == typeof(ErrorPacket)) {
                // Error packets only have a message
                return;
            }

            // If statement to determine what details to display
            if(selected.PacketType == typeof(WriteCommandPacket)) {
                packetProperties.AddRange(new StringPair[] {
                    new StringPair("Dest. Path Address", selected.DestinationPathAddress),
                    new StringPair("Dest. Logical Address", selected.DestinationLogicalAddress),
                    new StringPair("Protocol ID", selected.ProtocolId),
                    new StringPair("Dest. key", selected.DestinationKey),
                    new StringPair("Source Path Address", selected.SourcePathAddress),
                    new StringPair("Source Logical Address", selected.SourceLogicalAddress),
                    new StringPair("Transaction ID", selected.TransactionId),
                    new StringPair("Extended Write Address", selected.ExtendedWriteAddress),
                    new StringPair("Write Address", selected.WriteAddress),
                    new StringPair("Data Length", selected.DataLength),
                    new StringPair("Header CRC", selected.HeaderCRC),
                });

                if(string.IsNullOrEmpty(selected.DataBytes)) {
                    packetProperties.Add(new StringPair("Data", ""));
                } else {
                    bool label = false;
                    foreach(string part in SplitString(selected.DataBytes, maxStringLength)) {
                        if(!label) {
                            label = true;
                            packetProperties.Add(new StringPair("Data", part));
                        } else {
                            packetProperties.Add(new StringPair("", part));
                        }
                    }
                }

                packetProperties.AddRange(new StringPair[] {
                    new StringPair("Data CRC", selected.DataCRC),
                    new StringPair("End of Packet Marker", selected.EndCode)
                });
            } else if(selected.PacketType == typeof(WriteReplyPacket)) {
                packetProperties.AddRange(new StringPair[] {
                    new StringPair("Source Path Address", selected.SourcePathAddress),
                    new StringPair("Source Logical Address", selected.SourceLogicalAddress),
                    new StringPair("Protocol ID", selected.ProtocolId),
                    new StringPair("Status", selected.Status),
                    new StringPair("Dest. Logical Address", selected.DestinationLogicalAddress),
                    new StringPair("Transaction ID", selected.TransactionId),
                    new StringPair("Reply CRC", selected.ReplyCRC),
                    new StringPair("End of Packet Marker", selected.EndCode)
                });
            } else if(selected.PacketType == typeof(ReadCommandPacket)) {
                packetProperties.AddRange(new StringPair[] {
                    new StringPair("Dest. Path Address", selected.DestinationPathAddress),
                    new StringPair("Dest. Logical Address", selected.DestinationLogicalAddress),
                    new StringPair("Protocol ID", selected.ProtocolId),
                    new StringPair("Dest. key", selected.DestinationKey),
                    new StringPair("Source Path Address", selected.SourcePathAddress),
                    new StringPair("Source Logical Address", selected.SourceLogicalAddress),
                    new StringPair("Transaction ID", selected.TransactionId),
                    new StringPair("Extended Read Address", selected.ExtendedReadAddress),
                    new StringPair("Read Address", selected.ReadAddress),
                    new StringPair("Data Length", selected.DataLength),
                    new StringPair("Header CRC", selected.HeaderCRC),
                    new StringPair("End of Packet Marker", selected.EndCode)
                });
            } else if(selected.PacketType == typeof(ReadReplyPacket)) {
                packetProperties.AddRange(new StringPair[] {
                    new StringPair("Source Path Address", selected.SourcePathAddress),
                    new StringPair("Source Logical Address", selected.SourceLogicalAddress),
                    new StringPair("Protocol ID", selected.ProtocolId),
                    new StringPair("Transaction ID", selected.TransactionId),
                    new StringPair("Data Length", selected.DataLength),
                    new StringPair("Header CRC", selected.HeaderCRC)
                });

                if(string.IsNullOrEmpty(selected.DataBytes)) {
                    packetProperties.Add(new StringPair("Data", ""));
                } else {
                    bool label = false;
                    foreach(string part in SplitString(selected.DataBytes, maxStringLength)) {
                        if(!label) {
                            label = true;
                            packetProperties.Add(new StringPair("Data", part));
                        } else {
                            packetProperties.Add(new StringPair("", part));
                        }
                    }
                }

                packetProperties.AddRange(new StringPair[] {
                    new StringPair("Data CRC", selected.DataCRC),
                    new StringPair("End of Packet Marker", selected.EndCode)
                });
            } else if(selected.PacketType == typeof(NonRmapPacket)) {
                packetProperties.AddRange(new StringPair[] {
                    new StringPair("Dest. Path Address", selected.DestinationPathAddress),
                    new StringPair("Dest. Logical Address", selected.DestinationLogicalAddress),
                    new StringPair("Sequence ID", selected.SequenceId),
                });

                if(string.IsNullOrEmpty(selected.Cargo)) {
                    packetProperties.Add(new StringPair("Cargo", ""));
                } else {
                    bool label = false;
                    foreach(string part in SplitString(selected.Cargo, maxStringLength)) {
                        if(!label) {
                            label = true;
                            packetProperties.Add(new StringPair("Cargo", part));
                        } else {
                            packetProperties.Add(new StringPair("", part));
                        }
                    }
                }
            }
        }


        private void drawGraphs() {
            packetRatePoints.Clear();
            BackgroundWorker[] workers = {
                new BackgroundWorker(),
                new BackgroundWorker(),
                new BackgroundWorker()
            };

            Packet[] packets = capture.Packets.OrderBy(pkt => pkt.TimeStamp.Ticks).ToArray();

            workers[0].DoWork += delegate {
                packetRatePoints.Clear();
                foreach(DataPoint point in Graphing.getGraphPoints(packets, Graphing.GraphType.PacketRate)) {
                    packetRatePoints.Add(point);
                }
                Dispatcher.InvokeAsync(() => {
                    packetRateGraph.InvalidatePlot(true);
                });
            };
            workers[0].RunWorkerAsync();

            workers[1].DoWork += delegate {
                dataRatePoints.Clear();
                foreach(DataPoint point in Graphing.getGraphPoints(packets, Graphing.GraphType.DataRate)) {
                    dataRatePoints.Add(point);
                }
                Dispatcher.InvokeAsync(() => {
                    dataRateGraph.InvalidatePlot(true);
                });
            };
            workers[1].RunWorkerAsync();

            workers[2].DoWork += delegate {
                errorRatePoints.Clear();
                foreach(DataPoint point in Graphing.getGraphPoints(packets, Graphing.GraphType.ErrorRate)) {
                    errorRatePoints.Add(point);
                }
                Dispatcher.InvokeAsync(() => {
                    errorRateGraph.InvalidatePlot(true);
                });
            };
            workers[2].RunWorkerAsync();
        }

        private void ErrorPacketsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            packetNavigation();
        }

        private void packetNavigation() {
            PacketView p = (PacketView)ErrorPacketsListView.SelectedItem;
            if(p != null) {
                PacketsDataGrid.ScrollIntoView(p);
                PacketsDataGrid.UpdateLayout();
                PacketsDataGrid.SelectedItem = p;
            }
        }

        // Function used from http://stackoverflow.com/a/4712549
        private string[] SplitString(string source, int maxLength) {
            return source
                .Where((x, i) => i % maxLength == 0)
                .Select(
                    (x, i) => new string(source
                        .Skip(i * maxLength)
                        .Take(maxLength)
                        .ToArray()))
                .ToArray();
        }

        [TestClass]
        public class MainTester {
            [TestMethod]
            public void testFileLoading() {
                bool loadTester = false;
                Assert.IsTrue(loadTester);
            }

            [TestMethod]
            public void testClearing() {
                bool clearTester = false;
                Assert.IsFalse(clearTester);
            }

            [TestMethod]
            public void testHelp() {
                bool helpTester = true;
                Assert.IsTrue(helpTester);
            }

            [TestMethod]
            public void testGraph() {
                bool graphTester = true;
                Assert.IsFalse(graphTester);
            }

            [TestMethod]
            public void testRefresh() {
                bool refreshTester = true;
                Assert.IsTrue(refreshTester);
            }

            [TestMethod]
            public void testNavigation() {
                bool navigationTester = true;
                Assert.IsFalse(navigationTester);
            }

            [TestMethod]
            public void testFilter() {
                bool filterTester = false;
                Assert.IsFalse(filterTester);
            }

            [TestMethod]
            public void testConverter() {
                bool converterTester = false;
                Assert.IsTrue(converterTester);
            }

            [TestMethod]
            public void testInvalidVariables()
            {
                bool isInvalid = true;

                Assert.IsTrue(isInvalid);
            }
    }
        }
    }

