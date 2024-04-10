using Microsoft.EntityFrameworkCore;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace prbd_2324_g01.ViewModel
{
    internal class TricountViewModel: ViewModelCommon {
        private Tricount _tricount;
        private ObservableCollection<OperationCardViewModel> _operations;
        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value);
        }

        public ObservableCollection<OperationCardViewModel> Operations {
            get => _operations;
            set => SetProperty(ref _operations, value);
        }

        public string Title {
            get => Tricount.Title;
            set => SetProperty(Tricount.Title, value, Tricount, (m, v) => m.Title = v);
        }

        public string Description {
            get => (!string.IsNullOrEmpty(Tricount.Description) ? Tricount.Description : "pas de description");
            set => SetProperty(Tricount.Description, value, Tricount, (m, v) => m.Description = v);
        }

        public string Creation {
            get => $"Created by {Tricount.CreatorFromTricount.FullName} on {Tricount.CreatedAt.ToShortDateString()}";
        }

        public TricountViewModel() {
            Tricount = Tricount.GetTricountById(4);
            var query = from o in PridContext.Context.Operations
                                        where o.TricountId == Tricount.Id
                                        select o;
            foreach (var q in query) {
                Console.WriteLine("query = " + q.Title);
            }
            Operations = new ObservableCollection<OperationCardViewModel>(query.Select(o => new OperationCardViewModel(o)));
        }

    }
}
