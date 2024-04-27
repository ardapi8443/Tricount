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
        
        public TemplateViewModel(Template template,bool isNew) {
            Template = template;
            IsNew = isNew;
            EditCommand = new RelayCommand(EditTemplate);
            DeleteCommand = new RelayCommand(DeleteTemplate);
            
        }
        
        private void EditTemplate() {
            Console.WriteLine("Je suis dans TemplateViewModel");
            NotifyColleagues(App.Messages.MSG_EDIT_TEMPLATE,Template);
        }
        
        private void DeleteTemplate() {
            NotifyColleagues(App.Messages.MSG_DELETE_TEMPLATE,Template);
            Console.WriteLine("Je suis dans TemplateViewModel");
        }
    }

}