using prbd_2324_g01.Model;
using prbd_2324_g01.ViewModel;
using PRBD_Framework;
using System.Windows;

namespace prbd_2324_g01.View
{
    public partial class AddEditOperationView : DialogWindowBase
    {
        public AddEditOperationView(Operation operation, Tricount tricount, bool isNew) {
            InitializeComponent();
            if (isNew) {
                Title = "Add operation";
                saveadd.Content = "Add";
            }
            DataContext = new AddEditOperationViewModel(operation, tricount, isNew);
        }
        
        
    }
}