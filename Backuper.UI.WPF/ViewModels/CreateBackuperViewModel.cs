using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using System;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels; 

public class CreateBackuperViewModel : ViewModelBase {

    private string _backuperName = "";
    public string BackuperName {
        get { return _backuperName; }
        set { 
            _backuperName = value;
            OnPropertyChanged(nameof(BackuperName));
        }
    }

    private string _sourcePath = "";
    public string SourcePath {
        get { return _sourcePath; }
        set { 
            _sourcePath = value;
            OnPropertyChanged(nameof(SourcePath));
        }
    }

    private int _maxVersions;
    public int MaxVersions {
        get { return _maxVersions; }
        set { 
            _maxVersions = value;
            OnPropertyChanged(nameof(MaxVersions));
        }
    }

    private bool _updateOnBoot;
    public bool UpdateOnBoot {
        get { return _updateOnBoot; }
        set { 
            _updateOnBoot = value;
            OnPropertyChanged(nameof(UpdateOnBoot));
        }
    }

    public ICommand? SubmitCommand { get; }
    public ICommand? CancelCommand { get; }

    public CreateBackuperViewModel(BackuperStore backuperStore, NavigationService<BackuperListingViewModel> navigatorToBackuperListingViewModel) {
        SubmitCommand = new CreateBackuperCommand(this, backuperStore, navigatorToBackuperListingViewModel);
        CancelCommand = new NavigateCommand<BackuperListingViewModel>(navigatorToBackuperListingViewModel);
    }

}
