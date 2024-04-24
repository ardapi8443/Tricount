
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
        
        IEnumerable<Tricount> query = Tricount.tricountByMember(CurrentUser);
        
        if (!string.IsNullOrEmpty(Filter)) {
            query = from t in Tricounts
                where t.Title.Contains(Filter)
                      || t.Description.Contains(Filter)
                      || User.UserById(t.Creator).FullName.Contains(Filter)
                      || t.Subscribers.Any(sub => sub.FullName.Contains(Filter))
                      || t.Operations.Any(ope => ope.Title.Contains(Filter))
                select t;
        }

        Tricounts = new ObservableCollection<Tricount>(query);

        TricountsDetailVM.Clear();
        
        foreach (var tricount in Tricounts) {
            TricountsDetailVM.Add(new TricountDetailViewModel(tricount,CurrentUser));
        }


    }
    private void InitiateVM() {
        IEnumerable<Tricount> tricounts = Tricount.tricountByMember(CurrentUser);

        foreach (var tricount in tricounts) {
            TricountsDetailVM.Add(new TricountDetailViewModel(tricount,CurrentUser));
        }

        Tricounts = new ObservableCollection<Tricount>(tricounts);
    }


    public TricountsViewModel() {
        
        AddTricount = new RelayCommand(AddTricountAction);
        
        ClearFilter = new RelayCommand(() => Filter = "");
        InitiateVM();
        ApplyFilterAction();

        DisplayTricountDetails = new RelayCommand<TricountDetailViewModel>(vm => {
            NotifyColleagues(App.Messages.MSG_DISPLAY_TRICOUNT, vm.Tricount);
        });
    }
    
    public void AddTricountAction() {
        
        NotifyColleagues(App.Messages.MSG_DISPLAY_EDIT_TRICOUNT, new Tricount(true, DateTime.Now));
       
    }

}