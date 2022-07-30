using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels; 

public class CreateBackuperViewModel : ViewModelBase {

    private string _backuperName = "";
    public string BackuperName {
        get => _backuperName; 
        set => SetAndRaise(nameof(BackuperName), ref _backuperName, value);
    }

    private string _sourcePath = "";
    public string SourcePath {
        get => _sourcePath;
        set => SetAndRaise(nameof(SourcePath), ref _sourcePath, value);
    }

    private int _maxVersions;
    public int MaxVersions {
        get => _maxVersions;
        set => SetAndRaise(nameof(MaxVersions), ref _maxVersions, value);
    }

    private bool _updateOnBoot;
    public bool UpdateOnBoot {
        get => _updateOnBoot; 
        set => SetAndRaise(nameof(UpdateOnBoot), ref _updateOnBoot, value);
    }

    public ICommand SubmitCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand OpenPathDialogCommand { get; }

    public CreateBackuperViewModel(BackuperStore backuperStore, NavigationStore navigationStore, NavigationService<BackuperListingViewModel> navigatorToBackuperListingViewModel) {
        SubmitCommand = new CreateBackuperCommand(this, backuperStore, navigatorToBackuperListingViewModel);
        CancelCommand = new NavigateCommand<BackuperListingViewModel>(navigatorToBackuperListingViewModel);
        var navigateToSelf = new NavigationService<ViewModelBase>(navigationStore, () => this);
        OpenPathDialogCommand = new NavigateCommand<OpenPathDialogViewModel>(new(navigationStore, () => new(navigateToSelf, (s) => SourcePath = s)));
    }

}
