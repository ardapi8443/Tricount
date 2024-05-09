using prbd_2324_g01.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace prbd_2324_g01.View
{
    public partial class UserTemplateItemView : UserControl {
        public UserTemplateItemView() {
            Loaded += OnLoaded;
            InitializeComponent();
        }
        
        private void OnLoaded(object sender, RoutedEventArgs e) {
            // Subscribe to the TextChanged event
            weight.TextChanged += WeightOnChange;

            if(DataContext is UserTemplateItemViewModel { FromOperation: false } || weight.Text.Equals("0")) {
                Total.Visibility = Visibility.Collapsed;
            }
            
        }

        //add dynamic behavior to the checkbox
        private void WeightOnChange(object sender, TextChangedEventArgs e) {
            if (weight.Text.Equals("0")) {
                checkBox.IsChecked = false;
                Total.Visibility = Visibility.Collapsed;
            } else if (weight.Text.Equals("1")) {
                checkBox.IsChecked = true;
                if (DataContext is UserTemplateItemViewModel { FromOperation: true }) {
                    Total.Visibility = Visibility.Visible;
                }
            }
        }
    }
}