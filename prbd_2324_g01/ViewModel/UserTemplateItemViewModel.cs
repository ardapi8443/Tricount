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
            set => SetProperty(ref _weight, value, 
                () => NotifyColleagues(App.Messages.MSG_WEIGHT_CHANGED));
            
        }
        
        private bool _isChecked;

        public bool IsChecked {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value, () => NotifyColleagues(App.Messages.MSG_CHECKBOX_CHANGED));
        }
        
        public bool FromOperation { get; set; }
        private string _totalPerUser;
        public string TotalPerUser {
            get => _totalPerUser;
            set => SetProperty(ref _totalPerUser, value);
        }
        private int _totalWeight;
        private double _totalAmount;


        public UserTemplateItemViewModel(string userName, int weight, bool isNew, bool fromOperation) {
            UserName = userName;
            Weight = weight;
            FromOperation = fromOperation;
            IncrementCommand = new RelayCommand(IncreaseWeight);
            DecrementCommand = new RelayCommand(DecreaseWeight);
            //add dynamic behavior when the checkbox is clicked
            CheckBoxCommand = new RelayCommand(CheckBoxAction);
            
            if (FromOperation) {
                TotalPerUser = "0";
                
                //register to the event when the amount of the parent changed
                Register<double>(App.Messages.MSG_AMOUNT_CHANGED, (amount) => {
                    _totalAmount = amount;
                    CalculateTotal();
                });
            
                //register to weight changes to recalculate total
                Register<int>(App.Messages.MSG_TOTAL_WEIGHT_CHANGED, (totalWeight) => {
                    _totalWeight = totalWeight;
                    CalculateTotal();
                });
            }
            

            if (Weight > 0) {
                IsChecked = true;
            }
        }

        private void IncreaseWeight() {
            Weight++;
            NotifyColleagues(App.Messages.MSG_TEMP_0);
        }

        private void DecreaseWeight() {
            if (Weight > 0) {
                Weight--;
            }
            NotifyColleagues(App.Messages.MSG_TEMP_0);
        }

        private void CheckBoxAction() {
            if (IsChecked) {
                Weight = 1;
            } else {
                Weight = 0;
            }
            NotifyColleagues(App.Messages.MSG_TEMP_0);
        }
        
        private void CalculateTotal() {
            if (FromOperation) {
                TotalPerUser = (((double)_weight / _totalWeight) * _totalAmount).ToString("F2");
                 //Console.WriteLine($"{UserName} = " + "(" + _weight + " / " + _totalWeight + ") * " + _totalAmount + ")" + " = " + TotalPerUser);
            }
        }
        
    }
}