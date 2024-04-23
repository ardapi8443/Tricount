using prbd_2324_g01.ViewModel;
using System.Windows;

namespace prbd_2324_g01.View
{
    public partial class AddTemplateView : Window
    {
        public AddTemplateView() {
            InitializeComponent();
            var viewModel = new AddTemplateViewModel();
            viewModel.RequestClose += (dialogResult) => {
                this.DialogResult = dialogResult;
            };

            DataContext = viewModel;
        }
    }
}