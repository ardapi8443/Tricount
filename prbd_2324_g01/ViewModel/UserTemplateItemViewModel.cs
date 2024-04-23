using Msn.ViewModel;
using PRBD_Framework;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel {
    public class UserTemplateItemViewModel : ViewModelCommon {
        public string UserName { get; set; }
        private int _weight;

        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }

        public int Weight {
            get => _weight;
            set => SetProperty(ref _weight, value);
            
        }


        public UserTemplateItemViewModel(string userName, int weight) {
            UserName = userName;
            Weight = weight;
            IncrementCommand = new RelayCommand(IncreaseWeight);
            DecrementCommand = new RelayCommand(DecreaseWeight);
        }

        private void IncreaseWeight() {
            Weight++;
        }

        private void DecreaseWeight() {
            if (Weight > 0) {
                Weight--;
            }
        }
    }
}