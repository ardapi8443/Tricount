using prbd_2324_g01.ViewModel;
using PRBD_Framework;
using System.Windows;

namespace prbd_2324_g01.View {
    public partial class SignupView : WindowBase {
        public SignupView() {
            InitializeComponent();
            DataContext = new SignupViewModel();
        }
    }
}