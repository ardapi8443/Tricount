using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System.Windows;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel
{
    public class ParticipantViewModel : ViewModelCommon {
        public ICommand DeleteCommand { get; set; }
        public string Name { get; private set; }
        public User User { get; private set; }
        public string CreatorStatusDisplay => IsCreator ? "(creator)" : "";
        public string ExpensesDisplay => (NumberOfExpenses > 0 && !IsCreator) ? $"({NumberOfExpenses} expenses)" : string.Empty;
        
        public Visibility TrashCanVisibility => (NumberOfExpenses == 0 && !IsCreator && IsVisible && (User.Role != Role.Administrator || CurrentUser.Role == Role.Administrator)) ? Visibility.Visible : Visibility.Collapsed;

        private bool _isVisible;
        public bool IsVisible {
            get => _isVisible;
            set {
                if (_isVisible != value) {
                    _isVisible = value;
                    RaisePropertyChanged(nameof(IsVisible));
                    RaisePropertyChanged(nameof(TrashCanVisibility)); 
                }
            }
        }
        
        private bool _isCreator;
        
        private int _numberOfExpenses;
        
        private readonly Tricount _tricount;
        
        public int NumberOfExpenses {
            get => _numberOfExpenses;
            private set {
                _numberOfExpenses = value;
                RaisePropertyChanged(nameof(NumberOfExpenses));
                RaisePropertyChanged(nameof(ExpensesDisplay));
                RaisePropertyChanged(nameof(TrashCanVisibility));
            }
        }
        
        public bool IsCreator {
            get => _isCreator;
            private set {
                _isCreator = value;
                RaisePropertyChanged(nameof(IsCreator));
                RaisePropertyChanged(nameof(CreatorStatusDisplay));
                RaisePropertyChanged(nameof(TrashCanVisibility));
            }
        }

        
        public ParticipantViewModel(Tricount tricount, User User, int numberOfExpenses, bool isCreator) {
            _tricount = tricount;
            Name = User.FullName;
            IsVisible = true;
            this.User = User;
            NumberOfExpenses = numberOfExpenses;
            IsCreator = isCreator;
            DeleteCommand = new RelayCommand(DeleteParticipant);

            Register<bool>(App.Messages.MODIFED_TEMPLATE, OnModifiedTemplate);
        }

        private void DeleteParticipant() {
            NotifyColleagues(App.Messages.MSG_DEL_PARTICIPANT, this);
        }
        
        private void OnModifiedTemplate(bool isModifiedTemplate) {
            IsVisible = !isModifiedTemplate;
        }
    }
}