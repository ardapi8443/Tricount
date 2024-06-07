using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel {
    public class AddTemplateViewModel : ViewModelCommon
    {

        private readonly Tricount _tricount;

        public event Action<bool?> RequestClose;

        private ObservableCollection<UserTemplateItemViewModel> _templateItems;

        public ObservableCollection<UserTemplateItemViewModel> TemplateItems {
            get => _templateItems;
            set {
                if (_templateItems != value) {
                    _templateItems = value;
                    RaisePropertyChanged(nameof(TemplateItems));
                }
            }
        }

        private TemplateViewModel _selectedTemplate;

        public TemplateViewModel SelectedTemplate {
            get => _selectedTemplate;
            set {
                _selectedTemplate = value;
                RaisePropertyChanged(SelectedTemplate);
            }
        }
        private string _errorMessage;
        public string ErrorMessage {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        
        private ObservableCollectionFast<TemplateViewModel> _templates;

        public ObservableCollectionFast<TemplateViewModel> Templates {
            get => _templates;
            set {
                if (_templates != value) {
                    _templates = value;
                    RaisePropertyChanged(nameof(Templates));
                }
            }
        }

        private string _addButtonText;

        public string AddButtonText {
            get => _addButtonText;
            set => SetProperty(ref _addButtonText, value);
        }

        public Tricount Tricount {
            get => _tricount;
            private init => SetProperty(ref _tricount, value);
        }

        private Template _template;

        public Template Template { 
            get => _template;
            private init => SetProperty(ref _template, value); }

        private string _title;

        public string Title {
            get => _title;
            set {
                if (_title != value) {
                    _title = value;
                    RaisePropertyChanged(nameof(Title));
                    Validate();
                }
            }
        }
        
        private int part_weight { get; set; }

        public ICommand AddTemplateDbCommand { get; private set; }
        public ICommand CancelTemplate { get; private set; }


        public AddTemplateViewModel(Tricount tricount, Template template, bool isNew,
            ObservableCollection<UserTemplateItemViewModel> templateItems,
            ObservableCollectionFast<TemplateViewModel> templateViewModels, bool fromTemplateView) {
            Tricount = tricount;
            Template = template;
            TemplateItems = templateItems;
            Templates = templateViewModels;

            if (isNew) {
                if (!fromTemplateView) {
                    DisplayAddTemplateWindows(TemplateItems);
                    AddTemplateDbCommand = new RelayCommand(() => AddNewTemplateDB(Title, tricount.Id, TemplateItems), CanAddNewTemplate);
                } else {
                    DisplayAddTemplateWindows(new ObservableCollection<UserTemplateItemViewModel>());
                    AddTemplateDbCommand = new RelayCommand(() => AddNewTemplate(Title, tricount.Id, TemplateItems), CanAddNewTemplate);
                }
            } else {
                DisplayEditTemplateWindows(template, templateItems, Templates);
                AddTemplateDbCommand = new RelayCommand(() => EditTemplate(Title, TemplateItems, template, Templates), CanAddNewTemplate);
            }

            CancelTemplate = new RelayCommand(CloseWindow);
        }

        private void CountWeightZero() {
                           
            part_weight = 0;
            foreach (UserTemplateItemViewModel UTIVM in TemplateItems) {
                if (UTIVM.Weight == 0) {
                    part_weight++;
                }
            }
        }

        private void EditTemplate(string title, IEnumerable<UserTemplateItemViewModel> userItems, Template template,
            ObservableCollectionFast<TemplateViewModel> templateViewModels) {
            Title = title;
            SelectedTemplate = templateViewModels.FirstOrDefault(t => t.Template.TemplateId == template.TemplateId);

            if (SelectedTemplate == null) {
                return;
            }

            SelectedTemplate.Title = title;

            foreach (var userItem in userItems) {
                var existingItem =
                    SelectedTemplate.TemplateItems.FirstOrDefault(ti => ti.UserName == userItem.UserName);
                if (existingItem != null) {
                    existingItem.Weight = userItem.Weight;
                    existingItem.IsChecked = userItem.IsChecked;
                    if (!userItem.IsChecked) {
                        SelectedTemplate.TemplateItems.Remove(existingItem);
                    }
                } else if (userItem.IsChecked) {
                    var newItem = new UserTemplateItemViewModel(
                        userItem.UserName,
                        userItem.Weight,
                        true,
                        false
                    );
                    SelectedTemplate.TemplateItems.Add(newItem);
                }
            }

            RaisePropertyChanged(nameof(SelectedTemplate));
            CloseWindow();

        }

        private bool CanAddNewTemplate() {
            return !HasErrors;
        }

        private void AddNewTemplate(string title, int tricountId, IEnumerable<UserTemplateItemViewModel> userItems) {
            var template = new Template {
                Title = title,
                Tricount = tricountId
            };
            
            bool duplicateTitle = Templates.Any(t => t.Template.TemplateId == 0 && t.Title == template.Title);
            
            if (duplicateTitle) {
                AddError(nameof(Title), "Title already exists.");
                return;
            }

            var templateViewModel = new TemplateViewModel(template, true, false);

            foreach (var userItem in userItems.Where(u => u.IsChecked)) {
                User user = User.GetUserByFullName(userItem.UserName);
                if (user != null) {
                    var templateItem = new UserTemplateItemViewModel(
                        user.FullName,
                        userItem.Weight,
                        true,
                        false
                    );

                    templateViewModel.AddUserTemplateItem(templateItem);
                } else {
                    Console.WriteLine($"User not found: {userItem.UserName}");
                }
            }

            NotifyColleagues(App.Messages.MSG_ADD_TEMPLATE, templateViewModel);
            CloseWindow();
        }

        private void CloseWindow() {
            RequestClose?.Invoke(true);
        }

        private void DisplayEditTemplateWindows(Template template,
            ObservableCollection<UserTemplateItemViewModel> existingTemplateItems,
            ObservableCollectionFast<TemplateViewModel> templateViewModels) {

            Title = templateViewModels.FirstOrDefault(t => t.Template.TemplateId == template.TemplateId)?.Title;
            AddButtonText = "Save";


            TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                Tricount.GetSubscribers().OrderBy(u => u.FullName).Select(u => {
                    var templateItem = existingTemplateItems.FirstOrDefault(ti => ti.UserName == u.FullName);
                    return new UserTemplateItemViewModel(
                        u.FullName,
                        templateItem?.Weight ?? 0,
                        templateItem?.IsChecked ?? false,
                        false
                    );
                })
            );

            RaisePropertyChanged(nameof(TemplateItems));
        }

        private void DisplayAddTemplateWindows(ObservableCollection<UserTemplateItemViewModel> templateItems) {
            Title = "New Template";
            AddButtonText = "Add";

            if (templateItems.IsNullOrEmpty()) {
                TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                    Tricount.GetSubscribers().OrderBy(u => u.FullName).Select(u => new UserTemplateItemViewModel(u.FullName, 0, true, false)));
            } else {
                foreach (var userTemplateItemViewModel in templateItems) {
                    userTemplateItemViewModel.FromOperation = false;
                }

                TemplateItems = templateItems;
            }
        }

        private void AddNewTemplateDB(string title, int tricountId, IEnumerable<UserTemplateItemViewModel> userItems) {
            var template = new Template {
                Title = title,
                Tricount = tricountId
            };

            foreach (var userItem in userItems.Where(u => u.IsChecked)) {
                var user = User.GetUserByFullName(userItem.UserName);
              
                if (user != null) {
                    var templateItem = new TemplateItem {
                        User = user.UserId,
                        Weight = userItem.Weight,
                        TemplateFromTemplateItem = template
                    };

                    templateItem.Add();
                }
            }

            template.Add();
            Context.SaveChanges();
            NotifyColleagues(App.Messages.MSG_ADD_TEMPLATE_OPE, template);
            CloseWindow();
        }
        public override bool Validate() {
            ClearErrors();
            
            //on vérifie l'unicité du titre dans le context
            bool sameTitleTwice = Template.ValidateUnicity(Title, Tricount.Id, Template.TemplateId);

            //si pas, on vérifie dans les templates déjà existants et peut être pas encore sauvé
            bool sameTitleTwiceInRAM = false;
            if (!sameTitleTwice) {
                sameTitleTwiceInRAM = Templates.Any(t => t.Title == Title && t.Template.TemplateId != Template.TemplateId);
            }

            string validateTitle = Template.ValidateTitle(Title);
            if (validateTitle != null) {
                AddError(nameof(Title), validateTitle);
            } else if (sameTitleTwice || sameTitleTwiceInRAM) {
                AddError(nameof(Title), "Title already exists.");
            }
            
            if (!TemplateItems.Any(item => item.IsChecked)) {
                AddError(nameof(TemplateItems),"You must check at least one participant");
                ErrorMessage = "you must check at least one participant";
            } else {
                ErrorMessage = "";
            }
            
            return !HasErrors;
        }
    }
}