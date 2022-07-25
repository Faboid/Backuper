using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Backuper.UI.WPF.ViewModels {
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

        public CreateBackuperViewModel() {

        }

    }
}
