using BackuperLibrary;
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
using System.IO;

namespace BackuperUI {
    /// <summary>
    /// Interaction logic for BackuperCreator.xaml
    /// </summary>
    public partial class BackuperCreator : Window {

        public event EventHandler OnClose;

        public BackuperCreator() {
            InitializeComponent();
        }

        private void CreateBackuperButton_Click(object sender, RoutedEventArgs e) {
            if(ValidateInput(out string message)) {
                Backuper backuper = new Backuper(TextBoxSourcePath.Text, TextBoxName.Text, int.Parse(TextBoxMaxVersions.Text));
                BackupersHandler.AddBackuper(backuper);
                MessageBox.Show($"The {TextBoxName.Text} backuper has been created successfully.");
                this.Close();
                OnClose?.Invoke(this, null);
            } else {
                MessageBox.Show(message);
            }
        }

        private bool ValidateInput(out string message) {
            message = "";

            if(TextBoxName.Text == "" || TextBoxSourcePath.Text == "" || TextBoxMaxVersions.Text == "") {
                message = AddToMessage(message, "All the fields must be compiled.");
            }

            if(!PathBuilder.ValidatePath(TextBoxName.Text, out string errMessage)) {
                message = AddToMessage(message, errMessage);
            }

            if(!Directory.Exists(TextBoxSourcePath.Text)) {
                message = AddToMessage(message, "The path doesn't exist.");
            }

            if(!int.TryParse(TextBoxMaxVersions.Text, out int res)) {
                message = AddToMessage(message, "The max versions must be written as a numerical digit.");
            } else if(res < 2) {
                message = AddToMessage(message, "The minimum of max versions is two.");
            }

            //if the message is empty, it means no error has been found
            return message == "";
        }

        private string AddToMessage(string message, string toAdd) {
            if(message == "") {
                return toAdd;
            } else {
                return $"{message}{Environment.NewLine}{Environment.NewLine}{toAdd}";
            }
        }

        #region WindowBasicFunctionality
        private void CloseWindowButton_Click(object sender, RoutedEventArgs e) {
            this.Close();

        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e) {
            this.Maximize();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) {
            this.Minimize();
        }
        #endregion
    }
}
