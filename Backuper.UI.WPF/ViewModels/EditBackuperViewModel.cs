using Backuper.Core.Models;
using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels;

public class EditBackuperViewModel : ViewModelBase, INotifyDataErrorInfo {

    private readonly BackuperInfo _original;
    private readonly BackuperStore _backuperStore;
    private readonly ErrorsViewModel _errorsViewModel = new();

    private string _backuperName = "";
    public string BackuperName {
        get => _backuperName;
        set {
            SetAndRaise(nameof(BackuperName), ref _backuperName, value);

            _errorsViewModel.ClearErrors(nameof(BackuperName));
            if(value != _original.Name && _backuperStore.BackuperExists(_backuperName)) {
                _errorsViewModel.AddError(nameof(BackuperName), "The given name is already in use.");
            }
            if(string.IsNullOrEmpty(_backuperName)) {
                _errorsViewModel.AddError(nameof(BackuperName), "The name cannot be empty.");
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

    public bool CanEdit => !HasErrors;

    public ICommand SubmitCommand { get; }
    public ICommand CancelCommand { get; }

    public EditBackuperViewModel(BackuperInfo backuperInfo, BackuperStore backuperStore, NavigationService<BackuperListingViewModel> navigatorToBackuperListingViewModel) {
        _backuperStore = backuperStore;
        _original = backuperInfo;

        BackuperName = backuperInfo.Name;
        MaxVersions = backuperInfo.MaxVersions;
        UpdateOnBoot = backuperInfo.UpdateOnBoot;

        SubmitCommand = new EditBackuperCommand(this, backuperStore, navigatorToBackuperListingViewModel);
        CancelCommand = new NavigateCommand<BackuperListingViewModel>(navigatorToBackuperListingViewModel);
        _errorsViewModel.ErrorsChanged += OnErrorsChanged;
    }

    private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e) {
        ErrorsChanged?.Invoke(this, e);
        OnPropertyChanged(nameof(CanEdit));
    }

    public bool HasErrors => _errorsViewModel.HasErrors;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName) {
        return _errorsViewModel.GetErrors(propertyName);
    }
}
