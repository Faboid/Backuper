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
using BackuperLibrary;

namespace BackuperUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void StartBackupButton_Click(object sender, RoutedEventArgs e) {
            var backuper = new Backuper(@"D:\Programming\Small Projects\Backuper\TemporaryTestFolder\From", "Test", 5);
            string message = backuper.MakeBackup();
            MessageBox.Show(message);
        }
    }
}
