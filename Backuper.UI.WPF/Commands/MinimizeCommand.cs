using System.Windows;

namespace Backuper.UI.WPF.Commands; 

public class MinimizeCommand : CommandBase {

    private readonly Window _window;

    public MinimizeCommand(Window window) {
        _window = window;
    }

    public override void Execute(object? parameter) {
        _window.WindowState = WindowState.Minimized;
    }
}
