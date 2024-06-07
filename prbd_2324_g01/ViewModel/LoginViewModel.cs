using Microsoft.EntityFrameworkCore;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel
{
    internal class LoginViewModel : ViewModelCommon {
        public ICommand LoginCommand { get; set; }
        public ICommand SignupCommand { get; set; }
        public ICommand LoginAsBenoit { get; set; }
        public ICommand LoginAsBoris { get; set; }
        public ICommand LoginAsXavier { get; set; }
        public ICommand LoginAsAdmin { get; set; }
        public List<User> AllUser { get; set; }

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
            AllUser = User.GetAllUser();
            LoginAsBenoit = new RelayCommand(LoginAsBenoitAction);
            LoginAsBoris = new RelayCommand(LoginAsBorisAction);
            LoginAsXavier = new RelayCommand(LoginAsXavierAction);
            LoginAsAdmin = new RelayCommand(LoginAsAdminAction);
            SignupCommand = new RelayCommand(SignupAction);
        }

        private void SignupAction() {
            NotifyColleagues(App.Messages.MSG_SIGNUP, new User());
        }

        private void LoginAction() {
            if (Validate() && ValidateHashPassword()) {
                
                User user = User.GetUserByMail(Email);
                
                NotifyColleagues(App.Messages.MSG_LOGIN, user);
            } else {
                AddError(nameof(Email), "creditentials not valid");
            }
        }

        public override bool Validate() {
            ClearErrors();

            string validateMail = User.ValidateEmailForLogin(Email);
            if (validateMail != null)
                AddError(nameof(Email), validateMail);
            
            return !HasErrors;
        }

        public bool ValidatePassword() {

            string validatePassword = User.ValidatePasswordForLogin(Password);
            if (validatePassword != null)
                AddError(nameof(Password), validatePassword);

            return !HasErrors;
        }

        private bool ValidateHashPassword() {
            string hashPassword = User.GetHashPassword(Email);
            return SecretHasher.Verify(Password, hashPassword);
        }

        private void LoginAsBenoitAction() {
   
            var user = getUserFromList("bepenelle@epfc.eu");
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }

        private void LoginAsBorisAction() {

            var user = getUserFromList("boverhaegen@epfc.eu");
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }
        
        private void LoginAsXavierAction() {
    
            var user = getUserFromList("xapigeolet@epfc.eu");
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }
        
        private void LoginAsAdminAction() {
 
            var user = getUserFromList("admin@epfc.eu");
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }

        private User getUserFromList(String mail) {
            User res = new User();
            foreach (User u in AllUser) {
                if (u.Email == mail) {
                    res = u;
                    break;
                }
            }

            return res;
        }

        protected override void OnRefreshData() {
        }
    }
}