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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;

namespace prbd_2324_g01.ViewModel {
    public class EditTricountViewModel : ViewModelCommon {

        private Tricount _tricount;
        private Template _template;
        private int Myself;
       
        private bool _isNew;
        private string _updatedTitle;
        private string _TitlePlaceHolder;
        public string TitlePlaceHolder {
            get => _TitlePlaceHolder;
            set => SetProperty(ref _TitlePlaceHolder, value);
        }

        private string _DescriptionPlaceHolder;

        public string DescriptionPlaceHolder {
            get => _DescriptionPlaceHolder;
            set => SetProperty(ref _DescriptionPlaceHolder, value);
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

        private string _updatedDescription;
        private List<User> _usersNotSubscribed = new ();
 
        public List<User> UsersNotSubscribed {
            get => _usersNotSubscribed;
            set => SetProperty(ref _usersNotSubscribed, value);
        }
        
        private List<User> _usersSubscribed = new ();
 
        public List<User> UsersSubscribed {
            get => _usersSubscribed;
            set => SetProperty(ref _usersSubscribed, value);
        }

        private List<String> _fullnameNotSubscribed = new();
        private bool newTricount { get; set; }
        
        private User CurrentTricountCreator { get; set; }


        public List<String> FullnameNotSubscribed {
            get => _fullnameNotSubscribed;
            set {
                SetProperty(ref _fullnameNotSubscribed, value);
            }
        }

        public bool IsNew {
            get => _isNew;
            set => SetProperty(ref _isNew, value);
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
                    // SetProperty(ref _date, value);
                    SetProperty(ref _date, value);
                    RaisePropertyChanged(nameof(Date));
                    Validate();
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
                RaisePropertyChanged(nameof(IsTemplatesEmpty));
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
        
        public Visibility IsTemplatesEmpty => Templates == null || !Templates.Any() ? Visibility.Visible : Visibility.Collapsed;
            
        
        public void LoadTemplates() {
            // var templates = Context.Templates
            //     .Where(t => t.Tricount == Tricount.Id)
            //     .ToList();

            List<Template> templates = Tricount.GetTemplates();

            Templates = new ObservableCollectionFast<TemplateViewModel>(
                templates.Select(t => new TemplateViewModel(t, true))
            );
        }
        
        public EditTricountViewModel(Tricount tricount) {
            Tricount = tricount;
            Date = tricount.CreatedAt;
            newTricount = tricount.IsNew;
            CurrentTricountCreator = User.GetUserById(tricount.Creator);
            UsersSubscribed.Add(CurrentTricountCreator);
            UsersSubscribed = UsersSubscribed.OrderBy(x => x.FullName).ToList();
            Myself = CurrentUser.UserId;

            if (tricount.IsNew) {
                UsersNotSubscribed = User.GetAllUserButOne(CurrentTricountCreator);
            } else {
                UsersNotSubscribed = Tricount.GetUsersNotSubscribed();
            }
            
            setFullnameNotSubscribed();
            

            
            Register<TemplateViewModel>(App.Messages.MSG_ADD_TEMPLATE, (TemplateViewModel) => {
                Templates.Add(TemplateViewModel); RaisePropertyChanged(nameof(IsTemplatesEmpty));
            });

            Register<Tricount>(App.Messages.MSG_UPDATE_EDITVIEW, (t) => OnRefreshData());

            Register<TemplateViewModel>(
                App.Messages.MSG_DELETE_TEMPLATE, (TemplateViewModel) => {
                    Templates.Remove(TemplateViewModel); RaisePropertyChanged(nameof(IsTemplatesEmpty));
                });

            Register<TemplateViewModel>(
                App.Messages.MSG_EDIT_TEMPLATE, (TemplateViewModel) => {
                    AddTemplate(Tricount, TemplateViewModel.Template, false, TemplateViewModel.TemplateItems, Templates,true);
                });
            
            
            Register<ParticipantViewModel>(
                App.Messages.MSG_DEL_PARTICIPANT, (PVM) => {
                    DeleteParticipant(PVM);
                });
            
            AddTemplateCommand = new RelayCommand(AddTemplate, CanAddTemplate);
            AddEvryBodyCommand = new RelayCommand(AddEveryBody, CanAddEverybody);
            AddMySelfCommand = new RelayCommand(AddMySelfInParticipant, CanAddMySelfInParticipant);
            SaveCommand = new RelayCommand(SaveAction, CanSaveAction);
            CancelCommand = new RelayCommand(CancelAction, CanCancelAction);
            AddParticipant = new RelayCommand(AddParticipantAction, CanAddParticipantAction);

            LinqToXaml();


            if (tricount.IsNew) {
                TitlePlaceHolder = "<New Tricount>";
                DescriptionPlaceHolder = "No Description";
                
                int numberOfExpenses = Repartition.getExpenseByUserAndTricount(CurrentTricountCreator.UserId, tricount.Id);
                Participants.Add(new ParticipantViewModel(Tricount, CurrentTricountCreator, numberOfExpenses, true));

            } else {
                TitlePlaceHolder = tricount.Title;
                UpdatedTitle = tricount.Title;
                DescriptionPlaceHolder = tricount.Description;
                UpdatedDescription = tricount.Description;
                
            }
        }
        
        private void AddTemplate() {
            var templateItems = new ObservableCollection<UserTemplateItemViewModel>();
            AddTemplate(Tricount, new Template(), true, templateItems, Templates,true);
        }

        private bool CanAddTemplate() {
            return !newTricount;
        }
        
        private void AddParticipantAction() {
            User user = User.GetUserByFullName(SelectedFullName);
            int numberOfExpenses = Repartition.getExpenseByUserAndTricount(user.UserId, Tricount.Id);
            AddParticipants(SelectedFullName,numberOfExpenses);
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
            FullnameNotSubscribed.OrderBy(s => s);
            FullnameNotSubscribed.Sort();
        }
        
        private void AddEveryBody() {
            
            if (!UsersNotSubscribed.IsNullOrEmpty()) {

                foreach (User user in UsersNotSubscribed) {

                    int numberOfExpenses = Repartition.getExpenseByUserAndTricount(user.UserId, Tricount.Id);

                    Participants.Add(
                        new ParticipantViewModel(
                            Tricount, user, numberOfExpenses,
                            user.UserId == CurrentTricountCreator.UserId));
                   
                }
                UsersNotSubscribed.Clear();
                setFullnameNotSubscribed();
            } else {
                Console.WriteLine("Everyone is Already Sub in this Tricount");
            }

            SortPaticipants();
        }

        private bool CanAddEverybody() {
            return !UsersNotSubscribed.IsNullOrEmpty();
        }
        
    private void AddMySelfInParticipant() {
        string user = CurrentUser.FullName;
        int numberOfExpenses = 0;
        AddParticipants(user, numberOfExpenses);
    }

    private void AddParticipants(string usernotsub, int numberOfExpenses) {
        foreach (User u in UsersNotSubscribed) {
            if (u.FullName == usernotsub) {
                Participants. Add(new ParticipantViewModel(Tricount, u, numberOfExpenses, u.UserId == CurrentTricountCreator.UserId));
                UsersNotSubscribed.Remove(u);
                break;
            }
        }
        setFullnameNotSubscribed();
        SortPaticipants();
    }
    
        private bool CanAddMySelfInParticipant() {
            
            foreach (ParticipantViewModel PVM  in Participants) {
                if (Myself == PVM.User.UserId) {
                    return false;
                }
            }

            return true;
            
        }
        

        public override void SaveAction() {
            ClearErrors();
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Tricount);
            
            Tricount.Title = UpdatedTitle;
            Tricount.Description = UpdatedDescription;
            Tricount.CreatedAt = Date;

            if (newTricount) {
                Tricount.IsNew = false;
                Tricount.Add();
                
            }
            
            Context.SaveChanges(); 
            
            NotifyColleagues(App.Messages.MSG_ADD_NEW_TRICOUNT, Tricount);

            foreach (ParticipantViewModel PVM in Participants) {
                if (!Subscription.Exist(Tricount.Id, PVM.User.UserId)) {
                    Console.WriteLine(PVM.User.UserId);
                    Subscription NewSub = new(PVM.User.UserId, Tricount.Id);
                    NewSub.Add();
                }
            }

            SaveTemplate();
            Context.SaveChanges();
            
            if (!IsNew) {
                foreach (User u in UsersNotSubscribed) {
                    Subscription.DeleteIfExist(Tricount.Id, u.UserId);
                }
            }
            
            NotifyColleagues(App.Messages.MSG_TITLE_CHANGED, Tricount);
            NotifyColleagues(App.Messages.MSG_REFRESH_TRICOUNT,Tricount);
            NotifyColleagues(App.Messages.MSG_DISPLAY_TRICOUNT, Tricount);
            
            
        }

        private bool CanSaveAction() {
            return !HasErrors && !string.IsNullOrEmpty(UpdatedTitle);
        }

        public string Creation {
            get => $"Created by {User.GetUserById(CurrentTricountCreator.UserId).FullName} on {Tricount.CreatedAt.ToShortDateString()}";
        }
        

        private void AddTemplate(Tricount tricount, Template template, bool isNew, ObservableCollection<UserTemplateItemViewModel> existingItems, 
            ObservableCollectionFast<TemplateViewModel> templateViewModels, bool fromTemplateView) {
            IsNew = isNew;
            var addTemplateDialog = new AddTemplateView(tricount, template, isNew, existingItems,templateViewModels, fromTemplateView) {
                Owner = App.Current.MainWindow
            };
            addTemplateDialog.ShowDialog();
        }
        
        protected override void OnRefreshData() {
            /*if (IsNew || Tricount == null) return;*/
            if (Tricount.IsNew) {
                ClearErrors();
                NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Tricount);
            } else {
                LinqToXaml();
            }
           
        }
        
