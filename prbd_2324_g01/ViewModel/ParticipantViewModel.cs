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
        
        public string CreatorStatusDisplay => IsCreator ? "(creator)" : "";
        public string ExpensesDisplay => (NumberOfExpenses > 0 && !IsCreator) ? $"({NumberOfExpenses} expenses)" : string.Empty;
        
        public Visibility TrashCanVisibility => (NumberOfExpenses == 0 && !IsCreator) ? Visibility.Visible : Visibility.Collapsed;

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
            }
        }

        
        public ParticipantViewModel(Tricount tricount, string name, int numberOfExpenses, bool isCreator) {
            _tricount = tricount;
            Name = name;
            NumberOfExpenses = numberOfExpenses;
            IsCreator = isCreator;
            
            DeleteCommand = new RelayCommand(DeleteParticipant);
        }

        private void DeleteParticipant() {
            var q = Context.Users.FirstOrDefault(u => u.FullName == Name);

            if (q == null) {
                return;
            }
            Subscription sub = Context.Subscriptions.Find(_tricount.Id,q.UserId);
            if (sub == null) {
                return;
            }

            sub.Delete();
            NotifyColleagues(App.Messages.MSG_UPDATE_EDITVIEW, _tricount);
            Console.WriteLine(q.FullName + " est un traitre , A jamais sale eau");

        }
    }
}