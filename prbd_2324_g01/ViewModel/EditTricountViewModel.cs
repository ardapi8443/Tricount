using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using prbd_2324_g01.View;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel
{
    public class EditTricountViewModel : ViewModelCommon {
        
        private Tricount _tricount;
        
        private DateTime? _date;

        public ICommand AddTemplateCommand { get; private set; }
        
        public ICommand AddMySelfCommand { get; private set; }
        public ICommand AddEvryBodyCommand { get; private set; }
        
        //not working
        /*public ICommand SaveCommand { get; private set; }*/
        public ICommand CancelCommand { get; private set; }

        private ObservableCollection<ParticipantViewModel> _participants;
        public ObservableCollection<ParticipantViewModel> Participants {
            get => _participants;
            set {
                if (_participants != value) {
                    _participants = value;
                    RaisePropertyChanged(nameof(Participants));
                }
            }
        }
        private ObservableCollection<TemplateViewModel> _templates;
        public ObservableCollection<TemplateViewModel> Templates {
            get => _templates;
            set {
                if (_templates != value) {
                    _templates = value;
                    RaisePropertyChanged(nameof(Templates));
                }
            }
        }
        
        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value);
        }
        
        public string FormattedDate {
            get => _date?.ToShortDateString();
        }


        public EditTricountViewModel(Tricount tricount) {
            Tricount = tricount;
            
            Register<Template>(App.Messages.MSG_ADD_TEMPLATE, (template) => OnRefreshData());
            
            AddTemplateCommand = new RelayCommand(AddTemplate);
            AddEvryBodyCommand = new RelayCommand(AddEveryBody);
            AddMySelfCommand = new RelayCommand(AddMySelfInParticipant);
            /*SaveCommand = new RelayCommand(SaveAction);*/
            CancelCommand = new RelayCommand(CancelAction,CanCancelAction);
            
            LinqToXaml();
        }

        private void AddEveryBody() {
           
            var usersNotSubscribed = Context.Users
                .Where(user => !Context.Subscriptions
                                   .Any(sub => sub.UserId == user.UserId && sub.TricountId == Tricount.Id) 
                               && user.Role == Role.Viewer)
                .ToList();

            if (!usersNotSubscribed.IsNullOrEmpty()) {
                foreach (var user in usersNotSubscribed) {
                    var newSub = new Subscription() {
                        UserId = user.UserId,
                        TricountId = Tricount.Id
                    };
                    Context.Subscriptions.Add(newSub);
                }
                Context.SaveChanges();
                
            } else {
                Console.WriteLine("EveryOne is AlReady Sub in this Tricount");
            }
            OnRefreshData();
        }

        private void AddMySelfInParticipant() {
            var user = CurrentUser;
            var query = Context.Subscriptions.Where(u => u.UserId == user.UserId);

            if (query.IsNullOrEmpty()) {
                var newSub = new Subscription() {
                    UserId = user.UserId,
                    TricountId = Tricount.Id
                };
                Context.Subscriptions.Add(newSub);
            }
            Console.WriteLine("déja sub");
        }

        private void CancelEditTricount() {
            
        }

        public override void SaveAction() {
            Context.Add(Tricount);
            Context.SaveChanges();
            RaisePropertyChanged();
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Tricount);
        }

        public string Creation {
            get => $"Created by {Tricount.CreatorFromTricount.FullName} on {Tricount.CreatedAt.ToShortDateString()}";
        }

        public DateTime? Date {
            get => Tricount.CreatedAt;
            set => SetProperty(ref _date, value);
        }

        private void AddTemplate() {
            var addTemplateDialog = new AddTemplateView {
                Owner = App.Current.MainWindow
            };
            addTemplateDialog.ShowDialog();
        }
        protected override void OnRefreshData() {
            LinqToXaml();
        }
        
        private bool CanCancelAction() {
            return Tricount != null;
        }
        
        public override void CancelAction() {
            if (!Tricount.IsModified) {
                NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Tricount);
            } else {
                Tricount.Reload();
                RaisePropertyChanged();
            }
        }

        private void LinqToXaml() {
            var templates = PridContext.Context.Templates.ToList();
            
            Templates = new ObservableCollection<TemplateViewModel>(
                templates.Select(t => new TemplateViewModel(t.Title)));
            
            var subscriptions = PridContext.Context.Subscriptions
                .Include(sub => sub.UserFromSubscription)
                .ThenInclude(user => user.OperationsCreated)
                .Where(sub => sub.TricountId == Tricount.Id)
                .OrderBy(sub => sub.UserFromSubscription.FullName)
                .ToList();

            Participants = new ObservableCollection<ParticipantViewModel>(
                subscriptions.Select(sub => {
                    var user = sub.UserFromSubscription;
                    var numberOfExpenses = PridContext.Context.Repartitions
                        .Count(rep => rep.UserId == user.UserId && rep.OperationFromRepartition.TricountId == Tricount.Id);

                    return new ParticipantViewModel(
                        user.FullName, 
                        numberOfExpenses,
                        user.UserId == Tricount.CreatorFromTricount.UserId 
                    );
                })
            );
        }
    }
}