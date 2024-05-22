using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;

namespace prbd_2324_g01.ViewModel {
    public class UserAmountCardViewModel : ViewModelCommon {
        private string _userName;
        private double _amount;
        public string FormattedAmount { get; private set; }

        public string UserName {
            get => _userName; 
            init => SetProperty(ref _userName, value);
        }

        public double Amount {
            get => _amount;
            init {
                SetProperty(ref _amount, value); 
                FormattedAmount = $"{_amount:0.00} €";
            }
        }

        public UserAmountCardViewModel(User user, double amount) {
            if (user.UserId == CurrentUser.UserId) {
                UserName = user.FullName + " (me)";
            } else {
                UserName = user.FullName;   
            }
            Amount = amount;
        }

    }
}