using Microsoft.IdentityModel.Tokens;
using prbd_2324_g01.Model;
using System.Collections.ObjectModel;

namespace prbd_2324_g01.ViewModel;

public class MainViewModel : PRBD_Framework.ViewModelBase<User, PridContext>
{
    public string Title => "My Tricount";

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
    private void ApplyFilterAction() {


    }

    private void InitiateVM() {
        IEnumerable<Tricount> tricounts = Tricount.tricountByMember(User.UserById(1));

        foreach (var tricount in tricounts) {
            TricountsDetailVM.Add(new TricountDetailViewModel(tricount,User.UserById(1) ));
        }

        Tricounts = new ObservableCollection<Tricount>(tricounts);
    }
    public MainViewModel() {
        ApplyFilterAction();
        InitiateVM();

    }

}