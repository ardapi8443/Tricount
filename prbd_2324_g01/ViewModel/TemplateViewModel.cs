using Microsoft.EntityFrameworkCore;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel {
    public class TemplateViewModel : ViewModelCommon {
        public Template Template { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        
        private bool _isVisible;
        public bool IsVisible {
            get => _isVisible;
            set {
                if (_isVisible != value) {
                    _isVisible = value;
                    RaisePropertyChanged(nameof(IsVisible));
                    RaisePropertyChanged(nameof(Visibility)); 
                }
            }
        }
        public Visibility Visibility => IsVisible ? Visibility.Visible : Visibility.Collapsed;
        
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
            IsVisible = true;
            TemplateItems = new ObservableCollectionFast<UserTemplateItemViewModel>();
            IsNew = isNew;
            if (loadFromDb) {
                LoadTemplateItemsFromDb(template.TemplateId);
            }
            EditCommand = new RelayCommand(EditTemplate);
            DeleteCommand = new RelayCommand(DeleteTemplate);
            
            Register<bool>(App.Messages.MODIFED_PARTICIPANT, OnModifiedParticipant);
            
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
            
            List<TemplateItem> templateItems = TemplateItem.GetAllItemByTemplateID(templateId);


            foreach (var item in templateItems) {
                TemplateItems.Add(new UserTemplateItemViewModel(
                    item.UserFromTemplateItem.FullName,
                    item.Weight,
                    false,
                    false
                ));
            }
        }
        private void OnModifiedParticipant(bool isModifiedParticipant) {
            IsVisible = !isModifiedParticipant;
        }
    }
}