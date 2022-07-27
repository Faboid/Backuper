using Backuper.Core;
using Backuper.Core.Saves;
using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
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
            var connection = ConnectionFactory.CreateConnection(ConnectionFactory.BackupType.Text);
            _backuperStore = new(backuperFactory, connection);
        }

        protected override void OnStartup(StartupEventArgs e) {

            _navigationStore.CurrentViewModel = CreateBackuperListingViewModel();

            MainWindow = new MainWindow();
            MainWindow.DataContext = new MainViewModel(_navigationStore, MainWindow);
            MainWindow.Show();

            base.OnStartup(e);
        }

        private BackuperListingViewModel CreateBackuperListingViewModel() {
            return BackuperListingViewModel.LoadViewModel(_backuperStore, new(_navigationStore, CreateCreateBackuperViewModel));
        }

        private CreateBackuperViewModel CreateCreateBackuperViewModel() {
            return new(_backuperStore, new(_navigationStore, CreateBackuperListingViewModel));
        }

    }
}
