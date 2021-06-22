using BackuperLibrary;
using BackuperLibrary.Generic;
using System;
using System.Windows;
using System.IO;
using BackuperUI.Windows;
using BackuperLibrary.IO;
using System.Threading.Tasks;
using System.Threading;

namespace BackuperUI.Windows {
    /// <summary>
    /// Interaction logic for BackuperCreator.xaml
    /// </summary>
    public partial class BackuperEditor : Window {

        public static void Create() {
            var editor = new BackuperEditor();
            if(editor.ShowDialog() == true) {
                Backuper backuper = editor.Backuper;
                if(backuper != null) {
                    backuper.SaveToFile();
                    DarkMessageBox.Show("Operation completed.", $"The {backuper.Name} backuper has been created successfully.");
                }
            }
        }

        public static async void Edit(Backuper backuper) {

            var editor = new BackuperEditor {
                Backuper = backuper
            };
            editor.CompleteOperationButton.Content = "Save Edit";
            editor.TextBoxName.Text = backuper.Name;
            editor.TextBoxSourcePath.Text = backuper.SourcePath;
            editor.TextBoxMaxVersions.Text = backuper.MaxVersions.ToString();
            editor.TextBoxSourcePath.IsEnabled = false; //source path cannot be edited
            if(editor.ShowDialog() == true) {
                await Task.Run(() => {
                    Thread.CurrentThread.IsBackground = false;
                    backuper.Edit(editor.Backuper.Name, editor.Backuper.MaxVersions);
                    Thread.CurrentThread.IsBackground = true;
                });

                DarkMessageBox.Show("Operation completed.", $"The backuper has been edited successfully.");
            }
        }

        public Backuper Backuper { get; private set; }

        public BackuperEditor() {
            InitializeComponent();
        }

        private void CreateBackuperButton_Click(object sender, RoutedEventArgs e) {
            if(ValidateInput(out string message)) {
                Backuper = Factory.CreateBackuper(TextBoxName.Text, TextBoxSourcePath.Text, int.Parse(TextBoxMaxVersions.Text));
                DialogResult = true;
                this.Close();
            } else {
                DarkMessageBox.Show(string.Empty, message);
            }
        }

        private bool ValidateInput(out string message) {
            message = "";

            if(TextBoxName.Text == "" || TextBoxSourcePath.Text == "" || TextBoxMaxVersions.Text == "") {
                AddToMessage(ref message, "All the fields must be compiled.");
            }

            if(!PathBuilder.ValidatePath(TextBoxName.Text, out string errMessage)) {
                AddToMessage(ref message, errMessage);
            }

            if(BackupersManager.BackupersNames.Contains(TextBoxName.Text)) {
                AddToMessage(ref message, "This name is occupied by another backuper.");
            }

            if(TextBoxName.Text == PathBuilder.BinName) {
                AddToMessage(ref message, $"The name {PathBuilder.BinName} is occupied: it's used to store deleted backups.");
            }

            if(!Directory.Exists(TextBoxSourcePath.Text) && !File.Exists(TextBoxSourcePath.Text)) {
                AddToMessage(ref message, "The given path doesn't exist.");
            }

            if(!int.TryParse(TextBoxMaxVersions.Text, out int res)) {
                AddToMessage(ref message, "The max versions must be written as a numerical digit.");
            } else if(res < 2) {
                AddToMessage(ref message, "The minimum of max versions is two.");
            }

            //if the message is empty, it means no error has been found
            return message == "";
        }

        private static void AddToMessage(ref string message, string toAdd) {
            if(message == "") {
                message = $"- {toAdd}";
            } else {
                message += $"{Environment.NewLine}{Environment.NewLine}- {toAdd}";
            }
        }

    }
}
