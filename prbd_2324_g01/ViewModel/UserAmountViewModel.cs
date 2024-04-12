using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;

namespace prbd_2324_g01.ViewModel {
    public class UserAmountViewModel : ViewModelCommon {
        private string _userName;
        private double _amount;

        public string UserName {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        public double Amount {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        public UserAmountViewModel(User user, double amount) {
            if (user.UserId == CurrentUser.UserId) {
                UserName = user.FullName + " (me)";
            } else {
                UserName = user.FullName;   
            }
            Amount = amount;
            
            // Console.WriteLine("User = " + UserName + " amount = " + Amount);
        }

    }
}