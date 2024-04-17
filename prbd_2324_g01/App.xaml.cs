using prbd_2324_g01.Model;
using prbd_2324_g01.ViewModel;
using PRBD_Framework;
using System.Windows;
using System.Globalization;

namespace prbd_2324_g01;

public partial class App {
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

        NavigateTo<MainViewModel, User, PridContext>();
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
 

    }
}