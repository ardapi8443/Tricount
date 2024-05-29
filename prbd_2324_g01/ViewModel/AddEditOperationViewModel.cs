using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using prbd_2324_g01.Model;
using prbd_2324_g01.View;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Operation = prbd_2324_g01.Model.Operation;

namespace prbd_2324_g01.ViewModel {
    public class AddEditOperationViewModel : DialogViewModelBase<Operation, PridContext> {
        public ICommand ApplyTemplate { get; set; }
        public ICommand SaveTemplate { get; set; }
        public ICommand DeleteOperation { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand AddOperation { get; set; }

        private Tricount _tricount { get; set; }
        private bool _isNew { get; set; }
        private Operation _operation;
        private string _title;
        private double _amount;
        private User _selectedUser;
        private Template _selectedTemplate;
        private ObservableCollectionFast<User> _users = new();
        private DateTime _date;
        private ObservableCollectionFast<Template> _templates = new();
        private ObservableCollection<UserTemplateItemViewModel> _templateItems;
        private string _errorMessage;

        private bool _isNewTemplate = true;
        private int _totalWeight = 0;

        public Operation Operation {
            get => _operation;
            set => SetProperty(ref _operation, value);
        }
        
        public string Title {
            get => _title;
            set => SetProperty(ref _title, value, () => { Validate();});
        }

        public double Amount {
            get => _amount;
            set => SetProperty(ref _amount, value,
                () => {
                    Validate();
                    NotifyColleagues(App.Messages.MSG_AMOUNT_CHANGED, Amount);
                });
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
            set => SetProperty(ref _date, value, () => { Validate();});
        }

        public ObservableCollectionFast<Template> Templates {
            get => _templates;
            set => SetProperty(ref _templates, value);
        }
        
        public ObservableCollection<UserTemplateItemViewModel> TemplateItems {
            get => _templateItems;
            set => SetProperty(ref _templateItems, value, () => { Validate();});
            // set {
            //     if (_templateItems != value) {
            //         _templateItems = value;
            //         RaisePropertyChanged(nameof(TemplateItems));
            //     }
            // }
        }
        
        public string ErrorMessage {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public override bool Validate() {
            ClearErrors();

            if (string.IsNullOrEmpty(Title)) {
                AddError(nameof(Title), "required");
            } else if (Title.Length < 3) {
                AddError(nameof(Title), "min 3 characters");
            }
            
            if (Amount < 0.01) {
                AddError(nameof(Amount), "minimum 1 cent");
            }

            DateTime tempDate = new DateTime(
                _tricount.CreatedAt.Year, _tricount.CreatedAt.Month, _tricount.CreatedAt.Day, 0, 0, 0);
            
            if (Date < tempDate) {
                AddError(nameof(Date), "can't add operation before tricount creation date");
            } else if (Date > DateTime.Today) {
                AddError(nameof(Date), "cannot be in the future");
            }

            if (TemplateItems != null && !TemplateItems.Any(item => item.IsChecked)) {
                AddError(nameof(TemplateItems), "you must check at least one participant");
                ErrorMessage = "you must check at least one participant";
            } else {
                ErrorMessage = "";
            }
            
            return !HasErrors;
        }

        public AddEditOperationViewModel(Operation operation, Tricount tricount, bool isNew) {
            _isNew = isNew;
            Operation = operation;
            _tricount = tricount;
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
            
            Templates.Insert(0, new Template{Title = "--choose a template--"});
            SelectedTemplate = Templates.FirstOrDefault();
            
            DisplayRepartitions();
               
            //we define the buttons
            Cancel = new RelayCommand(CancelAction);
            AddOperation = new RelayCommand(AddOperationAction, () => !HasErrors);
            ApplyTemplate = new RelayCommand(ApplyTemplateAction, () => !SelectedTemplate.Title.Equals("--choose a template--"));
            SaveTemplate = new RelayCommand(SaveTemplateAction, () => !HasErrors);
            DeleteOperation = new RelayCommand(DeleteOperationAction);
            
            Register(App.Messages.MSG_CHECKBOX_CHANGED, () => Validate());
        }

        private void DisplayRepartitions() {
            // we populate the TemplateItems
            var queryUsersID = from s in PridContext.Context.Subscriptions
                where s.TricountId == _tricount.Id
                select s.UserId;
            //transform the list of user ids to a list of users
            var userTemplateItems = queryUsersID.ToList().Select(u => PridContext.Context.Users.Find(u)).OrderBy(t => t.FullName).ToList();
            
            if (!_isNew) {
                // Fetch the information from the Repartition table
                var repartitionItems = PridContext.Context.Repartitions
                    .AsNoTracking()
                    .Where(r => r.OperationId == _operation.OperationId)
                    .ToList();

                TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                    userTemplateItems.Select(u => new UserTemplateItemViewModel(u.FullName,
                        repartitionItems.FirstOrDefault(ri => ri.UserId == u.UserId)?.Weight ?? 0,
                        false, true)));
            } else {
                TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                    userTemplateItems.Select(u => new UserTemplateItemViewModel(u.FullName, 0, true, true)));
            }
            //calculate and send the total of weight and the amount to the templateItems
            
            Register(App.Messages.MSG_WEIGHT_CHANGED, CalculateTotalWeight);
            
            CalculateTotalWeight();
            NotifyColleagues(App.Messages.MSG_AMOUNT_CHANGED, Amount);
        }

