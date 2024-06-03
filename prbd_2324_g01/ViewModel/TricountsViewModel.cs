
using Microsoft.IdentityModel.Tokens;
using prbd_2324_g01.Model;
using System.Collections.ObjectModel;
using Msn.ViewModel;
using PRBD_Framework;
using System.Windows.Input;


namespace prbd_2324_g01.ViewModel;

public class TricountsViewModel : ViewModelCommon {
    
    public ICommand AddTricount { get; set; }
    
    private ObservableCollection<Tricount> _tricounts;
    public ObservableCollection<Tricount> Tricounts {
        get => _tricounts;
        set {
            SetProperty(ref _tricounts, value);
            RaisePropertyChanged(nameof(Tricounts));
        } 
    }

    private string _filter;
    public string Filter {
        get => _filter;
        set => SetProperty(ref _filter, value, ApplyFilterAction);
    }
    
    public ICommand ClearFilter { get; set; }
    public ObservableCollection<TricountDetailViewModel> TricountsDetailVM { get; set; } = new ();
    
    public ICommand DisplayTricountDetails { get; set; }
    private void ApplyFilterAction() {
        IEnumerable<Tricount> query;
        
        if (!string.IsNullOrEmpty(Filter)) {
            string lowerFilter = Filter.ToLower();
            
            query = from t in Tricounts
                where t.Title.ToLower().Contains(lowerFilter)
                      || t.Description.ToLower().Contains(lowerFilter)
                      || User.UserById(t.Creator).FullName.ToLower().Contains(lowerFilter)
                      || t.Subscribers.Any(sub => sub.FullName.ToLower().Contains(lowerFilter))
                      || t.Operations.Any(ope => ope.Title.ToLower().Contains(lowerFilter))
                select t;
        } else {
            query = Tricount.tricountByMember(CurrentUser);
        }

        Tricounts = new ObservableCollection<Tricount>(query);

        TricountsDetailVM.Clear();
        
        foreach (var tricount in Tricounts) {
            TricountsDetailVM.Add(new TricountDetailViewModel(tricount));
        }


    }
    private void InitiateVM() {
        IEnumerable<Tricount> tricounts = Tricount.tricountByMember(CurrentUser);

        foreach (var tricount in tricounts) {
            TricountsDetailVM.Add(new TricountDetailViewModel(tricount));
        }

        Tricounts = new ObservableCollection<Tricount>(tricounts);
    }


    public TricountsViewModel() {
        
        Register<Tricount>(App.Messages.MSG_REFRESH_TRICOUNT, (tricount) => ClearFilterBeforeApplyFilter());
        
        
        AddTricount = new RelayCommand(AddTricountAction);
        
        ClearFilter = new RelayCommand(() => Filter = "");
        InitiateVM();
        ApplyFilterAction();
        
        DisplayTricountDetails = new RelayCommand<TricountDetailViewModel>(vm => {
            NotifyColleagues(App.Messages.MSG_DISPLAY_TRICOUNT, vm.Tricount);
        });
    }
    private void ClearFilterBeforeApplyFilter() {
        Filter = "";
        ApplyFilterAction();
    }
    
    public void AddTricountAction() {
        

        Tricount tricount = new Tricount(true,"" ,"", CurrentUser.UserId, DateTime.Now);
        
        NotifyColleagues(App.Messages.MSG_DISPLAY_NEW_TRICOUNT, tricount);
       
    }
    
    protected override void OnRefreshData() {
        ApplyFilterAction();
    }

}