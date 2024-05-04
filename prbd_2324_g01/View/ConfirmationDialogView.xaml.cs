using System.Windows;

namespace prbd_2324_g01.View {
    public partial class ConfirmationDialogView : Window
    {
        private string text;
        
    public ConfirmationDialogView(string text) {
        this.text = text;
        Loaded += OnLoaded;
        
        InitializeComponent();
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e) {
        if (text.Equals("tricount")) {
            Text.Text = "You're about to delete this Tricount. \nDo you confirm?;";
        } else if (text.Equals("operation")) {
            Text.Text = "You're about to delete this Operation. \nDo you confirm?";
        }
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