using System.Windows;

namespace prbd_2324_g01.View {
    public partial class ConfirmationDialogView : Window {
        
    public ConfirmationDialogView() {
        InitializeComponent();
    }

    private void YesButton_Click(object sender, RoutedEventArgs e) {
        this.DialogResult = true; // User confirmed
        this.Close();
    }

    private void NoButton_Click(object sender, RoutedEventArgs e) {
        this.DialogResult = false; // User cancelled
        this.Close();
    }
}

}