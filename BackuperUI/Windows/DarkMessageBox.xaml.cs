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
    /// A dark-themed message box.
    /// </summary>
    public partial class DarkMessageBox : Window {

        /// <summary>
        /// Starts a <see cref="DarkMessageBox"/> window to show a message to the user, or to ask them for an input.
        /// </summary>
        /// <param name="title">Title of the window.</param>
        /// <param name="content">Message of the window.</param>
        /// <param name="buttons">Set buttons to ask the user for inputs.</param>
        /// <returns></returns>
        public static MessageBoxResult Show(string title, string content, System.Windows.Threading.Dispatcher dispatcher, MessageBoxButton buttons = MessageBoxButton.OK) {

            return dispatcher.Invoke(() => {
                var window = new DarkMessageBox(title, content, buttons);
                window.ShowDialog();
                return window.Result;
            });
        }

        /// <summary>
        /// The result of the user input.
        /// </summary>
        public MessageBoxResult Result { private set; get; } = MessageBoxResult.Cancel;

        private DarkMessageBox(string title, string content, MessageBoxButton buttons) {
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
