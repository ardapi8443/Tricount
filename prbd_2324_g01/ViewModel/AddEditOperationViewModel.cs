using prbd_2324_g01.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Operation = prbd_2324_g01.Model.Operation;

namespace prbd_2324_g01.ViewModel {
    public class AddEditOperationViewModel : DialogViewModelBase<Operation, PridContext> {
        public ICommand ApplyTemplate { get; set; }
        public ICommand SaveTemplate { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand AddOperation { get; set; }

        private Operation _operation;
        private string _title;
        private double _amount;
        private User _selectedUser;
        //combobox !!
        private ObservableCollectionFast<User> _users = new ObservableCollectionFast<User>();
        private DateTime _date;
        private ObservableCollectionFast<Template> _templates;

        public Operation Operation {
            get => _operation;
            set => SetProperty(ref _operation, value);
        }
        
        public string Title {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public double Amount {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public ObservableCollectionFast<User> Users {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public User SelectedUser {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value); 
        }
        
        public DateTime Date {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public ObservableCollectionFast<Template> Templates {
            get => _templates;
            set => SetProperty(ref _templates, value);
        }

        public AddEditOperationViewModel(Operation operation, bool isNew) {
//voir new member
            Operation = operation;
            Title = isNew ? "" : operation.Title;
            Amount = isNew ? 0.00 : operation.Amount;
            Date = isNew ? DateTime.Today : operation.OperationDate;
            SelectedUser = isNew ? App.CurrentUser : operation.Initiator;
            var query = from o in PridContext.Context.Operations
                where o.OperationId == operation.OperationId
                select o.Users;
            var user = query.First();
            foreach (var row in user) {
                Users.Add(row);
            }

            Cancel = new RelayCommand(CancelAction);
        }

        public override void CancelAction() {
            DialogResult = null;
        }
    }
}