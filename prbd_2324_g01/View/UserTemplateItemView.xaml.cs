using prbd_2324_g01.ViewModel;
using System.Windows;
using System.Windows.Controls;

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
        }

        //add dynamic behavior to the checkbox
        private void WeightOnChange(object sender, TextChangedEventArgs e) {
            if (weight.Text.Equals("0")) {
                checkBox.IsChecked = false;
            } else if (weight.Text.Equals("1")) {
                checkBox.IsChecked = true;
            }
        }
    }
}