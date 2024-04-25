using Microsoft.EntityFrameworkCore;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel {
    public class AddTemplateViewModel : ViewModelCommon {
        
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

        private string _addButtonText;

        public string AddButtonText {
            get => _addButtonText; 
            set => SetProperty(ref _addButtonText, value); 
        }
        
        private ObservableCollectionFast<TemplateItem> _templateItem = new();
        
        public ObservableCollectionFast<TemplateItem> TemplateItem {
            get => _templateItem;
            set => SetProperty(ref _templateItem, value);
        }

        public Tricount Tricount {
            get => _tricount;
            private init => SetProperty(ref _tricount, value);
        }

        private string _title;
        public string Title {
            get => _title; 
            set => SetProperty(ref _title, value); 
        }
        
        public ICommand AddTemplateDbCommand { get; private set; }


        public AddTemplateViewModel(Tricount tricount,Template template, bool isNew) {
            
            
            if (!isNew) {
                DisplayEditTemplateWindows(template);
                AddTemplateDbCommand = new RelayCommand(() => EditTemplate(Title, TemplateItems, template));
            } else {
                DisplayAddTemplateWindows();
                AddTemplateDbCommand = new RelayCommand(() => AddNewTemplate(Title, tricount.Id, TemplateItems));
            }
        }

        private void EditTemplate(string title, IEnumerable<UserTemplateItemViewModel> userItems, Template template) {
            Console.WriteLine("Je suis dans AddTemplateViewModel");
            /*template.Title = title;
            Context.Templates.Update(template);

            // Fetch existing template items linked to this template
            var existingItems = Context.TemplateItems
                .Include(ti => ti.UserFromTemplateItem)
                .Where(ti => ti.Template == template.TemplateId)
                .ToList();

            // Iterate through each user item provided in the view model
            foreach (var userItem in userItems) {
                // Find the existing template item that corresponds to the user item
                var existingItem = existingItems.FirstOrDefault(ti => ti.UserFromTemplateItem.FullName == userItem.UserName);
                
                if (userItem.IsChecked) {
                    if (existingItem != null) {

                        existingItem.Weight = userItem.Weight;
                        Context.TemplateItems.Update(existingItem); 
                    }
                }
            }
            Context.SaveChanges();*/
        }

        private void AddNewTemplate(string title, int tricountId, IEnumerable<UserTemplateItemViewModel> userItems) {
            var template = new Template {
                Title = title,
                Tricount = tricountId
            };
            
            foreach (var userItem in userItems.Where(u => u.IsChecked)) {
                var user = Context.Users.FirstOrDefault(u => u.FullName == userItem.UserName);
                if (user != null) {
                    var templateItem = new TemplateItem {
                        User = user.UserId,
                        Weight = userItem.Weight,
                        TemplateFromTemplateItem = template
                    };

                    Context.TemplateItems.Add(templateItem);
                }
            }
    
            Context.Templates.Add(template);
            Context.SaveChanges();
            NotifyColleagues(App.Messages.MSG_ADD_TEMPLATE, template);
            ExecuteAddTemplate();
        }
        
        private void ExecuteAddTemplate() {
            RequestClose?.Invoke(true); 
        }

        private void DisplayEditTemplateWindows(Template template) {
            var templateItems = PridContext.Context.TemplateItems
                .Where(ti => ti.Template == template.TemplateId) 
                .Include(ti => ti.UserFromTemplateItem) 
                .ToList();
            
            Title = template.Title;
            AddButtonText = "Save";
            
            TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                templateItems.Select(ti => new UserTemplateItemViewModel(ti.UserFromTemplateItem.FullName, ti.Weight,false)));

        }

        private void DisplayAddTemplateWindows() {
            
            var distinctUsers = PridContext.Context.Users
                .Where(u => u.Role == Role.Viewer)
                .OrderBy(u => u.FullName)
                .GroupBy(u => u.FullName)
                .Select(g => g.First())
                .ToList();
                
            Title = "New Template";
            AddButtonText = "Add";
                
            TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                distinctUsers.Select(u => new UserTemplateItemViewModel(u.FullName, 0,true)));
            
        }
        
    }
    
}