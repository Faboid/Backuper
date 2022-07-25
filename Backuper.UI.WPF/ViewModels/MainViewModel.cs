using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels; 

public class MainViewModel : ViewModelBase {

    public ViewModelBase CurrentViewModel { get; }

    public ICommand? MinimizeCommand { get; }
    public ICommand? ResizeCommand { get; }
    public ICommand? CloseCommand { get; }

    public MainViewModel() {
        CurrentViewModel = new CreateBackuperViewModel();
    }

}
