using System.Windows;

namespace Backuper.UI.WPF.Commands; 

public class ResizeCommand : CommandBase {

    private readonly Window _window;

    public ResizeCommand(Window window) {
        _window = window;
    }

    public override void Execute(object? parameter) {
        _window.WindowState = _window.WindowState switch {
            WindowState.Maximized => WindowState.Normal,
            _ => WindowState.Maximized,
        };
    }

}
