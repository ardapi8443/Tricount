using Microsoft.EntityFrameworkCore;
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
        
        public ObservableCollection<ParticipantViewModel> Participants { get; private set; }
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
            AddTemplateCommand = new RelayCommand(AddTemplate);
            
            Register<Template>(App.Messages.MSG_ADD_TEMPLATE, (template) => OnRefreshData());
            
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

            var templates = PridContext.Context.Templates.ToList();
            
            Templates = new ObservableCollection<TemplateViewModel>(
                templates.Select(t => new TemplateViewModel(t.Title)));
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
            var templates = PridContext.Context.Templates.ToList();
            Templates = new ObservableCollection<TemplateViewModel>(
                templates.Select(t => new TemplateViewModel(t.Title)));
        }
        
    }
}