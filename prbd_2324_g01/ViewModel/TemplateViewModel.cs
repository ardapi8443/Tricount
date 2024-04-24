using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel {
    public class TemplateViewModel : ViewModelCommon {
        public string TemplateTitle { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        
        public TemplateViewModel(string template) {
            TemplateTitle = template;
            EditCommand = new RelayCommand(EditTemplate);
            DeleteCommand = new RelayCommand(DeleteTemplate);
            
        }
        
        private void EditTemplate() {
            Console.WriteLine("Je suis dans TemplateViewModel");
        }
        
        private void DeleteTemplate() {
            // Delete template logic
            Console.WriteLine("Je suis dans TemplateViewModel");
        }
    }

}