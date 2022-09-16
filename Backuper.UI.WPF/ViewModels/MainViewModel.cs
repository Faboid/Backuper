using Backuper.UI.WPF.Commands;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using System.Windows;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels;

public class MainViewModel : ViewModelBase {

    private readonly INotificationService _notificationService;
    private readonly NavigationStore _navigationStore;
    public ViewModelBase? CurrentViewModel => _navigationStore.CurrentViewModel;

    private string _message = "";
    public string Message { get => _message; set => SetAndRaise(nameof(Message), ref _message, value); }

    private bool _messageBoxVisibility = false;
    public bool MessageBoxVisibility { get => _messageBoxVisibility; set=> SetAndRaise(nameof(MessageBoxVisibility), ref _messageBoxVisibility, value); }

    public ICommand MinimizeCommand { get; }
    public ICommand ResizeCommand { get; }
    public ICommand CloseCommand { get; }

    public ICommand CloseMessageCommand { get; }

    public MainViewModel(NavigationStore navigationStore, Window mainWindow, INotificationService notificationService) {
        _navigationStore = navigationStore;
        _notificationService = notificationService;
        MinimizeCommand = new MinimizeCommand(mainWindow);
        ResizeCommand = new ResizeCommand(mainWindow);
        CloseCommand = new CloseCommand(mainWindow);
        CloseMessageCommand = new RelayCommand(CloseMessage);

        _navigationStore.CurrentViewModelChanged += OnCurrentViewChanged;
        _notificationService.NewMessage += OnNewMessage;
    }

    private void OnNewMessage(string message) {
        Message = message;
        MessageBoxVisibility = true;
    }

    private void CloseMessage() {
        Message = "";
        MessageBoxVisibility = false;
    }

    private void OnCurrentViewChanged() {
        OnPropertyChanged(nameof(CurrentViewModel));
    }

    protected override void Dispose(bool disposed) {
        _notificationService.NewMessage -= OnNewMessage;
        _navigationStore.CurrentViewModelChanged -= OnCurrentViewChanged;
        base.Dispose(disposed);
    }
}
