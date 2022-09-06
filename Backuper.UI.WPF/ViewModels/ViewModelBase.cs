using System.ComponentModel;

namespace Backuper.UI.WPF.ViewModels;
public class ViewModelBase : INotifyPropertyChanged {

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string? propertyName) {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    protected void SetAndRaise<T>(string name, ref T prop, T value) {
        prop = value;
        OnPropertyChanged(name);
    }

}
