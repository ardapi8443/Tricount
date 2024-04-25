using prbd_2324_g01.Model;
using prbd_2324_g01.ViewModel;
using System.Windows;

namespace prbd_2324_g01.View {
    public partial class AddTemplateView : Window {
        public AddTemplateView(Tricount tricount) {
            InitializeComponent();

            var viewModel = new AddTemplateViewModel(tricount); 
            
            viewModel.RequestClose += (dialogResult) => {
                this.DialogResult = dialogResult;
                this.Close(); 
            };
            
            DataContext = viewModel;
        }
    }
}