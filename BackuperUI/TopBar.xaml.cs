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

namespace BackuperUI {
    /// <summary>
    /// Interaction logic for TopBar.xaml
    /// </summary>
    public partial class TopBar : UserControl {
        //todo - intergrate this usercontrol into the windows to centralize the style
        
        public TopBar() {
            InitializeComponent();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) {
            (sender as Window).WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e) {
            if((sender as Window).WindowState == WindowState.Normal) {

                (sender as Window).WindowState = WindowState.Maximized;

            } else if((sender as Window).WindowState == WindowState.Maximized) {

                (sender as Window).WindowState = WindowState.Normal;
            }
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e) {
            (sender as Window).Close();
        }
    }
}
