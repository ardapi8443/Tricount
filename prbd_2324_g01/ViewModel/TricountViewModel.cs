using Msn.ViewModel;
using prbd_2324_g01.Model;
using prbd_2324_g01.View;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel {
    internal class TricountViewModel: ViewModelCommon {
        private Tricount _tricount;
        private ObservableCollection<OperationCardViewModel> _operations;
        private Dictionary<User, double> _map;
        private ObservableCollection<UserAmountCardViewModel> _mapEntries;
        
        //boutons
        public ICommand NewOperation { get; set; }
        public ICommand EditTricount { get; set; }
        public ICommand DeleteTricount { get; set; }
        public ICommand DisplayOperation { get; set; }
        
        //get && set des variables utilisées dans la vue
        private Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value);
        }

        public ObservableCollection<OperationCardViewModel> Operations {
            get => _operations;
            set => SetProperty(ref _operations, value);
        }

        public string Title {
            get => Tricount.Title;
            set => SetProperty(Tricount.Title, value, Tricount, (m, v) => m.Title = v);
        }

        public string Description {
            get => (!string.IsNullOrEmpty(Tricount.Description) ? Tricount.Description : "pas de description");
            set => SetProperty(Tricount.Description, value, Tricount, (m, v) => m.Description = v);
        }

        public string Creation {
            get => $"Created by {Tricount.CreatorFromTricount.FullName} on {Tricount.CreatedAt.ToShortDateString()}";
        }

        public Dictionary<User, double> Map {
            get => _map;
            set => SetProperty(ref _map, value);
        }
        public ObservableCollection<UserAmountCardViewModel> MapEntries {
            get => _mapEntries; 
            set => SetProperty(ref _mapEntries, value); 
        }

        //constructeur
        public TricountViewModel(Tricount tricount) {
            Tricount = tricount;
            Console.WriteLine(Tricount.Title);
            
            //on va chercher les opérations lié au Tricount en DB
            var query = from o in PridContext.Context.Operations
                                        where o.TricountId == Tricount.Id
                                        select o;
            query = query.OrderByDescending(x => x.OperationDate)
                .ThenBy(x => x.Title);
            Operations = new ObservableCollection<OperationCardViewModel>(query.Select(o => new OperationCardViewModel(o)));

            //on va chercher les Users ainsi que les montants lié à ceux-ci en DB
//manque les users qui n'ont pas fait d'opérations mais qui ont souscrit au tricount
            Map = new Dictionary<User, double>();
            var operations = from o in PridContext.Context.Operations
                where o.TricountId == Tricount.Id
                group o by o.UserId into g
                orderby g.Key
                select new {
                    UserId = g.Key,
                    Amount = g.Sum(x => x.Amount)
                };
            var query2 = from user in tricount.Subscribers
                join op in operations on user.UserId equals op.UserId into operationDetails
                from subOp in operationDetails.DefaultIfEmpty()
                orderby user.FullName
                select new {
                    UserId = user.UserId,
                    Amount = subOp != null ? subOp.Amount : 0.00 
                };
            
            foreach (var q in query2) {
                User user = User.GetUserById(q.UserId);
                Map.Add(user, Tricount.ConnectedUserBal(user));
            }

            MapEntries = new ObservableCollection<UserAmountCardViewModel>(
                Map.Select(entry => new UserAmountCardViewModel(entry.Key, entry.Value))
            );
            
            //attribution des actions aux boutons
            EditTricount = new RelayCommand(EditTricountAction);
            DeleteTricount = new RelayCommand(DeleteTricountAction);
            
            NewOperation = new RelayCommand<OperationCardViewModel>(vm => {
                NotifyColleagues(App.Messages.MSG_NEW_OPERATION, vm.Operation);
            });    
            //on vient définir l'action au double clic sur une operation
            DisplayOperation = new RelayCommand<OperationCardViewModel>(vm => {
                NotifyColleagues(App.Messages.MSG_DISPLAY_OPERATION, vm.Operation);
            });
        }

        public void NewOperationAction() {
            //DialogWindowBase dialog = new AddEditOperationView();
            //dialog.ShowDialog();
        }

//bouton vers l'édition d'un tricount
        public void EditTricountAction() {
            Console.WriteLine("je suis dans TricountViewModel");
            NotifyColleagues(App.Messages.MSG_DISPLAY_EDIT_TRICOUNT, Tricount);
        }

//bouton vers la suppression d'un tricount
        public void DeleteTricountAction() {
            
            var confirmationDialog = new ConfirmationDialogView();
            bool? dialogResult = confirmationDialog.ShowDialog();

            if (dialogResult == true && Tricount != null) {
                Console.WriteLine("Vous venez de delete le Tricount : " + Tricount.Title );
                Tricount.delete();
                NotifyColleagues(App.Messages.MSG_CLOSE_TAB,Tricount);
                NotifyColleagues(App.Messages.MSG_REFRESH_TRICOUNT,Tricount);
            }
            
            Console.WriteLine("je suis dans TricountViewModel");
        }
    }
}
