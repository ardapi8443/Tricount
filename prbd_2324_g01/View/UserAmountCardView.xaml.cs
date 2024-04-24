using prbd_2324_g01.ViewModel;
using PRBD_Framework;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace prbd_2324_g01.View
{
    public partial class UserAmountCardView : UserControlBase
    {
        public UserAmountCardView() {
            InitializeComponent();
            Loaded += OnLoaded;
            
            // Console.WriteLine(name.Width);
        }
        
        private void OnLoaded(object sender, RoutedEventArgs e) {
            // Check the value of Amount and adjust Grid.Column accordingly
            if (DataContext is UserAmountCardViewModel viewModel) {
                if (viewModel.Amount < 0) {
                    Grid.SetColumn(name, 1);
                    Grid.SetColumn(border, 0);
                    
                    name.HorizontalAlignment = HorizontalAlignment.Left;
                    border.HorizontalAlignment = HorizontalAlignment.Right;
                    amount.HorizontalAlignment = HorizontalAlignment.Right;
                    
                    border.Background = Brushes.Red;
                    
                }

                if (viewModel.Amount == 0) {
                    border.Background = Brushes.Transparent;
                } // à recalculer en fonction du montant total des opérations, du poids et expenses du user
// do we ??
                border.Width = Math.Abs(viewModel.Amount)*2;
            }
        }
    }
}