        private bool CanCancelAction() {

            return Tricount != null && (IsNew || !Tricount.IsModified);

        }
        
        public override void CancelAction() {
            if (!Tricount.IsModified) {
                ClearErrors();
                NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Tricount);
                if (!Tricount.IsNew) {
                    NotifyColleagues(App.Messages.MSG_DISPLAY_TRICOUNT,Tricount);
                }
                
            } else {
                Tricount.Reload();
                RaisePropertyChanged();
            }
           
        }

        private void LinqToXaml() {
            
            LoadTemplates();
            
            Participants = new ObservableCollectionFast<ParticipantViewModel>(
                Tricount.GetSubscribers().Select(sub => {
      
                    int numberOfExpenses = Repartition.getExpenseByUserAndTricount(sub.UserId, Tricount.Id);
                    
                    return new ParticipantViewModel(
                        Tricount,
                        sub, 
                        numberOfExpenses,
                        sub.UserId == CurrentTricountCreator.UserId 
                    );
                })
            );

            SortPaticipants();

        }
        
        public override bool Validate() {
            ClearErrors();

            string validateTitle = Tricount.ValidateTitle(UpdatedTitle);
            string titleExist = Tricount.IsDuplicateTitle(UpdatedTitle);
            if (validateTitle != null) {
                AddError(nameof(UpdatedTitle), validateTitle);
            }  else if (titleExist != null) {
                AddError(nameof(UpdatedTitle), titleExist);
            } 
            
            string validateDescriptin = Tricount.ValidateDescription(UpdatedDescription);
            if (validateDescriptin != null) {
                AddError(nameof(UpdatedDescription), validateDescriptin);
            }
            
            DateTime TempDate = new DateTime(
                Date.Year, Date.Month, Date.Day, 0, 0, 0);
                        
            string validateDate = Tricount.ValidateDate(TempDate); 
            if (validateDate != null) {
                AddError(nameof(Date), validateDate);
            }
    
            return !HasErrors;
        }
        
