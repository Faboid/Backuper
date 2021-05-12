using BackuperLibrary;
using BackuperLibrary.Generic;
using System;
using System.Windows;
using System.IO;

namespace BackuperUI {
    /// <summary>
    /// Interaction logic for BackuperCreator.xaml
    /// </summary>
    public partial class BackuperEditor : Window {

        public static void Create() {
            BackuperEditor editor = new BackuperEditor();
            if(editor.ShowDialog() == true) {
                Backuper backuper = editor.Backuper;
                if(backuper != null) {
                    BackupersHandler.AddBackuper(backuper);
                    MessageBox.Show($"The {backuper.Name} backuper has been created successfully.");
                }
            }
        }

        public static void Edit(Backuper backuper) {
            BackuperEditor editor = new BackuperEditor();
            editor.Backuper = backuper;
            editor.CompleteOperationButton.Content = "Save Edit";
            editor.TextBoxName.Text = backuper.Name;
            editor.TextBoxSourcePath.Text = backuper.From;
            editor.TextBoxMaxVersions.Text = backuper.MaxVersions.ToString();
            editor.TextBoxSourcePath.IsEnabled = false; //source path cannot be edited
            if(editor.ShowDialog() == true) {
                backuper.ModifyBackuper(editor.Backuper.Name, editor.Backuper.MaxVersions);
                MessageBox.Show($"The backuper has been edited successfully.");
            }
        }

        public Backuper Backuper { get; private set; }

        public BackuperEditor() {
            InitializeComponent();
        }

        private void CreateBackuperButton_Click(object sender, RoutedEventArgs e) {
            if(ValidateInput(out string message)) {
                Backuper = new Backuper(TextBoxName.Text, TextBoxSourcePath.Text, int.Parse(TextBoxMaxVersions.Text));
                DialogResult = true;
                this.Close();
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
            DialogResult = false;
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
