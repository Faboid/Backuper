using Backuper.UI.WPF.ViewModels;
using System.IO;

namespace Backuper.UI.WPF.Commands;

public class MoveToParentFolderCommand : CommandBase {

    OpenPathDialogViewModel _openPathDialogViewModel;

    public MoveToParentFolderCommand(OpenPathDialogViewModel openPathDialogViewModel) {
        _openPathDialogViewModel = openPathDialogViewModel;
    }

    public override void Execute(object? parameter) {

        _openPathDialogViewModel.CurrentPath = Directory.GetParent(_openPathDialogViewModel.CurrentPath)?.FullName ?? _openPathDialogViewModel.CurrentPath;
    }
}
