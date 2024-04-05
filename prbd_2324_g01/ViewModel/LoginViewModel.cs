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
            set => SetProperty(ref _email, value, () => Validate());
        }
        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => ValidatePassword());
        }

        public LoginViewModel() : base() {
            LoginCommand = new RelayCommand(LoginAction,
                () => _email != null && _password != null && !HasErrors);
        }

        private void LoginAction() {
            if (Validate() && ValidateHashPassword()) {
                var user = from u in Context.Users
                           where u.email.Equals(Email)
                           select u;
        //surement un meilleur moyen de selectionner le User
                NotifyColleagues(App.Messages.MSG_LOGIN, user.FirstOrDefault());
            } else {
                AddError(nameof(Email), "creditentials not valid");
            }
        }

        public override bool Validate() {
            //comment ne retirer les erreurs que d'un champs particulier ?
            //      (nameof(Email) => ne fonctionne pas
            ClearErrors();

            var user = from u in Context.Users
                       where u.email.Equals(Email)
                       select u.email;

            if (string.IsNullOrEmpty(Email))
                AddError(nameof(Email), "required");
            else if (!Email.Contains("@") || !Email.Contains("."))
                AddError(nameof(Email), "email not valid");
            else if (!user.Any())
                AddError(nameof(Email), "does not exist");

            return !HasErrors;
        }

        public bool ValidatePassword() {
            ClearErrors();

            if (string.IsNullOrEmpty(Password))
                AddError(nameof(Password), "required");
            else if (Password.Length < 3)
                AddError(nameof(Password), "length must be >= 3");

            // doit on vérifier que le password est valide selon les règles métiers ou s'il est valide pour ce user ?
            //      (pas super safe déjà que l'on indique si un mail existe ou pas...)

            return !HasErrors;
        }

        private bool ValidateHashPassword() {
            var hashPassword = from u in Context.Users
                                 where u.email.Equals(Email)
                                 select u.HashedPassword;

            //surement un meilleur moyen de sélectionner directement le string HashedPassword de l'objet User
            return SecretHasher.Verify(Password, hashPassword.ToList().FirstOrDefault(""));
        }


        protected override void OnRefreshData() {
        }
    }
}
