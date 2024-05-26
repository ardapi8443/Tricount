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
    
    private TemplateViewModel _selectedTemplate;
    public TemplateViewModel SelectedTemplate {
        get => _selectedTemplate;
        set {
            _selectedTemplate = value;
            RaisePropertyChanged(SelectedTemplate);
        }
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

    private string _title;

    public string Title {
        get => _title;
        set {
            if ( _title != value) {
                _title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }
    }

    public ICommand AddTemplateDbCommand { get; private set; }
    public ICommand CancelTemplate { get; private set; }


    public AddTemplateViewModel(Tricount tricount, Template template, bool isNew, ObservableCollection<UserTemplateItemViewModel> templateItems, ObservableCollectionFast<TemplateViewModel> templateViewModels) {
        TemplateItems = templateItems;
        Templates = templateViewModels;
        
        if (isNew) {
            DisplayAddTemplateWindows();
            AddTemplateDbCommand = new RelayCommand(() => AddNewTemplate(Title, tricount.Id, TemplateItems));
        } else {
            DisplayEditTemplateWindows(template, templateItems, Templates);
            AddTemplateDbCommand = new RelayCommand(() => EditTemplate(Title, TemplateItems, template, Templates));
        }

        CancelTemplate = new RelayCommand(CloseWindow);
    }

    private void EditTemplate(string title, IEnumerable<UserTemplateItemViewModel> userItems, Template template,ObservableCollectionFast<TemplateViewModel> templateViewModels) {
        Title = title;
        SelectedTemplate = templateViewModels.FirstOrDefault(t => t.Template.TemplateId == template.TemplateId);
        
        if (SelectedTemplate == null) {
            return;
        }
        SelectedTemplate.Title = title;
        
        foreach (var userItem in userItems) {
            var existingItem = SelectedTemplate.TemplateItems.FirstOrDefault(ti => ti.UserName == userItem.UserName);
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
      //  RaisePropertyChanged(nameof(SelectedTemplate));
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
        
        NotifyColleagues(App.Messages.MSG_ADD_TEMPLATE, templateViewModel);
        CloseWindow();
    }

        private void CloseWindow() {
            RequestClose?.Invoke(true);
        }
        
        private void DisplayEditTemplateWindows(Template template, ObservableCollection<UserTemplateItemViewModel> existingTemplateItems, ObservableCollectionFast<TemplateViewModel> templateViewModels) {
            
            var distinctUsers = Context.Users
                .Where(u => u.Role == Role.Viewer)
                .OrderBy(u => u.FullName)
                .GroupBy(u => u.FullName)
                .Select(g => g.First())
                .ToList();
            

            Title = templateViewModels.FirstOrDefault(t => t.Template.TemplateId == template.TemplateId)?.Title;
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