﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
            }
        }

        private void ClearFilesButton_Click(object sender, RoutedEventArgs e) {
            capture = new LinkCapture();
            OpenFile1Button.IsEnabled = true;
            OpenFile2Button.IsEnabled = true;
            ClearFilesButton.IsEnabled = false;
        }
    }
}
