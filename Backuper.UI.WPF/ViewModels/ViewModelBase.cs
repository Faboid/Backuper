using System.ComponentModel;

namespace Backuper.UI.WPF.ViewModels {
    public class ViewModelBase : INotifyPropertyChanged {
        
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string? propertyName) {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

    }
}
