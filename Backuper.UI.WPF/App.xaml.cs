﻿using Backuper.Core;
using Backuper.Core.Saves;
using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Backuper.UI.WPF {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        private readonly NavigationStore _navigationStore;
        private readonly BackuperStore _backuperStore;

        public App() {
            _navigationStore = new();

            var backuperFactory = new BackuperFactory();
            var connection = ConnectionFactory.CreateConnection(ConnectionFactory.BackupType.Memory);
            _backuperStore = new(backuperFactory, connection);
        }

        protected override void OnStartup(StartupEventArgs e) {

            var listingView = new BackuperListingViewModel(_backuperStore, _navigationStore);
            _navigationStore.CurrentViewModel = listingView;

            MainWindow = new MainWindow();
            MainWindow.DataContext = new MainViewModel(_navigationStore, MainWindow);
            MainWindow.Show();

            base.OnStartup(e);
        }

    }
}
