using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Xaml.Behaviors.Core;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using prbd_2324_g01.View;
using PRBD_Framework;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace prbd_2324_g01.ViewModel {
    public class EditTricountViewModel : ViewModelCommon
    {

        private Tricount _tricount;
        private Template _template;
        private bool _isNew;
        private string _updatedTitle;
        private string _updatedDescription;
        private List<User> _usersNotSubscribed = new ();
 
        public List<User> UsersNotSubscribed {
            get => _usersNotSubscribed;
            set => SetProperty(ref _usersNotSubscribed, value);
        }

        private List<String> _fullnameNotSubscribed = new();
        private bool newTricount { get; set; }
        

        public List<String> FullnameNotSubscribed {
            get => _fullnameNotSubscribed;
            set => SetProperty(ref _fullnameNotSubscribed, value);
        }

        public bool IsNew {
            get => _isNew;
            set => SetProperty(ref _isNew, value);
        }

        public string UpdatedTitle {
            get => _updatedTitle;
            set {
                if (_updatedTitle != value) {
                    _updatedTitle = value;
                    RaisePropertyChanged(nameof(UpdatedTitle));
                    Validate();
                }
            }
        }

        public string UpdatedDescription {
            get => _updatedDescription;
            set {
                if (_updatedDescription != value) {
                    _updatedDescription = value;
                    RaisePropertyChanged(nameof(UpdatedDescription));
                    Validate();
                }
            }
        }
        
        private String _selectedFullName;
        public String SelectedFullName
        {
            get => _selectedFullName;
            set
            {
                SetProperty(ref _selectedFullName, value);
                RaisePropertyChanged(nameof(SelectedFullName)); 
            }
        }

        private DateTime _date;

        public DateTime Date {
            get => _date;
            set {
                if (_date != value) {
                    SetProperty(ref _date, value);
                    RaisePropertyChanged(nameof(Date));
                }
            }
        }

        public ICommand AddTemplateCommand { get; private set; }

        public ICommand AddMySelfCommand { get; private set; }
        public ICommand AddEvryBodyCommand { get; private set; }
        public ICommand AddParticipant { get; private set; }

        //not working
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private ObservableCollectionFast<ParticipantViewModel> _participants;

        public ObservableCollectionFast<ParticipantViewModel> Participants {
            get => _participants;
            set {
                if (_participants != value) {
                    _participants = value;
                    RaisePropertyChanged(nameof(Participants));
                }
            }
        }
        
        private ObservableCollectionFast<TemplateViewModel> _templates;

        public ObservableCollectionFast<TemplateViewModel> Templates {
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
        
        public Template Template {
            get => _template;
            set => SetProperty(ref _template, value);
        }
        
        public void LoadTemplates() {
            var templates = Context.Templates
                .Where(t => t.Tricount == Tricount.Id)
                .ToList();
            

            Templates = new ObservableCollectionFast<TemplateViewModel>(
                templates.Select(t => new TemplateViewModel(t, true))
            );
            
            
        }
        
        public EditTricountViewModel(Tricount tricount) {
            Tricount = tricount;
            Date = tricount.CreatedAt;
            newTricount = tricount.IsNew;
            
            
            UsersNotSubscribed = Tricount.getUsersNotSubscribed();
            setFullnameNotSubscribed();
            
            Register<TemplateViewModel>(App.Messages.MSG_ADD_TEMPLATE, (TemplateViewModel) => {
                Templates.Add(TemplateViewModel);
            });

            Register<Tricount>(App.Messages.MSG_UPDATE_EDITVIEW, (t) => OnRefreshData());
            
            Register<TemplateViewModel>(
                App.Messages.MSG_UPDATE_TEMPLATE, (TemplateViewModel) => {
                    UpdateTemplateTitle(TemplateViewModel);
                });

            Register<TemplateViewModel>(
                App.Messages.MSG_DELETE_TEMPLATE, (TemplateViewModel) => {
                    Templates.Remove(TemplateViewModel);
                });

            Register<TemplateViewModel>(
                App.Messages.MSG_EDIT_TEMPLATE, (TemplateViewModel) => {
                    AddTemplate(Tricount, TemplateViewModel.Template, false, TemplateViewModel.TemplateItems, Templates);
                });
            
            Register<ParticipantViewModel>(
                App.Messages.MSG_DEL_PARTICIPANT, (PVM) => {
                    DeleteParticipant(PVM);
                });

            AddTemplateCommand = new RelayCommand(() => {
                var templateItems = new ObservableCollection<UserTemplateItemViewModel>();
                AddTemplate(Tricount, new Template(), true, templateItems, Templates);
            });
            AddEvryBodyCommand = new RelayCommand(AddEveryBody, CanAddEverybody);
            AddMySelfCommand = new RelayCommand(AddMySelfInParticipant);
            SaveCommand = new RelayCommand(SaveAction, CanSaveAction);
            CancelCommand = new RelayCommand(CancelAction, CanCancelAction);
            AddParticipant = new RelayCommand(AddParticipantAction, CanAddParticipantAction);

            LinqToXaml();


            if (tricount.IsNew) {
                tricount.Title = "<New Tricount>";
                tricount.Description = "No Description";
                UpdatedTitle = "";
            } else {
                UpdatedTitle = tricount.Title;
                UpdatedDescription = tricount.Description;
            }
        }

        private void UpdateTemplateTitle(TemplateViewModel templateViewModel) {
            
        }

        private void AddParticipantAction() {

            foreach (User u in UsersNotSubscribed) {
                if (u.FullName == SelectedFullName) {
                    int numberOfExpenses = Repartition.getExpenseByUserAndTricount(u.UserId, Tricount.Id);
                    Participants. Add(new ParticipantViewModel(Tricount, u, numberOfExpenses, u.UserId == Tricount.CreatorFromTricount.UserId));
                    UsersNotSubscribed.Remove(u);
                    break;
                }
            }
            
            setFullnameNotSubscribed();
        }

        private bool CanAddParticipantAction() {
            return !UsersNotSubscribed.IsNullOrEmpty();
        }
        
        private void setFullnameNotSubscribed() {
            FullnameNotSubscribed.Clear();
            List<String> res = new();

            if (!UsersNotSubscribed.IsNullOrEmpty()) {
                foreach (User u in UsersNotSubscribed) {
                    res.Add(u.FullName);
                }

            }
            
            FullnameNotSubscribed = res;
        }

        private void DeleteTemplate(Template template) {
            template.Deleted();
            OnRefreshData();
        }

        private void AddEveryBody() {
            
            if (!UsersNotSubscribed.IsNullOrEmpty()) {

                foreach (User user in UsersNotSubscribed) {

                    int numberOfExpenses = Repartition.getExpenseByUserAndTricount(user.UserId, Tricount.Id);

                    Participants.Add(
                        new ParticipantViewModel(
                            Tricount, user, numberOfExpenses,
                            user.UserId == Tricount.CreatorFromTricount.UserId));
                   
                }
                UsersNotSubscribed.Clear();
                setFullnameNotSubscribed();
            } else {
                Console.WriteLine("Everyone is Already Sub in this Tricount");
            }
            
        }

        private bool CanAddEverybody() {
            return !UsersNotSubscribed.IsNullOrEmpty();
        }
        
    private void AddMySelfInParticipant() {
            var user = CurrentUser;
            var alreadySubscribed = Context.Subscriptions
                .Any(s => s.UserId == user.UserId && s.TricountId == Tricount.Id);

            if (!alreadySubscribed) {
                var newSub = new Subscription() {
                    UserId = user.UserId,
                    TricountId = Tricount.Id
                };
                Context.Subscriptions.Add(newSub);
                Context.SaveChanges();
                OnRefreshData();
            } else {
                Console.WriteLine("déja sub");
            }
           
        }

        private void CancelEditTricount() { }

        public override void SaveAction() {
            Tricount.Title = UpdatedTitle;
            Tricount.Description = UpdatedDescription;
            Tricount.CreatedAt = Date;
            
            Console.WriteLine(Context.ChangeTracker.DebugView.LongView);
            
            if (newTricount) {
               Context.Add(Tricount);
            }
            
            Context.SaveChanges();
            
            foreach(ParticipantViewModel PVM in Participants) {
                if (!Subscription.Exist(Tricount.Id, PVM.User.UserId)) {
                    
                    
                    Subscription NewSub = new (PVM.User.UserId, Tricount.Id);
                    NewSub.Add();
                }
            }
            
            if (!IsNew) {
                foreach (User u in UsersNotSubscribed) {
                    Subscription.DeleteIfExist(Tricount.Id, u.UserId);
                }
            
            }
            
            
            NotifyColleagues(App.Messages.MSG_TITLE_CHANGED, Tricount);
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Tricount);
            NotifyColleagues(App.Messages.MSG_REFRESH_TRICOUNT,Tricount);
            NotifyColleagues(App.Messages.MSG_DISPLAY_TRICOUNT, Tricount);
            
            
        }

        public bool CanSaveAction() {
            return !HasErrors && !string.IsNullOrEmpty(UpdatedTitle);
        }

        public string Creation {
            get => $"Created by {Tricount.CreatorFromTricount.FullName} on {Tricount.CreatedAt.ToShortDateString()}";
        }
        

        private void AddTemplate(Tricount tricount, Template template, bool isNew, ObservableCollection<UserTemplateItemViewModel> existingItems, ObservableCollectionFast<TemplateViewModel> templateViewModels) {
            IsNew = isNew;
            var addTemplateDialog = new AddTemplateView(tricount, template, isNew, existingItems,templateViewModels) {
                Owner = App.Current.MainWindow
            };
            addTemplateDialog.ShowDialog();
        }
        
        protected override void OnRefreshData() {
            /*if (IsNew || Tricount == null) return;*/
            LinqToXaml();
        }
        
        private bool CanCancelAction() {

            return Tricount != null && (IsNew || !Tricount.IsModified);

        }
        
        public override void CancelAction() {
            if (!Tricount.IsModified) {
                ClearErrors();
                NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Tricount);
                NotifyColleagues(App.Messages.MSG_DISPLAY_TRICOUNT,Tricount);
            } else {
                Tricount.Reload();
                RaisePropertyChanged();
            }
        }

        private void LinqToXaml() {
            
            LoadTemplates();
            
            Participants = new ObservableCollectionFast<ParticipantViewModel>(
                Tricount.getSubscribers().Select(sub => {
                    var numberOfExpenses = Context.Repartitions
                        .Count(rep => rep.UserId == sub.UserId && rep.OperationFromRepartition.TricountId == Tricount.Id);
            
                    return new ParticipantViewModel(
                        Tricount,
                        sub, 
                        numberOfExpenses,
                        sub.UserId == Tricount.CreatorFromTricount.UserId 
                    );
                })
            );
            
        }
        
        public override bool Validate() {
            ClearErrors();
            
            bool titleExist = Context.Tricounts.Any(t => t.Title.Equals(UpdatedTitle) && Tricount.Id != t.Id);

            if (!string.IsNullOrEmpty(UpdatedDescription) && UpdatedDescription.Length < 3) {
                AddError(nameof(UpdatedDescription), "Minimum 3 characters required.");
            }
            if (string.IsNullOrEmpty(UpdatedTitle)) {
                AddError(nameof(UpdatedTitle), "Title is required.");
            } else if (titleExist) {
                AddError(nameof(UpdatedTitle), "Title must be unique.");
            }
    
            return !HasErrors;
        }
        
        public void DeleteParticipant(ParticipantViewModel PVM) {
            
            Participants.Remove(PVM);
            UsersNotSubscribed.Add(User.GetUserById(PVM.User.UserId));
            setFullnameNotSubscribed();
            PVM.Dispose();

        }
        
        
    }
    
}