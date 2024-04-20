using prbd_2324_g01.Model;
using prbd_2324_g01.ViewModel;
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
using System.Windows.Shapes;
using PRBD_Framework;

namespace prbd_2324_g01.View {
    /// <summary>
    /// Interaction logic for TricountView.xaml
    /// </summary>
    public partial class TricountView : UserControlBase {


        public TricountView(Tricount tricount) {
            DataContext  = new TricountViewModel(tricount);
            InitializeComponent();
        }
    }
}
