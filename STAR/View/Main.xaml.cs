﻿using System;
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
using System.Windows.Controls.DataVisualization.Charting;
using ListBox = System.Windows.Forms.ListBox;

namespace STAR.View {
    public partial class Main : Window {
        // Storage for all data read in from files and processed
        private Capture capture;

        // File selection dialog
        private OpenFileDialog openFileDialog;

        // ObservableCollection allows external code to be notified
        // when changes are made to the collection. This means that
        // when we add packets to the collection, the UI is updated.
        private ObservableCollection<PacketView> packetView;

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

        //Interface to the packet errors, which currently displays all errors and their types
        private ICollectionView lvpacketCollectionView;

        // Array of checkboxes for port filters
        // used when updating packet view filter
        private CheckBox[] portFilterCheckbox;

        public Main() {
            InitializeComponent();


            // Packet collection
            packetView = new ObservableCollection<PacketView>();

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
            lvpacketCollectionView = CollectionViewSource.GetDefaultView(packetView);
            
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
            foreach (Packet packet in capture.Packets) {
                packetView.Add(new PacketView(packet));
            }

            // Re-add the sort description and filter
            packetCollectionView.SortDescriptions.Add(packetCollectionViewSort);
            packetCollectionView.Filter = packetCollectionViewFilter;

            errorCollectionView.SortDescriptions.Add(packetCollectionViewSort);
            errorCollectionView.Filter = errorPacketCollectionViewFilter;

            //Call method to show stats
            displayGeneralStats();

            //Select first packet in the grid
            PacketsDataGrid.SelectedIndex = 0;
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
        }

        //Navigating to previous packet
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (PacketsDataGrid.SelectedIndex >= 1)
            {
                PacketsDataGrid.SelectedIndex--;
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
            lblDataRate.Content = stats.TotalBytesPerSecond;
            lblErrorRate.Content = stats.TotalErrorsPerSecond;
            lblPacketRate.Content = stats.TotalPacketsPerSecond;
            lblTotalPackets.Content = stats.PacketCount;
            lblTotalErrors.Content = stats.ErrorMessageCount;
            lblTotalDataCharacters.Content = stats.TotalByteCount;
            lblStartTime.Content = capture.GetStartTime.ToString("hh:mm:ss:fff"); 
            lblEndTime.Content = capture.GetEndTime.ToString("hh:mm:ss:fff"); 
        }

        //Method for displaying packet data when clicked on datagrid
        private void PacketsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   
            //Get current packet
            PacketView selected = (PacketView)PacketsDataGrid.SelectedItem;

            if (selected != null)
            {
                //Set timestamp
                lblTimestamp.Content = selected.TimeString;

                //Set source and destination
                lblPort.Content = selected.EntryPort;
                lblDestination.Content = selected.ExitPort;

                //Display different data based on packet type
                //Non-errors have protocol ID, destination path address and destination logical address
                if (selected.PacketType != typeof(ErrorPacket))
                {
                    
                }
                

            }

        }

            private void GraphGeneration()
            {

            Statistics GraphStats = capture.Stats;

            List<int> Packets_per_Min;
            List<int> Error_per_Min;
            List<int> DataChar_per_Min;
            List<DateTime> Seconds;

            //PacketsDataGrid.ItemsSource = packetCollectionView; //is the idea of what we want

            while ( !packetCollectionView.CurrentItem.Equals(null))
            {
                /*
            Packets_per_Min.Add( GraphStats.NumPacketsInMinute);
            Error_per_Min.Add(GraphStats.NumErrorsInMinute);
            DataChar_per_Min.add(GraphStats.NumDataCharactersInMinute);
            */
            }

            
            /*
            if ( != "Packet Rate") {
                //var PacketGraphData = new Tuple<int, DateTime>(Packets_per_Min, Seconds);
            }
            else if (ComboBox.SelectedValueProperty != "Error Rate") {
                //var ErrorGraphData = new Tuple<int, DateTime>(Error_per_Min, Seconds);
            }
            else if (ComboBox.SelectedValueProperty != "Data Rate") {
                //var DataCharGraphData = new Tuple<int, DateTime>(DataChar_per_Min, Seconds);
            }
            else
            {
                
            }
            */



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
    }
}
