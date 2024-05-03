using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        private Template _selectedTemplate;
        //combobox !!
        private ObservableCollectionFast<User> _users = new();
        private DateTime _date;
        private ObservableCollectionFast<Template> _templates = new();
        private ObservableCollection<UserTemplateItemViewModel> _templateItems;

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
        
        public Template SelectedTemplate {
            get => _selectedTemplate;
            set => SetProperty(ref _selectedTemplate, value); 
        }
        
        public DateTime Date {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public ObservableCollectionFast<Template> Templates {
            get => _templates;
            set => SetProperty(ref _templates, value);
        }
        
        public ObservableCollection<UserTemplateItemViewModel> TemplateItems {
            get => _templateItems;
            set {
                if (_templateItems != value) {
                    _templateItems = value;
                    RaisePropertyChanged(nameof(TemplateItems));
                }
            }
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

            //we populate the Users Combobox
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
                            
            //default selected user must be a user from the combobox(=> from Users)
            SelectedUser = isNew ? Users.FirstOrDefault(u => u.UserId == App.CurrentUser.UserId) : operation.Initiator;
            
            //we populate the Templates Combobox
            var q2 = from t in PridContext.Context.Templates
                where t.Tricount == tricount.Id
                select t;
            var templates = q2.ToList();
            

            if (!templates.IsNullOrEmpty()) {
               foreach (var row in templates) { 
               Templates.Add(row);
               }
            }
            
            // we popullate the TemplateItems
            var queryUsersID = from s in PridContext.Context.Subscriptions
                where s.TricountId == tricount.Id
                select s.UserId;
            //transform the list of user ids to a list of users
            var userTemplateItems = queryUsersID.ToList().Select(u => PridContext.Context.Users.Find(u)).OrderBy(t => t.FullName).ToList();
            foreach (var u in userTemplateItems) {
            }
            
            if (!isNew) {
                // Fetch the information from the Repartition table
                var repartitionItems = PridContext.Context.Repartitions
                    .Where(r => r.OperationId == operation.OperationId)
                    .ToList();

                TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                    userTemplateItems.Select(u => new UserTemplateItemViewModel(u.FullName,
                        repartitionItems.FirstOrDefault(ri => ri.UserId == u.UserId)?.Weight ?? 0,
                        false)));
            } else {
                TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                    userTemplateItems.Select(u => new UserTemplateItemViewModel(u.FullName, 0, true)));
            }
               
            //we define the buttons
            Cancel = new RelayCommand(CancelAction);
            AddOperation = new RelayCommand(AddOperationAction, () => !HasErrors);
            ApplyTemplate = new RelayCommand(ApplyTemplateAction);
            //don't forget the delete button when editing
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
        
        public void ApplyTemplateAction() {
            var templateItems = PridContext.Context.TemplateItems
                .AsNoTracking() 
                .Where(ti => ti.Template == SelectedTemplate.TemplateId) 
                .Include(ti => ti.UserFromTemplateItem) 
                .DefaultIfEmpty()
                .ToList();
                        
            var userTemplateItems = PridContext.Context.Tricounts
                .Where(t => t.Id == Operation.TricountId)
                .SelectMany(t => t.Subscribers)
                .OrderBy(t => t.FullName)
                .ToList();
                        
            TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                userTemplateItems.Select(u => new UserTemplateItemViewModel(u.FullName, 
                    templateItems.FirstOrDefault(ti => ti.User == u.UserId)?.Weight ?? 0, 
                    _isNew)));
        }
    }
}