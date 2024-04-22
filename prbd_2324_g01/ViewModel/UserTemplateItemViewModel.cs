using Msn.ViewModel;

namespace prbd_2324_g01.ViewModel
{
    public class UserTemplateItemViewModel : ViewModelCommon {
        public string UserName { get; set; }
        private int _weight;

        public int Weight {
            get => _weight;
            set => SetProperty(ref _weight, value);
        }

        public UserTemplateItemViewModel(string userName) {
            UserName = userName;
        }
    }
}