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
    /// <summary>
    /// Interaction logic for TricountDetailView.xaml
    /// </summary>
    public partial class TricountDetailView : UserControlBase
    {
        public TricountDetailViewModel ViewModel { get; set; }
        public TricountDetailView()
        {
            InitializeComponent();
        }
    }
}
