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

        /// <summary>
        /// Starts a new <see cref="BackuperEditor"/> window to create a new backuper.
        /// </summary>
        public static void Create(System.Windows.Threading.Dispatcher dispatcher) {
            var editor = new BackuperEditor(true);
            if(editor.ShowDialog() == true) {
                Backuper backuper = editor.Backuper;
                if(backuper != null) {
                    backuper.SaveToFile();
                    DarkMessageBox.Show("Operation completed.", $"The {backuper.Name} backuper has been created successfully.", dispatcher);
                }
            }
        }

        /// <summary>
        /// Starts a new <see cref="BackuperEditor"/> window to edit <paramref name="backuper"/>.
        /// </summary>
        /// <param name="backuper">The backuper to edit.</param>
        public static async void Edit(Backuper backuper, System.Windows.Threading.Dispatcher dispatcher) {

            var editor = new BackuperEditor(backuper.UpdateAutomatically) {
                Backuper = backuper
            };
            editor.CompleteOperationButton.Content = "Save Edit";
            editor.TextBoxName.Text = backuper.Name;
            editor.TextBoxSourcePath.Text = backuper.SourcePath;
            editor.TextBoxMaxVersions.Text = backuper.MaxVersions.ToString();
            editor.TextBoxSourcePath.IsEnabled = false; //source path cannot be edited
            if(editor.ShowDialog() == true) {
                await Task.Run(() => {
                    Settings.SetThreadForegroundHere(() => {
                        backuper.Edit(editor.Backuper.Name, editor.Backuper.MaxVersions, editor.Backuper.UpdateAutomatically);
                    });
                });

                DarkMessageBox.Show("Operation completed.", $"The backuper has been edited successfully.", dispatcher);
            }
        }

        /// <summary>
        /// Cache for the backuper to edit.
        /// </summary>
        public Backuper Backuper { get; private set; }


        private string[] comboboxChoices = new string[2] { "ON", "OFF" };
        private bool convertCombox { get => (string)ComboBoxAutoUpdate.SelectedItem == "ON"; }


        private BackuperEditor(bool comboBoxSetter) {
            InitializeComponent();
            ComboBoxAutoUpdate.ItemsSource = comboboxChoices;
            if(comboBoxSetter) {
                ComboBoxAutoUpdate.SelectedItem = comboboxChoices[0];
            } else {
                ComboBoxAutoUpdate.SelectedItem = comboboxChoices[1];
            }
        }

        private void CreateBackuperButton_Click(object sender, RoutedEventArgs e) {
            if(ValidateInput(out string message)) {
                Backuper = Factory.CreateBackuper(TextBoxName.Text, TextBoxSourcePath.Text, int.Parse(TextBoxMaxVersions.Text), convertCombox);
                DialogResult = true;
                this.Close();
            } else {
                DarkMessageBox.Show(string.Empty, message, Dispatcher);
            }
        }

        private bool ValidateInput(out string message) {
            message = "";

            if(TextBoxName.Text == "" || TextBoxSourcePath.Text == "" || TextBoxMaxVersions.Text == "" || ComboBoxAutoUpdate.SelectedItem is null) {
                AddToMessage(ref message, "All the fields must be compiled.");
            }

            if(!PathBuilder.ValidatePath(TextBoxName.Text, out string errMessage)) {
                AddToMessage(ref message, errMessage);
            }

            if(TextBoxName.Text.Contains(',') || TextBoxSourcePath.Text.Contains(',') || TextBoxMaxVersions.Text.Contains(',')) {
                AddToMessage(ref message, "Commas aren't allowed.");
            }

            if(BackupersManager.BackupersNames.Contains(TextBoxName.Text) && !(Backuper is not null && Backuper.Name == TextBoxName.Text)) {

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
