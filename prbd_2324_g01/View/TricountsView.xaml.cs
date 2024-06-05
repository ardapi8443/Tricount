using Microsoft.EntityFrameworkCore.Internal;
using PRBD_Framework;
using System.Windows;
using System.Windows.Media;

namespace prbd_2324_g01.View;

public partial class TricountsView
{
    public TricountsView() {
        InitializeComponent();
        Loaded += Onloaded;
        
    }
    private void Onloaded(object sender, RoutedEventArgs e) {

        
        gray.Background =  Brushes.LightGray;
        green.Background = new SolidColorBrush(Color.FromArgb(255, 200, 230, 190));
        red.Background = new SolidColorBrush(Color.FromArgb(255, 255, 230, 210));
        
    }
}