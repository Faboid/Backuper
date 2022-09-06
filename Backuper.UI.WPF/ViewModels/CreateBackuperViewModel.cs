using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels;

public class CreateBackuperViewModel : ViewModelBase, INotifyDataErrorInfo {

    private readonly BackuperStore _backuperStore;
    private readonly ErrorsViewModel _errorsViewModel;

    private string _backuperName = "";
    public string BackuperName {
        get => _backuperName;
        set {
            SetAndRaise(nameof(BackuperName), ref _backuperName, value);

            _errorsViewModel.ClearErrors(nameof(BackuperName));
            if(_backuperStore.BackuperExists(_backuperName)) {
                _errorsViewModel.AddError(nameof(BackuperName), "The given name is already in use.");
            }
            if(string.IsNullOrEmpty(_backuperName)) {
                _errorsViewModel.AddError(nameof(BackuperName), "The name cannot be empty.");
            }
        }
    }

    private string _sourcePath = "";
    public string SourcePath {
        get => _sourcePath;
        set {
            SetAndRaise(nameof(SourcePath), ref _sourcePath, value);

            _errorsViewModel.ClearErrors(nameof(SourcePath));
            if(!Directory.Exists(_sourcePath) && !File.Exists(_sourcePath)) {
                _errorsViewModel.AddError(nameof(SourcePath), "The given path must point to an existing file or directory.");
            }

        }
    }

    private int _maxVersions = 3;
    public int MaxVersions {
        get => _maxVersions;
        set {
            SetAndRaise(nameof(MaxVersions), ref _maxVersions, value);
            _errorsViewModel.ClearErrors(nameof(MaxVersions));
            if(_maxVersions < 1) {
                _errorsViewModel.AddError(nameof(MaxVersions), "The maximum versions cannot be less than one.");
            }

        }
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
        _backuperStore = backuperStore;
        _errorsViewModel = new ErrorsViewModel();
        _errorsViewModel.ErrorsChanged += OnErrorsChanged;
    }

    private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e) {
        ErrorsChanged?.Invoke(this, e);
        OnPropertyChanged(nameof(CanCreate));
    }

    public bool CanCreate => !HasErrors;
    public bool HasErrors => _errorsViewModel.HasErrors;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName) {
        return _errorsViewModel.GetErrors(propertyName);
    }

}
