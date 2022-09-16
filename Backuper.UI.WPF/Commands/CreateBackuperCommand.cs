using Backuper.Core.Models;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using System.Threading.Tasks;
using System.Windows;
namespace Backuper.UI.WPF.Commands;

public class CreateBackuperCommand : AsyncCommandBase {

    private readonly INotificationService _notificationService;
    private readonly CreateBackuperViewModel createBackuperViewModel;
    private readonly BackuperStore backuperStore;
    private readonly NavigationService<BackuperListingViewModel> navigatorToBackuperListingViewModel;

    public CreateBackuperCommand(CreateBackuperViewModel createBackuperViewModel, BackuperStore backuperStore, INotificationService notificationService, NavigationService<BackuperListingViewModel> navigatorToBackuperListingViewModel) {
        this.createBackuperViewModel = createBackuperViewModel;
        this.backuperStore = backuperStore;
        this.navigatorToBackuperListingViewModel = navigatorToBackuperListingViewModel;
        _notificationService = notificationService;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        BackuperInfo info = new(
                createBackuperViewModel.BackuperName,
                createBackuperViewModel.SourcePath,
                createBackuperViewModel.MaxVersions,
                createBackuperViewModel.UpdateOnBoot
            );

        var result = await backuperStore.CreateBackuperAsync(info);

        var message = result switch {
            CreateBackuperResponse.Success => $"{info.Name} has been created.",
            CreateBackuperResponse.NameIsEmpty => "The given name cannot be empty.",
            CreateBackuperResponse.NameHasIllegalCharacters => $"The name '{info.Name}' contains illegal characters.",
            CreateBackuperResponse.NameIsOccupied => $"{info.Name} is already used elsewhere.",
            CreateBackuperResponse.SourceDoesNotExist => "The given source path does not exist.",
            CreateBackuperResponse.SourceIsEmpty => "The source path cannot be empty.",
            CreateBackuperResponse.ZeroOrNegativeMaxVersions => "The max versions cannot be less than one.",
            _ => "There has been an unknown error."
        };

        _notificationService.Send(message);

        if(result == CreateBackuperResponse.Success) {
            navigatorToBackuperListingViewModel.Navigate(true);
        }

    }
}
