
using Microsoft.IdentityModel.Tokens;
using prbd_2324_g01.Model;
using System.Collections.ObjectModel;
using Msn.ViewModel;
using PRBD_Framework;
using System.Windows.Input;


namespace prbd_2324_g01.ViewModel;

public class TricountsViewModel : ViewModelCommon {

    private ObservableCollection<Tricount> _tricounts;
    public ObservableCollection<Tricount> Tricounts {
        get => _tricounts;
        set => SetProperty(ref _tricounts, value);
    }

    private string _filter;
    public string Filter {
        get => _filter;
        set => SetProperty(ref _filter, value, ApplyFilterAction);
    }
    public ObservableCollection<TricountDetailViewModel> TricountsDetailVM { get; set; } = new ();
    
    public ICommand DisplayTricountDetails { get; set; }
    private void ApplyFilterAction() {


    }

    private void InitiateVM() {
        IEnumerable<Tricount> tricounts = Tricount.tricountByMember(CurrentUser);

        foreach (var tricount in tricounts) {
            TricountsDetailVM.Add(new TricountDetailViewModel(tricount,CurrentUser));
        }

        Tricounts = new ObservableCollection<Tricount>(tricounts);
    }


    public TricountsViewModel() {
        ApplyFilterAction();
        InitiateVM();

        DisplayTricountDetails = new RelayCommand<TricountDetailViewModel>(vm => {
            NotifyColleagues(App.Messages.MSG_DISPLAY_TRICOUNT, vm.Tricount);
        });
    }

}