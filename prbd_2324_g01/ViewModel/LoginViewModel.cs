﻿using Msn.ViewModel;
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
                var user = (from u in Context.Users
                        where u.Email.Equals(Email)
                        select u).First();
                
                NotifyColleagues(App.Messages.MSG_LOGIN, user);
            } else {
                AddError(nameof(Email), "creditentials not valid");
            }
        }

        public override bool Validate() {
            ClearErrors();

            var user = from u in Context.Users
                       where u.Email.Equals(Email)
                       select u.Email;

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
                                 where u.Email.Equals(Email)
                                 select u.HashedPassword).First();
            return SecretHasher.Verify(Password, hashPassword);
        }

        private void LoginAsBenoitAction() {
            var user = (from u in Context.Users
                                where u.Email.Equals("bepenelle@epfc.eu")
                                select u).First();
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }

        private void LoginAsBorisAction() {
            var user = (from u in Context.Users
                                where u.Email.Equals("boverhaegen@epfc.eu")
                                select u).First();
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }
        
        private void LoginAsXavierAction() {
            var user = (from u in Context.Users
                                where u.Email.Equals("xapigeolet@epfc.eu")
                                select u).First();
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }
        
        private void LoginAsAdminAction() {
            var user = (from u in Context.Users
                                where u.Email.Equals("admin@epfc.eu")
                                select u).First();
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }


        protected override void OnRefreshData() {
        }
    }
}