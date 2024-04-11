using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;

namespace prbd_2324_g01.ViewModel {
    public class UserAmountViewModel : ViewModelCommon {
        private User _user;
        private double _amount;

        public User User {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        public double Amount {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        public UserAmountViewModel(User user, double amount) {
            User = user;
            Amount = amount;
            
            Console.WriteLine("User = " + User.FullName + " amount = " + Amount);
        }

        public UserAmountViewModel() {
            Console.WriteLine("coucou");
        }
    }
}