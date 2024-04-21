using Microsoft.EntityFrameworkCore;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using System.Collections.ObjectModel;

namespace prbd_2324_g01.ViewModel
{
    public class EditTricountViewModel : ViewModelCommon {
        
        private Tricount _tricount;
        
        public ObservableCollection<ParticipantViewModel> Participants { get; private set; }
        
        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value);
        }

        public EditTricountViewModel(Tricount tricount) {
            Tricount = tricount;

            var subscriptions = PridContext.Context.Subscriptions
                .Include(sub => sub.UserFromSubscription)
                .ThenInclude(ope => ope.OperationsCreated)
                .Where(sub => sub.TricountId == Tricount.Id)
                .OrderBy(sub => sub.UserFromSubscription.FullName)
                .ToList(); 
            
            Participants = new ObservableCollection<ParticipantViewModel>(
                subscriptions.Select(sub => new ParticipantViewModel(
                    sub.UserFromSubscription.FullName, 
                    sub.UserFromSubscription.OperationsCreated.Count,
                    sub.UserFromSubscription.UserId ==
                    Tricount.CreatorFromTricount.UserId 
                ))
            );
        }

        public string Creation {
            get => $"Created by {Tricount.CreatorFromTricount.FullName} on {Tricount.CreatedAt.ToShortDateString()}";
        }

        public string Date {
            get => Tricount.CreatedAt.ToShortDateString();
        }
    }
}