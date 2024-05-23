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
    public ICommand CancelTemplate { get; private set; }


    public AddTemplateViewModel(Tricount tricount, Template template, bool isNew, ObservableCollection<UserTemplateItemViewModel> templateItems) {
        TemplateItems = templateItems;

        if (isNew) {
            DisplayAddTemplateWindows();
            AddTemplateDbCommand = new RelayCommand(() => AddNewTemplate(Title, tricount.Id, TemplateItems));
        } else {
            DisplayEditTemplateWindows(template, templateItems);
            AddTemplateDbCommand = new RelayCommand(() => EditTemplate(Title, TemplateItems, template));
        }

        CancelTemplate = new RelayCommand(CloseWindow);
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

        var templateViewModel = new TemplateViewModel(template, true,false);

        foreach (var userItem in userItems.Where(u => u.IsChecked)) {
            var user = Context.Users.FirstOrDefault(u => u.FullName == userItem.UserName);
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
        
        NotifyColleagues(App.Messages.ADD_TEMPLATE, templateViewModel);
        CloseWindow();
    }

        private void CloseWindow() {
            RequestClose?.Invoke(true);
        }
        
        private void DisplayEditTemplateWindows(Template template, ObservableCollection<UserTemplateItemViewModel> existingTemplateItems) {
            
            var distinctUsers = Context.Users
                .Where(u => u.Role == Role.Viewer)
                .OrderBy(u => u.FullName)
                .GroupBy(u => u.FullName)
                .Select(g => g.First())
                .ToList();
            

            Title = template.Title;
            AddButtonText = "Save";
            
            
            TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                distinctUsers.Select(u => {
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
                distinctUsers.Select(u => new UserTemplateItemViewModel(u.FullName, 0,true, false)));
            
        }
        
    }
    
}