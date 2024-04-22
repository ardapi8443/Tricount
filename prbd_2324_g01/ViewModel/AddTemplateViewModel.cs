using Microsoft.EntityFrameworkCore;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using System.Collections.ObjectModel;

namespace prbd_2324_g01.ViewModel {
    public class AddTemplateViewModel : ViewModelCommon {
        public ObservableCollection<UserTemplateItemViewModel> TemplateItems { get; set; }
        
        public AddTemplateViewModel() {

            var distinctUsers = PridContext.Context.Users
                .Where(u => u.FullName != "Administrator")
                .GroupBy(u => u.FullName)
                .Select(g => g.First())
                .ToList();

            TemplateItems = new ObservableCollection<UserTemplateItemViewModel>(
                distinctUsers.Select(u => new UserTemplateItemViewModel(u.FullName)));
        }
    }
}