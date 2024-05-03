using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using prbd_2324_g01;
using prbd_2324_g01.Model;
using prbd_2324_g01.View;
using prbd_2324_g01.ViewModel;
using PRBD_Framework;
using prbd_2324_g01;

namespace prbd_2324_g01.View;

public partial class MainView : WindowBase {
    
    Tricount openTricount;
    public MainView() {
        InitializeComponent();

        Register<Tricount>(App.Messages.MSG_NEW_TRICOUNT,
            member => DoDisplayTricount(member, true));

        Register<Tricount>(App.Messages.MSG_DISPLAY_TRICOUNT,
            member => DoDisplayTricount(member, false));

        Register<Operation>(App.Messages.MSG_DISPLAY_OPERATION,
            (operation) => DoDisplayOperation(operation, openTricount,  false));
        
        Register<Operation>(App.Messages.MSG_NEW_OPERATION,
            (operation) => DoDisplayOperation(operation, openTricount, true));
        
        Register<Tricount>(App.Messages.MSG_DISPLAY_EDIT_TRICOUNT,
            member => DoDisplayEditTricount(member, false));
        
        // Register<Tricount>(App.Messages.MSG_PSEUDO_CHANGED,
        //     member => DoRenameTab(string.IsNullOrEmpty(member.Pseudo) ? "<New Member>" : member.Pseudo));

        Register<Tricount>(App.Messages.MSG_CLOSE_TAB,
            tricount => DoCloseTab(tricount));
    }

    private void DoDisplayTricount(Tricount tricount, bool isNew) {
        if (tricount != null) {
            openTricount = tricount;
            OpenTab(isNew ? "<New Tricount>" : tricount.Title, tricount.Title, () => new TricountView(tricount));
        }
    }

    private void DoDisplayOperation(Operation operation, Tricount tricount, bool isNew) {
        App.ShowDialog<AddEditOperationViewModel, Operation, PridContext>(operation, tricount, isNew);
    }

    private void DoDisplayEditTricount(Tricount tricount, bool isNew) {
        if (tricount != null) {
            DoCloseTab(tricount);
            OpenTab(isNew ? "<New Tricount>" : tricount.Title, tricount.Title, () => new EditTricountView(tricount));
        }
    }

    private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
        var tab = tabControl.FindByTag(tag);
        if (tab == null)
            tabControl.Add(createView(), header, tag);
        else
            tabControl.SetFocus(tab);
    }

    private void DoRenameTab(string header) {
        if (tabControl.SelectedItem is TabItem tab) {
            MyTabControl.RenameTab(tab, header);
            tab.Tag = header;
        }
    }

    private void DoCloseTab(Tricount tricount) {
        tabControl.CloseByTag(string.IsNullOrEmpty(tricount.Title) ? "<New Tricount>" : tricount.Title);
    }

    private void MenuLogout_Click(object sender, System.Windows.RoutedEventArgs e) {
         NotifyColleagues(App.Messages.MSG_LOGOUT);
    }

    private void WindowBase_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Q && Keyboard.IsKeyDown(Key.LeftCtrl))
            Close();
    }

    // Nécessaire pour pouvoir Dispose tous les UC et leur VM
    protected override void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
        tabControl.Dispose();
    }
}
