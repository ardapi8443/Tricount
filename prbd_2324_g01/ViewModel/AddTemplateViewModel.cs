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
    public ICommand CancelTempalte { get; private set; }


    public AddTemplateViewModel(Tricount tricount, Template template, bool isNew) {


        if (!isNew) {
            DisplayEditTemplateWindows(template);
            AddTemplateDbCommand = new RelayCommand(() => EditTemplate(Title, TemplateItems, template));
        } else {
            DisplayAddTemplateWindows();
            AddTemplateDbCommand = new RelayCommand(() => AddNewTemplate(Title, tricount.Id, TemplateItems));
        }

        CancelTempalte = new RelayCommand((CloseWindow));
    }

    public AddTemplateViewModel(Tricount tricount, Template template, bool isNew,
        ObservableCollection<UserTemplateItemViewModel> templateItems) 
    : this(tricount, template, isNew) {
    TemplateItems = templateItems;
}
        
    private void EditTemplate(string title, IEnumerable<UserTemplateItemViewModel> userItems, Template template) {

        template.Title = title;
        
        var users = Context.Users.ToDictionary(u => u.FullName, u => u.UserId);
        
        var existingItems = Context.TemplateItems
            .Where(ti => ti.Template == template.TemplateId)
            .ToList();
        
        var existingItemsMap = existingItems.ToDictionary(ti => ti.User);

        foreach (var userItem in userItems) {
            
            if (users.TryGetValue(userItem.UserName, out var userId)) {
                if (existingItemsMap.TryGetValue(userId, out var existingItem)) {
                    existingItem.Weight = userItem.Weight;
                    if (!userItem.IsChecked) {
                        Context.TemplateItems.Remove(existingItem);
                    }
                } else {
                    var newItem = new TemplateItem {
                        User = userId,
                        Weight = userItem.Weight,
                        Template = template.TemplateId
                    };
                    Context.TemplateItems.Add(newItem);
                }
            }
        }
        Context.SaveChanges();
        NotifyColleagues(App.Messages.MSG_UPDATE_EDITVIEW, Tricount);
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        CloseWindow();
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

            template.Save();
            NotifyColleagues(App.Messages.MSG_UPDATE_EDITVIEW, Tricount);
            CloseWindow();
        }

        private void CloseWindow() {
            RequestClose?.Invoke(true);
        }
        
        private void DisplayEditTemplateWindows(Template template) {
           /* asNoTracking used to only read the datas , update is used with the save button */
           /* Refresh datas after edtiting it with the save button */
            var templateItems = PridContext.Context.TemplateItems
                .AsNoTracking() 
                .Where(ti => ti.Template == template.TemplateId) 
                .Include(ti => ti.UserFromTemplateItem) 
                .ToList();
            
            var users = PridContext.Context.Users
                .Where(u => u.Role == Role.Viewer)
                .OrderBy(u => u.FullName)
                .ToList();
            
            Title = template.Title;
            AddButtonText = "Save";
            
            TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                users.Select(u => new UserTemplateItemViewModel(u.FullName, 
                    templateItems.FirstOrDefault(ti => ti.User == u.UserId)?.Weight ?? 0, 
                    false)));
            
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