        private void CalculateTotalWeight() {
            foreach (var repartition in TemplateItems) {
                _totalWeight += repartition.Weight;
            }
            NotifyColleagues(App.Messages.MSG_TOTAL_WEIGHT_CHANGED, _totalWeight);
            _totalWeight = 0;
            Console.WriteLine();
        }

        public override void CancelAction() {
            DialogResult = null;
        }

        private void AddOperationAction() {
           
            Operation.Title = Title;
            Operation.Amount = Amount;
            Operation.OperationDate = Date;
                
            //first, save/update the operation 
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
            
            //must save here to have an Operation ID
            Context.SaveChanges();
            
            //then, save/update the repartitions in TemplateItems
            if (_isNew) {
                //save repartitions
                foreach (var repartition in TemplateItems) {
                    if (repartition.Weight > 0) {
                        Repartition rep = new(
                            //get UserId from the UserName
                            Context.Users.Where(u => u.FullName == repartition.UserName).Select(u => u.UserId).FirstOrDefault(),
                            Operation.OperationId,
                            repartition.Weight);
                        Context.Repartitions.Add(rep);
                    }
                }
            } else {
                // Update repartitions
                foreach (var repartition in TemplateItems) {
                    var userId = Context.Users.Where(u => u.FullName == repartition.UserName).Select(u => u.UserId).FirstOrDefault();
                    var existingRepartition = Context.Repartitions.FirstOrDefault(r => r.OperationId == Operation.OperationId && r.UserId == userId);

                    if (repartition.Weight > 0) {
                        if (existingRepartition != null) {
                            // Console.WriteLine("Existing repartition: " + existingRepartition.OperationId + "." + existingRepartition.UserId);
                            // If repartition exists, update the weight
                            // Console.WriteLine(existingRepartition.Weight + " -> " + repartition.Weight);
                            existingRepartition.Weight = repartition.Weight;
                            
                            // Attach the repartition to the context and set its state to Modified
                            //Context.Repartitions.Update(existingRepartition);
                        } else {
                            // Console.WriteLine("c'est le caca");
                            // Else create a new repartition
                            var newRepartition = new Repartition(userId, Operation.OperationId, repartition.Weight);
                            Context.Repartitions.Add(newRepartition);
                        }
                    } else {
                        // If the weight is 0 and the repartition exists, delete the repartition
                        if (existingRepartition != null) {
                            Context.Repartitions.Remove(existingRepartition);
                        }
                    }
                }
            }
            //finally, save the changes
            Context.SaveChanges();
            Console.WriteLine("");
            
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
            CancelAction();
        }
        
        public void ApplyTemplateAction() {
            _isNewTemplate = false;
            
            var templateItems = Context.TemplateItems
                .AsNoTracking()
                .Where(ti => ti.Template == SelectedTemplate.TemplateId) 
                .Include(ti => ti.UserFromTemplateItem) 
                .DefaultIfEmpty()
                .ToList();
                        
            var userTemplateItems = Context.Tricounts
                .Where(t => t.Id == Operation.TricountId)
                .SelectMany(t => t.Subscribers)
                .OrderBy(t => t.FullName)
                .ToList();
                        
            TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                userTemplateItems.Select(u => new UserTemplateItemViewModel(u.FullName, 
                    templateItems.FirstOrDefault(ti => ti.User == u.UserId)?.Weight ?? 0, 
                    _isNew, true)));
            
            CalculateTotalWeight();
            NotifyColleagues(App.Messages.MSG_AMOUNT_CHANGED, Amount);
        }

        // !! doit prendre les élément tels que vu dans Templatesitems au moment de cliquer sur le bouton
        //?? et si l'operation est déjà basé sur un template ??
        public void SaveTemplateAction() {
            Template template;
            if (_isNewTemplate) {
                template = new Template();
            } else {
                template = Context.Templates.Find(SelectedTemplate.TemplateId);
            }
            // need to update code here
            var addTemplateDialog = new AddTemplateView(_tricount, template, _isNewTemplate, TemplateItems,null) {
                Owner = App.Current.MainWindow
            };
            addTemplateDialog.ShowDialog();
        }

        public void DeleteOperationAction() {
            var confirmationDialog = new ConfirmationDialogView("operation");
            bool? dialogResult = confirmationDialog.ShowDialog();

            if (dialogResult == true && Operation != null) {
                // Fetch the operation
                var operation = Context.Operations.Find(Operation.OperationId);
                // Delete the operation
                Context.Operations.Remove(operation);
                Context.SaveChanges();
                NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
                CancelAction();
            }
            
            
            
        }
        
        //vraiment utile ici ?
        protected override void OnRefreshData() {
                // Refresh the operation
                var operation = PridContext.Context.Operations.Find(Operation.OperationId);
                if (operation != null) {
                    Operation = operation;
                }
                //refresh the operations
                DisplayRepartitions();
            }
    }
    
    
}