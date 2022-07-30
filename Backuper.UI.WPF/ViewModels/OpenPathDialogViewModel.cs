﻿using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels {
    public class OpenPathDialogViewModel : ViewModelBase {

        private readonly static string _defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        private readonly ObservableCollection<DirectoryInfo> _roots = new();
        private readonly ObservableCollection<DirectoryInfo> _directories = new();
        private readonly ObservableCollection<FileInfo> _files = new();

        public IEnumerable<DirectoryInfo> Roots => _roots;
        public IEnumerable<DirectoryInfo> Directories => _directories.Where(x => x.Name.Contains(Search, StringComparison.InvariantCultureIgnoreCase));
        public IEnumerable<FileInfo> Files => _files.Where(x => x.Name.Contains(Search, StringComparison.InvariantCultureIgnoreCase));

        private string _currentPath = _defaultPath;
        public string CurrentPath {
            get { return _currentPath; }
            set { 
                _currentPath = value;
                OnPropertyChanged(nameof(CurrentPath));
                Load();
            }
        }

        private string _search = "";
        public string Search {
            get { return _search; }
            set { 
                _search = value;
                OnPropertyChanged(nameof(Search));
                OnPropertyChanged(nameof(Directories));
                OnPropertyChanged(nameof(Files));
            }
        }

        private FileSystemInfo _selectedPath = new DirectoryInfo(_defaultPath);
        public FileSystemInfo SelectedPath {
            get { return _selectedPath; }
            set { 
                if(value == null) {
                    return;
                }

                _selectedPath = value;
                OnPropertyChanged(nameof(SelectedPath));
                OnPropertyChanged(nameof(Selected));
            }
        }

        public string Selected => _selectedPath.FullName.Trim('\\');

        public ICommand ParentFolderCommand { get; }
        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }

        public ICommand RootsDoubleClickCommand { get; }
        public ICommand DirectoriesDoubleClickCommand { get; }

        public OpenPathDialogViewModel(NavigationService<ViewModelBase> navigateToSender, Action<string> setter) {
            CancelCommand = new NavigateCommand<ViewModelBase>(navigateToSender);
            SubmitCommand = new SetValueAndReturnCommand(this, navigateToSender, setter);
            ParentFolderCommand = new MoveToParentFolderCommand(this);

            RootsDoubleClickCommand = new MoveToDirectoryCommand(this);
            DirectoriesDoubleClickCommand = new MoveToDirectoryCommand(this);

            foreach(var root in DriveInfo.GetDrives().Where(x => x.IsReady).Select(x => x.RootDirectory)) {
                _roots.Add(root);
            }

            Load();
        }

        private void Load() {

            _directories.Clear();
            _files.Clear();

            if(Directory.Exists(_currentPath)) {
                foreach(var dir in Directory.EnumerateDirectories(_currentPath).Select(x => new DirectoryInfo(x)).Where(x => HasPermissions(x))) {
                    _directories.Add(dir);
                }

                foreach(var file in Directory.EnumerateFiles(_currentPath).Select(x => new FileInfo(x)).Where(x => HasPermissions(x))) {
                    _files.Add(file);
                }

                SelectedPath = new DirectoryInfo(_currentPath);
            }

            OnPropertyChanged(nameof(Directories));
            OnPropertyChanged(nameof(Files));
        }

        private static bool HasPermissions(FileSystemInfo path) {

            try {

                if(path.Attributes.HasFlag(FileAttributes.Hidden)) {
                    return false;
                }

                if(path is FileInfo file) {
                    file.GetAccessControl();
                    return true;
                }

                if(path is DirectoryInfo directory) {
                    directory.GetAccessControl();
                    return true;
                }

            } catch(UnauthorizedAccessException) { }

            return false;
        }

    }
}
