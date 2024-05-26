using Microsoft.EntityFrameworkCore;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel {
    public class TemplateViewModel : ViewModelCommon {
        public Template Template { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        
        private bool _isNew;
        private string _title;
        public bool IsNew
        {
            get => _isNew;
            set
            {
                if (_isNew != value)
                {
                    _isNew = value;
                    RaisePropertyChanged(nameof(IsNew));
                }
            }
        }
        
        public string Title {
            get => _title;
            set {
                if ( _title != value) {
                    _title = value;
                    RaisePropertyChanged(nameof(Title));
                }
            }
        }
        
        private ObservableCollectionFast<UserTemplateItemViewModel> _templateitems;

        public ObservableCollectionFast<UserTemplateItemViewModel> TemplateItems {
            get => _templateitems;
            set {
                if (_templateitems != value) {
                    _templateitems = value;
                    RaisePropertyChanged(nameof(_templateitems));
                }
            }
        }
        
        public TemplateViewModel(Template template,bool isNew, bool loadFromDb = true) {
            Template = template;
            Title = template.Title;
            TemplateItems = new ObservableCollectionFast<UserTemplateItemViewModel>();
            IsNew = isNew;
            if (loadFromDb) {
                LoadTemplateItemsFromDb(template.TemplateId);
            }
            EditCommand = new RelayCommand(EditTemplate);
            DeleteCommand = new RelayCommand(DeleteTemplate);
            
        }
        
        public void AddUserTemplateItem(UserTemplateItemViewModel userTemplateItem) {
            TemplateItems.Add(userTemplateItem);
        }
        
        private void EditTemplate() {
            NotifyColleagues(App.Messages.MSG_EDIT_TEMPLATE,this);
        }
        
        private void DeleteTemplate() {
            NotifyColleagues(App.Messages.MSG_DELETE_TEMPLATE,this);
        }
        
        private void LoadTemplateItemsFromDb(int templateId) {
            var templateItems = Context.TemplateItems
                .Where(ti => ti.Template == templateId)
                .Include(ti => ti.UserFromTemplateItem)
                .ToList();


            foreach (var item in templateItems) {
                TemplateItems.Add(new UserTemplateItemViewModel(
                    item.UserFromTemplateItem.FullName,
                    item.Weight,
                    true,
                    false
                ));
            }
        }
    }

}