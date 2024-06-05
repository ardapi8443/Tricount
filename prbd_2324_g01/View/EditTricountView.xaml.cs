using prbd_2324_g01.Model;
using prbd_2324_g01.ViewModel;
using PRBD_Framework;
using System.Windows;
using System.Windows.Controls;

namespace prbd_2324_g01.View {
    public partial class EditTricountView : UserControlBase {
        
        public EditTricountView(Tricount tricount) {
            InitializeComponent();
            DataContext = new EditTricountViewModel(tricount);
        } 
   
    }
}