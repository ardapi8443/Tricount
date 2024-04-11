using prbd_2324_g01.ViewModel;
using PRBD_Framework;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace prbd_2324_g01.View
{
    public partial class UserAmountView : UserControlBase
    {
        public UserAmountView() {
            InitializeComponent();
            Loaded += OnLoaded;
        }
        
        private void OnLoaded(object sender, RoutedEventArgs e) {
            // Check the value of Amount and adjust Grid.Column accordingly
            if (DataContext is UserAmountViewModel viewModel) {
                if (viewModel.Amount < 0) {
                    Grid.SetColumn(name, 1);
                    Grid.SetColumn(amount, 0);
                }
            }
        }
    }
}