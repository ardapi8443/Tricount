using Msn.ViewModel;
using PRBD_Framework;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel {
    public class UserTemplateItemViewModel : ViewModelCommon {
        public string UserName { get; set; }
        private int _weight;

        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }
        public ICommand CheckBoxCommand { get; }

        public int Weight {
            get => _weight;
            set => SetProperty(ref _weight, value);
            
        }
        
        private bool _isChecked;

        public bool IsChecked {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }


        public UserTemplateItemViewModel(string userName, int weight, bool isNew) {
            UserName = userName;
            Weight = weight;
            IncrementCommand = new RelayCommand(IncreaseWeight);
            DecrementCommand = new RelayCommand(DecreaseWeight);
            //add dynamic behavior when the checkbox is clicked
            CheckBoxCommand = new RelayCommand(CheckBoxAction);

            if (!isNew && Weight > 0) {
                IsChecked = true;
            }
        }

        private void IncreaseWeight() {
            Weight++;
        }

        private void DecreaseWeight() {
            if (Weight > 0) {
                Weight--;
            }
        }

        private void CheckBoxAction() {
            if (IsChecked) {
                Weight = 1;
            } else {
                Weight = 0;
            }
        }
    }
}