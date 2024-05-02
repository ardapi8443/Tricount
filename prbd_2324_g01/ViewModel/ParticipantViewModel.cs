using Msn.ViewModel;
using PRBD_Framework;
using System.Windows;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel
{
    public class ParticipantViewModel : ViewModelCommon {
        public ICommand DeleteCommand { get; set; }
        public string Name { get; private set; }
        
        private int _numberOfExpenses;
        public int NumberOfExpenses {
            get => _numberOfExpenses;
            private set {
                _numberOfExpenses = value;
                RaisePropertyChanged(nameof(NumberOfExpenses));
                RaisePropertyChanged(nameof(ExpensesDisplay));
                RaisePropertyChanged(nameof(TrashCanVisibility));
            }
        }
        
        public Visibility TrashCanVisibility => (NumberOfExpenses == 0 && !IsCreator) ? Visibility.Visible : Visibility.Collapsed;

        private bool _isCreator;

        public bool IsCreator {
            get => _isCreator;
            private set {
                _isCreator = value;
                RaisePropertyChanged(nameof(IsCreator));
                RaisePropertyChanged(nameof(CreatorStatusDisplay));
            }
        }

        public string CreatorStatusDisplay => IsCreator ? "(creator)" : "";
        public string ExpensesDisplay => (NumberOfExpenses > 0 && !IsCreator) ? $"({NumberOfExpenses} expenses)" : string.Empty;
        
        public ParticipantViewModel(string name, int numberOfExpenses, bool isCreator) {
            Name = name;
            NumberOfExpenses = numberOfExpenses;
            IsCreator = isCreator;
            
            DeleteCommand = new RelayCommand(DeleteParticipant);
        }

        private void DeleteParticipant() {
            
            Console.WriteLine("je suis dans ParticipantViewModel");
        }
    }
}