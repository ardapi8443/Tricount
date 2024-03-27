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

    private void ApplyFilterAction() {
        IEnumerable<Tricount> tricouts = Tricount.tricountByMember(User.UserById(1));

        Tricounts = new ObservableCollection<Tricount>(tricouts);
    }
    public MainViewModel() {
        ApplyFilterAction();
    }
}