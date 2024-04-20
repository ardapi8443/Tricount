using Msn.ViewModel;
using System.Windows.Input;
using prbd_2324_g01;
using PRBD_Framework;

namespace prbd_2324_g01.ViewModel;

public class MainViewModel : ViewModelCommon {
    public ICommand ReloadDataCommand { get; set; }

    public MainViewModel() : base() {
        ReloadDataCommand = new RelayCommand(() => {
            // refuser un reload s'il y a des changements en cours
            if (Context.ChangeTracker.HasChanges()) return;
            // permet de renouveller le contexte EF
            App.ClearContext();
            // notifie tout le monde qu'il faut rafraîchir les données
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        });
    }

    public static string Title {
        get => $"Tricount of ({CurrentUser?.FullName})";
    }

    protected override void OnRefreshData() {
        // pour plus tard
    }
}