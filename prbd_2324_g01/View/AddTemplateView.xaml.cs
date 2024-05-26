using prbd_2324_g01.Model;
using prbd_2324_g01.ViewModel;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows;

namespace prbd_2324_g01.View {
    public partial class AddTemplateView : Window {
        
        private readonly AddTemplateViewModel _viewModel;
        public AddTemplateView(Tricount tricount, Template template, bool isNew, ObservableCollection<UserTemplateItemViewModel> templateItems, ObservableCollectionFast<TemplateViewModel> templateViewModels) {
        
            InitializeComponent();

            _viewModel = new AddTemplateViewModel(tricount, template, isNew, templateItems,templateViewModels); 
        
            _viewModel.RequestClose += ViewModel_RequestClose;
        
            Title = isNew ? "Add New Template" : "Edit Template";
        
            DataContext = _viewModel;
        }

        private void ViewModel_RequestClose(bool? obj) {
            Close();
        }
    }
}