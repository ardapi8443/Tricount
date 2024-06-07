using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using prbd_2324_g01.Model;
using prbd_2324_g01.View;
using PRBD_Framework;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Documents;
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
        private string _amount;
        private User _selectedUser;
        private Template _selectedTemplate;
        private ObservableCollectionFast<User> _users = new();
        private DateTime _date;
        private ObservableCollectionFast<Template> _templates = new();
        private ObservableCollectionFast<UserTemplateItemViewModel> _templateItems;
        private string _errorMessage;
        private double _amountParsed;

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

        public string Amount {
            get => _amount;
            set => SetProperty(ref _amount, value,
                () => {
                    //_amountParsed = double.Parse(Amount);
                    
                    //if (double.TryParse(Amount, out _amountParsed)) {
                    if (!CheckIfDouble(Amount)) {
                        Amount = _amountParsed.ToString();
                    } else {
                        _amountParsed = double.Parse(Amount);
                        Amount = Math.Round(_amountParsed, 2).ToString("F2");
                        
                    } 
                    Console.WriteLine(Amount);
                    Validate();
                    NotifyColleagues(App.Messages.MSG_AMOUNT_CHANGED, _amountParsed);
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

        public DateTime CreatedAt { get;  private set; }

        public ObservableCollectionFast<Template> Templates {
            get => _templates;
            set => SetProperty(ref _templates, value);
        }
        
        public ObservableCollectionFast<UserTemplateItemViewModel> TemplateItems {
            get => _templateItems;
            set => SetProperty(ref _templateItems, value, () => { Validate();});
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
            if (double.IsNaN(_amountParsed) || _amountParsed < 0.01) {
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
        
        private bool CheckIfDouble(string value) {
            return Regex.IsMatch(value, @"^[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$");
        }

        public AddEditOperationViewModel(Operation operation, Tricount tricount, bool isNew) {
            _isNew = isNew;
            Operation = operation;
            _tricount = tricount;
            CreatedAt = tricount.CreatedAt;
            if (isNew) {
                Operation.TricountId = tricount.Id;   
            }
            Title = isNew ? "" : operation.Title;
            _amountParsed = isNew ? 0.00 : operation.Amount;
            Amount = _amountParsed.ToString();
            Date = isNew ? DateTime.Today : operation.OperationDate;

            //we populate the Users Combobox
            ICollection<User> users;
                users = _tricount.GetSubscribersForOperation();
            
            foreach (var row in users) {
                Users.Add(row);
            }
                            
            //default selected user must be a user from the combobox(=> from Users)
            SelectedUser = isNew ? App.CurrentUser : operation.Initiator;
            
            //we populate the Templates Combobox
            List<Template> templates = tricount.GetTemplates();
            
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

            Register<Template>(App.Messages.MSG_ADD_TEMPLATE_OPE, (t) => {
                AddTemplate(t);
            });
        }

        private void DisplayRepartitions() {
            // we populate the TemplateItems
            // //transform the list of user ids to a list of users
   
            List<User> userTemplateItems = _tricount.GetUserTemplateItems();
            
            if (!_isNew) {
                // Fetch the information from the Repartition table
                List<Repartition> repartitionItems = Operation.GetRepartitionItems();
   
                TemplateItems = new ObservableCollectionFast<UserTemplateItemViewModel>(
                    userTemplateItems.Select(u => new UserTemplateItemViewModel(u.FullName,
                        repartitionItems.FirstOrDefault(ri => ri.UserId == u.UserId)?.Weight ?? 0,
                        false, true)));
            } else {
                TemplateItems = new ObservableCollectionFast<UserTemplateItemViewModel>(
                    userTemplateItems.Select(u => new UserTemplateItemViewModel(u.FullName, 0, true, true)));
            }
            //calculate and send the total of weight and the amount to the templateItems
            
            Register(App.Messages.MSG_WEIGHT_CHANGED, CalculateTotalWeight);
            
            CalculateTotalWeight();
            NotifyColleagues(App.Messages.MSG_AMOUNT_CHANGED, _amountParsed);
        }

        private void CalculateTotalWeight() {
            foreach (var repartition in TemplateItems) {
                _totalWeight += repartition.Weight;
            }
            NotifyColleagues(App.Messages.MSG_TOTAL_WEIGHT_CHANGED, _totalWeight);
            _totalWeight = 0;
        }

        public override void CancelAction() {
            DialogResult = null;
        }

        private void AddOperationAction() {
           
            Operation.Title = Title;
            Operation.Amount = _amountParsed;
            Operation.OperationDate = Date;
                
            //first, save/update the operation 
            if (_isNew) {    
                 // to prevent InvalidOperationException, we need bind the selected user to the context
                 var user = User.UserById(SelectedUser.UserId);
                 Operation.Initiator = user;
                Operation.Add();
               
            } else {
                Operation.Initiator = SelectedUser;
                
            }
            
            //must save here to have an Operation ID
            Context.SaveChanges();
            
            //then, save/update the repartitions in TemplateItems
            if (_isNew) {
                //save repartitions
                foreach (var repartition in TemplateItems) {
                    int userID = User.GetUserIdFromUserName(repartition.UserName);
                    if (repartition.Weight > 0) {
                        Repartition rep = new(
                            //get UserId from the UserName
                            userID,
                            Operation.OperationId,
                            repartition.Weight);
                        rep.Add();
                       
                    }
                } 
            } else {
                // Update repartitions
                foreach (var repartition in TemplateItems) {
                    int userId = User.GetUserIdFromUserName(repartition.UserName);
                    var existingRepartition = Repartition.GetRepartitionByUserIdAndOperationId(userId, Operation.OperationId);

                    if (repartition.Weight > 0) {
                        if (existingRepartition != null) {
                            // If repartition exists, update the weight
                            existingRepartition.Weight = repartition.Weight;
                            
                            // Attach the repartition to the context and set its state to Modified
                            //Context.Repartitions.Update(existingRepartition);
                        } else {
                            // Else create a new repartition
                            var newRepartition = new Repartition(userId, Operation.OperationId, repartition.Weight);
                            newRepartition.Add();
                            
                        }
                    } else {
                        // If the weight is 0 and the repartition exists, delete the repartition
                        if (existingRepartition != null) {
                            existingRepartition.Delete();
                        }
                    }
                }
            }
            //finally, save the changes
            Context.SaveChanges();
            
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
            CancelAction();
        }
        
        public void ApplyTemplateAction() {

            List<TemplateItem> templateItems = SelectedTemplate.GetTemplateItems();
            List<User> userTemplateItems = Operation.GetUserTemplateItems();
            
            TemplateItems = new ObservableCollectionFast<UserTemplateItemViewModel>(
                userTemplateItems.Select(u => new UserTemplateItemViewModel(u.FullName, 
                    templateItems.FirstOrDefault(ti => ti.User == u.UserId)?.Weight ?? 0, 
                    _isNew, true)));
            
            CalculateTotalWeight();
            NotifyColleagues(App.Messages.MSG_AMOUNT_CHANGED, _amountParsed);
        }

        // !! doit prendre les élément tels que vu dans Templatesitems au moment de cliquer sur le bouton
        //?? et si l'operation est déjà basé sur un template ??
        public void SaveTemplateAction() {
            Template template;
            if (_isNewTemplate) {
                template = new Template();
            } else {
                template = Template.GetTemplateById(SelectedTemplate.TemplateId);
            } 
            // need to update code here
            var addTemplateDialog = new AddTemplateView(_tricount, template, _isNewTemplate, TemplateItems,new ObservableCollectionFast<TemplateViewModel>(),false) {
                Owner = App.Current.MainWindow
            };
            addTemplateDialog.ShowDialog(); 
        }
  
        public void AddTemplate(Template template) {
            Templates.Add(template);
        }

        public void DeleteOperationAction() {
            var confirmationDialog = new ConfirmationDialogView("operation");
            bool? dialogResult = confirmationDialog.ShowDialog();

            if (dialogResult == true && Operation != null) {
       
                Operation.Delete();
                
                NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
                CancelAction();
            }
            
            
            
        }
        
        //vraiment utile ici ?
        protected override void OnRefreshData() {
                // Refresh the operation
                Operation operation = Operation.GetOperationById(Operation.OperationId);
                
                if (operation != null) {
                    Operation = operation;
                }
                //refresh the operations
                DisplayRepartitions();
                
                
        }
    }
}