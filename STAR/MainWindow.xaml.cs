using Microsoft.Win32;
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
        private LinkCapture data;

        public MainWindow() {
            InitializeComponent();
            data = new LinkCapture();
        }

        private void OpenFile1Button_Click(object sender, RoutedEventArgs e) {
            OpenFile();
        }

        private void OpenFile2Button_Click(object sender, RoutedEventArgs e) {
            OpenFile();
        }

        private void OpenFile() {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Capture files (*.rec)|*.rec|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = false;


            if(openFileDialog.ShowDialog() == true) {
                // TODO: Use thread so as not to lock up window?
                data.processFile(openFileDialog.FileName);
                data.Stats.print();
            }
        }
    }
}
