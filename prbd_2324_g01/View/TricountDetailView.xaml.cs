using prbd_2324_g01.Model;
using prbd_2324_g01.ViewModel;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace prbd_2324_g01.View
{
  
    public partial class TricountDetailView : UserControlBase
    {
        public TricountDetailViewModel ViewModel { get; set; }
        
        public TricountDetailView()
        {
            InitializeComponent();
            Loaded += Onloaded;
            
        }

        private void Onloaded(object sender, RoutedEventArgs e) {
            ColorUserBal();
        }
        private void ColorUserBal() {
            
            if (DataContext is TricountDetailViewModel viewModel) {
                if (viewModel.Tricount.ConnectedUserBal(viewModel.ConnectedUSer) > 0) {
                    UserBal.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 200, 125));
                    FullCard.Background = new SolidColorBrush(Color.FromArgb(128, 60, 179, 113));
                } else if (viewModel.Tricount.ConnectedUserBal(viewModel.ConnectedUSer) < 0) {
                    UserBal.Foreground = new SolidColorBrush(Color.FromArgb(255, 220, 20, 60));
                    FullCard.Background = new SolidColorBrush(Color.FromArgb(128, 220, 20, 60));
                } else {
                    FullCard.Background = Brushes.Gray;
                }
            }
        }
    }
}
