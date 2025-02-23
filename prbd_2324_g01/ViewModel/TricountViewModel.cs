﻿using Microsoft.EntityFrameworkCore;
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
            get => (!string.IsNullOrEmpty(Tricount.Description) ? Tricount.Description : "No Description");
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

            DisplayOperations();

            DisplayMap();
            
            //attribution des actions aux boutons
            EditTricount = new RelayCommand(EditTricountAction, CanDeleteTricountAction);
            DeleteTricount = new RelayCommand(DeleteTricountAction, CanDeleteTricountAction);
            
            NewOperation = new RelayCommand<OperationCardViewModel>(vm => {
                // NotifyColleagues(App.Messages.MSG_NEW_OPERATION, new Operation());
                App.ShowDialog<AddEditOperationViewModel, Operation, PridContext>(new Operation(), tricount, true);
            });    
            //on vient définir l'action au double clic sur une operation
            DisplayOperation = new RelayCommand<OperationCardViewModel>(vm => {
                // NotifyColleagues(App.Messages.MSG_DISPLAY_OPERATION, vm.Operation);
                App.ShowDialog<AddEditOperationViewModel, Operation, PridContext>(vm.Operation, tricount, false);
            });
        }

        private bool CanDeleteTricountAction() {
            return CurrentUser.UserId == Tricount.Creator || CurrentUser.Role == Role.Administrator;
        }

        private void DisplayMap() {
            Tricount tricount = Tricount.GetTricountById(Tricount.Id);
            Tricount = tricount;
            
            //on va chercher les Users ainsi que les montants lié à ceux-ci en DB
            Map = new Dictionary<User, double>();
            
            foreach (var q in Tricount.GetAllSubscribersBalance()) {
                User user = User.GetUserById(q.UserId);
                Map.Add(user, user.GetBalanceByTricount(Tricount.Id));
            }

            MapEntries = new ObservableCollection<UserAmountCardViewModel>(
                Map.Select(entry => new UserAmountCardViewModel(entry.Key, entry.Value))
            );
        }

        private void DisplayOperations() {
            //on va chercher les opérations lié au Tricount en DB
            var query = Tricount.GetAllOperation();
            Operations = new ObservableCollection<OperationCardViewModel>(query.Select(o => new OperationCardViewModel(o)));
        }

//bouton vers l'édition d'un tricount
        public void EditTricountAction() {
            NotifyColleagues(App.Messages.MSG_DISPLAY_EDIT_TRICOUNT, Tricount);
        }

//bouton vers la suppression d'un tricount
        public void DeleteTricountAction() {
            
            var confirmationDialog = new ConfirmationDialogView("tricount");
            bool? dialogResult = confirmationDialog.ShowDialog();

            if (dialogResult == true && Tricount != null) {
                Tricount.delete();
                
                NotifyColleagues(App.Messages.MSG_CLOSE_TAB,Tricount);
                NotifyColleagues(App.Messages.MSG_REFRESH_TRICOUNT,Tricount);
            }
        }

        protected override void OnRefreshData() {
            //refresh tricount
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Tricount);
            
            var tricount = Tricount.GetTricountById(Tricount.Id);
            Tricount = tricount;
            
            NotifyColleagues(App.Messages.MSG_DISPLAY_TRICOUNT, Tricount);
            
            //refresh operations
            DisplayOperations();
            
            //refresh map
            DisplayMap();
        }
    }
}
