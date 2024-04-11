using Microsoft.EntityFrameworkCore;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace prbd_2324_g01.ViewModel
{
    internal class TricountViewModel: ViewModelCommon {
        private Tricount _tricount;
        private ObservableCollection<OperationCardViewModel> _operations;
        private Dictionary<User, double> _map;
        private ObservableCollection<UserAmountViewModel> _mapEntries;

        public ICommand NewOperation { get; set; }
        
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
        public ObservableCollection<UserAmountViewModel> MapEntries {
            get => _mapEntries; 
            set => SetProperty(ref _mapEntries, value); 
        }

        //constructeur
//doit recevoir le Tricount au double clique dans l'UC list_tricounts
        public TricountViewModel() {
// à remplacer par le Tricount recu en paramètre
            Tricount = Tricount.GetTricountById(4);
            
            //on va chercher les opérations lié au Tricount en DB
            var query = from o in PridContext.Context.Operations
                                        where o.TricountId == Tricount.Id
                                        select o;
            query = query.OrderByDescending(x => x.OperationDate);
            Operations = new ObservableCollection<OperationCardViewModel>(query.Select(o => new OperationCardViewModel(o)));

            //on va chercher les Users ainsi que les montants lié à ceux-ci en DB
//manque les users qui n'ont pas fait d'opérations mais qui ont souscrit au tricount
            Map = new Dictionary<User, double>();
            var query2 = from o in PridContext.Context.Operations
                                            where o.TricountId == Tricount.Id
                                            group o by o.UserId into g
                                            orderby g.Key
                                            select new { UserId = g.Key, Amount = g.Sum(x => x.Amount) };
            foreach (var q in query2) {
                User user = User.GetUserById(q.UserId);
                Map.Add(user, Tricount.ConnectedUserBal(user));
            }

            MapEntries = new ObservableCollection<UserAmountViewModel>(
                Map.Select(entry => new UserAmountViewModel(entry.Key, entry.Value))
            );
            
            //attribution des actions aux boutons
            NewOperation = new RelayCommand(NewOperationAction);
        }

        public void NewOperationAction() {
//bouton vers une nouvelle opération
            Console.WriteLine("je suis dans TricountViewModel");
        }

    }
}
