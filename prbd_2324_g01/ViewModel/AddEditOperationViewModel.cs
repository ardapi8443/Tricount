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

        private Tricount _tricount { get; set; }
        private bool _isNew { get; set; }
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

        public AddEditOperationViewModel(Operation operation, Tricount tricount, bool isNew) {
//voir new member
            _isNew = isNew;
            Operation = operation;
            if (isNew) {
                Operation.TricountId = tricount.Id;   
            }
            Title = isNew ? "" : operation.Title;
            Amount = isNew ? 0.00 : operation.Amount;
            Date = isNew ? DateTime.Today : operation.OperationDate;

            IQueryable<ICollection<User>> query;
            if (!isNew) {
                query = from o in PridContext.Context.Operations
                    where o.OperationId == operation.OperationId
                    select o.Users;
                  
            } else {
                query = from t in PridContext.Context.Tricounts
                    where t.Id == tricount.Id
                    select t.Subscribers;
            }
            var user = query.First();
            
            foreach (var row in user) {
                Users.Add(row);
            }
                            
            //selected user must be a user from the combobox(=> from Users)
            SelectedUser = isNew ? Users.FirstOrDefault(u => u.UserId == App.CurrentUser.UserId) : operation.Initiator;

            Cancel = new RelayCommand(CancelAction);
            AddOperation = new RelayCommand(AddOperationAction, () => !HasErrors);
        }

        public override void CancelAction() {
            DialogResult = null;
        }

        private void AddOperationAction() {
//!!répartitions
           
            Operation.Title = Title;
            Operation.Amount = Amount;
            Operation.OperationDate = Date;
                
             if (_isNew) {    
                 // to prevent InvalidOperationException, we need bind the selected user to the context
                 var user = Context.Users.Find(SelectedUser.UserId);
                 Operation.Initiator = user;
                
                Context.Operations.Add(Operation);
                
            } else {
                Operation.Title = Title;
                Operation.Amount = Amount;
                Operation.OperationDate = Date;
                Operation.Initiator = SelectedUser;
            }
            
            Context.SaveChanges();
            CancelAction();
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        }
    }
}