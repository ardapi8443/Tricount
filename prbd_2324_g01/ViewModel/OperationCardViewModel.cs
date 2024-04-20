using Msn.ViewModel;
using prbd_2324_g01.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_2324_g01.ViewModel {
    internal class OperationCardViewModel : ViewModelCommon {
        private readonly Operation _operation;

        public Operation Operation {
            get => _operation;
            private init => SetProperty(ref _operation, value);
        }

        public string Title {
            get => Operation.Title;
        }

        public string Amount {
            get => Math.Round(Operation.Amount, 2).ToString("0.00"+ " €");
        }

        public string PaidBy {
            get => "Paid by " + Operation.Initiator.FullName;
        }

        public string Date {
            get => Operation.OperationDate.ToShortDateString();
        }

        public OperationCardViewModel(Operation operation) {
            Operation = operation;
        }
    }
}
