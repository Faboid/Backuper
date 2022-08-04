using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backuper.UI.WPF.Commands {
    public class EditBackuperCommand : AsyncCommandBase {

        private readonly EditBackuperViewModel _editBackuperViewModel;
        private readonly BackuperStore _backuperStore;
        private readonly NavigationService<BackuperListingViewModel> navigatorToBackuperListingViewModel;

        public EditBackuperCommand(EditBackuperViewModel editBackuperViewModel, BackuperStore backuperStore, NavigationService<BackuperListingViewModel> navigatorToBackuperListingViewModel) {
            _editBackuperViewModel = editBackuperViewModel;
            _backuperStore = backuperStore;
            this.navigatorToBackuperListingViewModel = navigatorToBackuperListingViewModel;
        }

        protected override Task ExecuteAsync(object? parameter) {

            throw new NotImplementedException(); //todo - implement

        }
    }
}