        private void DeleteParticipant(ParticipantViewModel PVM) {
            
            Participants.Remove(PVM);
            UsersNotSubscribed.Add(User.GetUserById(PVM.User.UserId));
            UsersNotSubscribed = UsersNotSubscribed.OrderBy(x => x.FullName).ToList();
            setFullnameNotSubscribed();
            PVM.Dispose();

        }

        private void SortPaticipants() {
            Participants = new ObservableCollectionFast<ParticipantViewModel>(Participants.OrderBy(PVM => PVM.Name));

        }
        
        private void SaveTemplate() {
            
         KeepTemplateFromView();
         
         foreach (TemplateViewModel templateViewModel in Templates) {
            // Template existingTemplate = Context.Templates
            //     .Include(t => t.TemplateItems)
            //     .FirstOrDefault(t => t.TemplateId == templateViewModel.Template.TemplateId);

            Template = Template.GetTemplateById(templateViewModel.Template.TemplateId);

            if (Template != null) {
                // Mise à jour du template existant
                Template.Title = templateViewModel.Title;
                
            } else {
                // Création d'un nouveau template
                Template template = new Template {
                    TemplateId = templateViewModel.Template.TemplateId,
                    Title = templateViewModel.Title,
                    Tricount = Tricount.Id,
                    TemplateItems = new List<TemplateItem>()
                };
                template.Add();

                Template = template; 
            }

            foreach (UserTemplateItemViewModel userTemplateItemViewModel in templateViewModel.TemplateItems) {
                User user = User.GetUserByFullName(userTemplateItemViewModel.UserName);
                
                if (user != null) {
                    List<TemplateItem> existingTemplateItems =  TemplateItem.GetAllItemByTemplateID(Template.TemplateId);
                    var item = existingTemplateItems.FirstOrDefault(u => u.User == user.UserId);
                    if (item != null) {
                            item.Weight = userTemplateItemViewModel.Weight;
                    } else {
                        if(Template.Exist(user)) {
                            // Création d'un nouvel item
                            TemplateItem templateItem = new TemplateItem {
                                Weight = userTemplateItemViewModel.Weight,
                                User = user.UserId,
                                Template = Template.TemplateId,
                                TemplateFromTemplateItem = Template,
                                UserFromTemplateItem = user
                            };
                            Template.TemplateItems.Add(templateItem);
                            templateItem.Add();
                        }
                    }
                }
            } 
         } 
        }

        private void KeepTemplateFromView() {
            List<int> templateIdsInViewModels = Templates.Select(t => t.Template.TemplateId).ToList();

            List<Template> templatesInDb = Tricount.GetTemplates();
            
            var templatesToDelete = templatesInDb
                .Where(t => !templateIdsInViewModels.Contains(t.TemplateId))
                .ToList();
            
            foreach (Template templateToDelete in templatesToDelete) {
                templateToDelete.Delete();
                
            }

            Context.SaveChanges();
        }
    }
}