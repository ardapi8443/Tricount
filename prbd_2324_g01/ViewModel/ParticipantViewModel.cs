using Msn.ViewModel;

namespace prbd_2324_g01.ViewModel
{
    public class ParticipantViewModel : ViewModelCommon {
        public string Name { get; private set; }
        
        private int _numberOfExpenses;
        public int NumberOfExpenses {
            get => _numberOfExpenses;
            private set {
                _numberOfExpenses = value;
                RaisePropertyChanged(nameof(NumberOfExpenses));
                RaisePropertyChanged(nameof(ExpensesDisplay));
            }
        }
        
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
        }
        
    }
}