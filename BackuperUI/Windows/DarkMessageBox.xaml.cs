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
using System.Windows.Shapes;

namespace BackuperUI.Windows {
    /// <summary>
    /// Interaction logic for DarkMessageBox.xaml
    /// </summary>
    public partial class DarkMessageBox : Window {

        public static MessageBoxResult Show(string title, string content, MessageBoxButton buttons = MessageBoxButton.OK) {
            var window = new DarkMessageBox(title, content, buttons);
            window.ShowDialog();
            return window.Result;
        }

        public MessageBoxResult Result { private set; get; } = MessageBoxResult.Cancel;

        public DarkMessageBox(string title, string content, MessageBoxButton buttons) {
            InitializeComponent();
            this.Title = title;
            this.ContentTextBlock.Text = content;

            switch(buttons) {
                case MessageBoxButton.YesNoCancel:
                    SetYesNoButtons();
                    break;
                case MessageBoxButton.YesNo:
                    SetYesNoButtons();
                    break;
            }
        }

        private void SetYesNoButtons() {
            YesBtn.Visibility = Visibility.Visible;
            NoBtn.Visibility = Visibility.Visible;
            OKBtn.Visibility = Visibility.Hidden;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e) {
            Result = MessageBoxResult.OK;
            this.Close();
        }

        private void NoBtn_Click(object sender, RoutedEventArgs e) {
            Result = MessageBoxResult.No;
            this.Close();
        }

        private void YesBtn_Click(object sender, RoutedEventArgs e) {
            Result = MessageBoxResult.Yes;
            this.Close();
        }
    }
}
