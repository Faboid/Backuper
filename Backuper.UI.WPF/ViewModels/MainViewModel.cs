using Backuper.UI.WPF.Commands;
using System.Windows;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels; 

public class MainViewModel : ViewModelBase {

    public ViewModelBase CurrentViewModel { get; }

    public ICommand? MinimizeCommand { get; }
    public ICommand? ResizeCommand { get; }
    public ICommand? CloseCommand { get; }

    public MainViewModel(Window mainWindow) {
        CurrentViewModel = new CreateBackuperViewModel();
        MinimizeCommand = new MinimizeCommand(mainWindow);
        ResizeCommand = new ResizeCommand(mainWindow);
        CloseCommand = new CloseCommand(mainWindow);
    }

}
