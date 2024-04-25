using prbd_2324_g01.Model;
using prbd_2324_g01.ViewModel;
using System.Windows;

namespace prbd_2324_g01.View {
    public partial class AddTemplateView : Window {
        public AddTemplateView(Tricount tricount, Template template, bool isNew) {
            
            InitializeComponent();

            var viewModel = new AddTemplateViewModel(tricount, template, isNew); 
            
            viewModel.RequestClose += (dialogResult) => {
                this.DialogResult = dialogResult;
                this.Close(); 
            };
            
            if (isNew) {
                Title = "Add New Template"; 
            } else {
                Title = "Edit Template";
            }
            
            DataContext = viewModel;
        }
    }
}