using Azure;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Operation = prbd_2324_g01.Model.Operation;

namespace prbd_2324_g01.ViewModel {
    public class AddEditOperationViewModel : DialogViewModelBase<Operation, PridContext> {
        public ICommand ApplyTemplate { get; set; }
        public ICommand SaveTemplate { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand AddOperation { get; set; }

        private Operation _operation;
        private string _title;
        private double _amount;
        //combobox !!
        private ObservableCollection<User> _users;
        private DateTime _date;
        private ObservableCollection<Template> _templates;

        public Operation Operation {
            get => _operation;
            set => SetProperty(ref _operation, value);
        }
        
        public string Title {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public double Amount {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public ObservableCollection<User> Users {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public DateTime Date {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public ObservableCollection<Template> Templates {
            get => _templates;
            set => SetProperty(ref _templates, value);
        }

        public AddEditOperationViewModel(Operation operation) {
//voir new member
            Operation = operation;
            Title = operation.Title;
            Amount = operation.Amount;
            Date = operation.OperationDate;
        }
        
        public AddEditOperationViewModel() {}
    }
}