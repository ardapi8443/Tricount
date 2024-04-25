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
        public ObservableCollection<UserTemplateItemViewModel> TemplateItems { get; set; }
        
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
        
        public ICommand AddTemplateDbCommand { get; }


        public AddTemplateViewModel(Tricount tricount,Template template, bool isNew) {
            
            if (!isNew) {
                Title = template.Title;
                
                var templateItems = PridContext.Context.TemplateItems
                    .Where(ti => ti.Template == template.TemplateId) // Filter by the TemplateId
                    .Include(ti => ti.UserFromTemplateItem) // Include the User navigation property
                    .ToList();
                TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                    templateItems.Select(ti => new UserTemplateItemViewModel(ti.UserFromTemplateItem.FullName, ti.Weight)));
                
            } else {
                var distinctUsers = PridContext.Context.Users
                    .Where(u => u.Role == Role.Viewer)
                    .OrderBy(u => u.FullName)
                    .GroupBy(u => u.FullName)
                    .Select(g => g.First())
                    .ToList();
                
                Title = "New Template";
                
                TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                    distinctUsers.Select(u => new UserTemplateItemViewModel(u.FullName, 0)));
            
                AddTemplateDbCommand = new RelayCommand(() => AddNewTemplate(Title, tricount.Id, TemplateItems));
            }
        }

        public void AddNewTemplate(string title, int tricountId, IEnumerable<UserTemplateItemViewModel> userItems) {
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
    }
    
}