using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel
{
    internal class LoginViewModel : ViewModelCommon
    {
        public ICommand LoginCommand { get; set; }

        private string _email;
        private string _password;

        public string Email {
            get => _email;
            set => SetProperty(ref _email, value, () => {
                Validate();
                ValidatePassword();
            });
        }
        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => {
                Validate();
                ValidatePassword();
            });
        }

        public LoginViewModel() : base() {
            LoginCommand = new RelayCommand(LoginAction,
                () => _email != null && _password != null && !HasErrors);
        }

        private void LoginAction() {
            if (Validate() && ValidateHashPassword()) {
                var user = (from u in Context.Users
                        where u.email.Equals(Email)
                        select u).First();
                
                NotifyColleagues(App.Messages.MSG_LOGIN, user);
            } else {
                AddError(nameof(Email), "creditentials not valid");
            }
        }

        public override bool Validate() {
            ClearErrors();

            var user = from u in Context.Users
                       where u.email.Equals(Email)
                       select u.email;

            if (string.IsNullOrEmpty(Email))
                AddError(nameof(Email), "required");
            else if (!Email.Contains('@') || !Email.Contains('.'))
                AddError(nameof(Email), "email not valid");
            else if (!user.Any())
                AddError(nameof(Email), "does not exist");

            return !HasErrors;
        }

        public bool ValidatePassword() {

            if (string.IsNullOrEmpty(Password))
                AddError(nameof(Password), "required");
            else if (Password.Length < 3)
                AddError(nameof(Password), "length must be >= 3");

            return !HasErrors;
        }

        private bool ValidateHashPassword() {
            var hashPassword = (from u in Context.Users
                                 where u.email.Equals(Email)
                                 select u.HashedPassword).First();
            return SecretHasher.Verify(Password, hashPassword);
        }


        protected override void OnRefreshData() {
        }
    }
}
