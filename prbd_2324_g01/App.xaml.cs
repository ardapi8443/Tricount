using prbd_2324_g01.Model;
using prbd_2324_g01.ViewModel;
using PRBD_Framework;
using System.Windows;
using System.Globalization;

namespace prbd_2324_g01;

public partial class App {

    public enum Messages
    {
        MSG_NEW_TRICOUNT,
        MSG_TRICOUNT_CHANGED,
        MSG_DISPLAY_TRICOUNT,
        MSG_DISPLAY_OPERATION,
        MSG_CLOSE_TAB,
        MSG_LOGIN
    }

    public App() {
        var ci = new CultureInfo("fr-BE") {
            DateTimeFormat = {
                ShortDatePattern = "dd/MM/yyyy",
                DateSeparator = "/"
            }
        };
        CultureInfo.DefaultThreadCurrentCulture = ci;
        CultureInfo.DefaultThreadCurrentUICulture = ci;
        CultureInfo.CurrentCulture = ci;
        CultureInfo.CurrentUICulture = ci;
    }
    
    protected override void OnStartup(StartupEventArgs e) {
        PrepareDatabase();
        TestQueries();

        Register<User>(this, Messages.MSG_LOGIN, user => {
            Login(user);
            NavigateTo<TricountViewModel, User, PridContext>();
        });

        NavigateTo<LoginViewModel, User, PridContext>();
    }

    private static void PrepareDatabase() {
        // Clear database and seed data
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();


        // Cold start
        Console.Write("Cold starting database... ");
        Console.WriteLine("done");
        
    }

    protected override void OnRefreshData() {
        // TODO
    }

    private static void TestQueries() {
 

        
        foreach(Operation o in Operation.OperationByTricount(4)){
            Console.WriteLine(o.Title);
        }

        Console.WriteLine("");

        foreach (Operation o in Operation.OperationByTricount(6)) {
            Console.WriteLine(o.Title);
        }

    }
}