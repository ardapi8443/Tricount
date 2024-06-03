using Msn.ViewModel;
using System.Windows.Input;
using prbd_2324_g01;
using prbd_2324_g01.Model;
using PRBD_Framework;

namespace prbd_2324_g01.ViewModel;

public class MainViewModel : ViewModelCommon {
    public ICommand ReloadDataCommand { get; set; }
    public ICommand ResetCommand { get; set; }

    public MainViewModel() : base() {
        ReloadDataCommand = new RelayCommand(() => {
            // refuser un reload s'il y a des changements en cours
            if (Context.ChangeTracker.HasChanges()) return;
            // permet de renouveller le contexte EF
            App.ClearContext();
            // notifie tout le monde qu'il faut rafraîchir les données
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
            NotifyColleagues(App.Messages.MSG_REFRESH_TRICOUNT,null);
        });

        ResetCommand = new RelayCommand(() => {
            App.ClearContext();
            App.PrepareDatabase();
            //ajouter tout les App.Messages pour refresh les données
            NotifyColleagues(App.Messages.REM_NEW_TRICOUNT);
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
            NotifyColleagues(App.Messages.MSG_REFRESH_TRICOUNT,null);
        });

    }
    

    public static string Title {
        get => $"Tricount of ({CurrentUser?.FullName})";
    }

    protected override void OnRefreshData() {
        // pour plus tard
    }
    


}