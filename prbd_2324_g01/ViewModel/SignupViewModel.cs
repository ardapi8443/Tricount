using Msn.ViewModel;
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

        private static readonly string PasswordPattern =
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}";
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
            // Context.Users.Add(newUser);
            Context.Add(newUser);
            Context.SaveChanges();
            NotifyColleagues(App.Messages.MSG_LOGIN, newUser);
        }

        private bool CanSignUp() {
            return !HasErrors && !string.IsNullOrWhiteSpace(ConfirmPassword);
        }

        public override void CancelAction() {
            NotifyColleagues(App.Messages.MSG_LOGOUT);
        }
        
        private bool ValidateConfirmPassword() {
            
            if (!ConfirmPassword.Equals(Password))
                AddError(nameof(ConfirmPassword), "Passwords do not match.");
            
            return !HasErrors;
        }


        public bool ValidatePasswordComplexity(string password) {
            return new Regex(PasswordPattern).IsMatch(password);
        }
        
        public override bool Validate() {
            ClearErrors();

            bool emailExists = Context.Users.Any(u => u.Email.Equals(Email));

            if (string.IsNullOrEmpty(Email))
                AddError(nameof(Email), "required");
            else if (!Email.Contains('@') || !Email.Contains('.'))
                AddError(nameof(Email), "email not valid");
            else if (emailExists)
                AddError(nameof(Email), "Already used");
            
            return !HasErrors;
        }

        public bool ValidatePassword() {
            
            if (string.IsNullOrWhiteSpace(Password)) {
                AddError(nameof(Password), "Password is required.");
            } else if (!ValidatePasswordComplexity(Password))
                AddError(nameof(Password), "Password does not meet complexity requirements.");
            else if (Password.Length < 3)
                AddError(nameof(Pseudo), "length must be >= 3");
            
            return !HasErrors;
        }

        public bool ValidePseudo() {
            
            bool existingUser = Context.Users.Any(u => u.FullName.Equals(Pseudo));
            
            if (string.IsNullOrEmpty(Pseudo))
                AddError(nameof(Pseudo), "required");
            else if (Pseudo.Length < 3 )
                AddError(nameof(Pseudo), "length must be >= 3");
            else if (existingUser)
                AddError(nameof(Pseudo), "This Pseudo is not available");
            
            return !HasErrors;
        }
    }

}