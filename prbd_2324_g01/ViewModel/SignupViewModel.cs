﻿using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel {
    public class SignupViewModel : ViewModelCommon {
        private string _pseudo;
        private string _password;
        private string _confirmpassword;
        private string _email;
        
        public string Pseudo {
            get => _pseudo;
            set => SetProperty(ref _pseudo, value, () => {
                ValidePseudo();
                Validate();
                ValidePseudo();
                ValidatePassword();
                ValidateConfirmPassword();
            });
        }

        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => {
                ValidePseudo();
                Validate();
                ValidePseudo();
                ValidatePassword();
                ValidateConfirmPassword();
            });
        }

        public string ConfirmPassword {
            get => _confirmpassword;
            set => SetProperty(ref _confirmpassword, value, () => {
                ValidePseudo();
                Validate();
                ValidePseudo();
                ValidatePassword();
                ValidateConfirmPassword();
            });
        }
        

        public string Email {
            get => _email;
            set => SetProperty(ref _email, value, () => {
                ValidePseudo();
                Validate();
                ValidePseudo();
                ValidatePassword();
                ValidateConfirmPassword();
            });
        }
        
        public ICommand SignupCommand { get; }
        public ICommand CancelCommand { get; }

        public SignupViewModel() : base() {
            SignupCommand = new RelayCommand(SignUpAction, CanSignUp);
            CancelCommand = new RelayCommand(CancelAction);
        }

        private void SignUpAction() {
            var newUser = new User {
                FullName = Pseudo, 
                Email = Email,
                HashedPassword = SecretHasher.Hash(Password) 
            };

            newUser.AddToContext();
            
            Context.SaveChanges();
            NotifyColleagues(App.Messages.MSG_LOGIN, newUser);
        }

        private bool CanSignUp() {
            return !HasErrors && !string.IsNullOrWhiteSpace(ConfirmPassword);
        }

        public override void CancelAction() {
            NotifyColleagues(App.Messages.MSG_LOGOUT);
        }
        
        //peut rester ici non ?
        private bool ValidateConfirmPassword() {
            
            if (!ConfirmPassword.Equals(Password))
                AddError(nameof(ConfirmPassword), "Passwords do not match.");
            
            return !HasErrors;
        }
        
        public override bool Validate() {
            ClearErrors();

            string validateMail = User.ValidateEmailForSignup(Email);
            if (validateMail != null)
                AddError(nameof(Email), validateMail);
            
            return !HasErrors;
        }

        public bool ValidatePassword() {
            string validatePassword = User.ValidatePasswordForSignup(Password);
            
            if (validatePassword != null) {
                AddError(nameof(Password), validatePassword);
            }
            
            return !HasErrors;
        }

        public bool ValidePseudo() {
            string validatePseudo = User.ValidatePseudo(Pseudo);
            
            if (validatePseudo != null) {
                AddError(nameof(Pseudo), validatePseudo);  
            }
            
            return !HasErrors;
        }
    }

}