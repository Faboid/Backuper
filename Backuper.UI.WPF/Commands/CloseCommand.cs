using System.Windows;

namespace Backuper.UI.WPF.Commands; 

public class CloseCommand : CommandBase {

    private readonly Window _window;

    public CloseCommand(Window window) {
        _window = window;
    }

    public override void Execute(object? parameter) {
        _window.Close();
    }

}
