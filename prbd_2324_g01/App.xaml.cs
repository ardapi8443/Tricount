﻿using prbd_2324_g01.Model;
using prbd_2324_g01.ViewModel;
using PRBD_Framework;
using System.Windows;
using System.Globalization;
using System.Windows.Controls;

namespace prbd_2324_g01;

public partial class App {

    public enum Messages
    {
        MSG_NEW_TRICOUNT,
        MSG_TRICOUNT_CHANGED,
        MSG_DISPLAY_EDIT_TRICOUNT,
        MSG_EDIT_TEMPLATE,
        MSG_DISPLAY_TRICOUNT,
        MSG_DISPLAY_NEW_TRICOUNT,
        MSG_DISPLAY_OPERATION,
        MSG_CLOSE_TAB,
        MSG_LOGIN,
        MSG_UPDATE_EDITVIEW,
        MSG_LOGOUT,
        MSG_NEW_OPERATION,
        MSG_DELETE_TEMPLATE,
        MSG_REFRESH_TRICOUNT,
        MSG_SIGNUP,
        MSG_TITLE_CHANGED,
        MSG_AMOUNT_CHANGED,
        MSG_WEIGHT_CHANGED,
        MSG_TOTAL_WEIGHT_CHANGED,
        MSG_CHECKBOX_CHANGED,
        MSG_DEL_PARTICIPANT,
        MSG_ADD_TEMPLATE,
        MSG_ADD_TEMPLATE_OPE,
        REM_NEW_TRICOUNT,
        MSG_ADD_NEW_TRICOUNT,
        MODIFED_TEMPLATE,
        MODIFED_PARTICIPANT
    }

    public App() {
        var ci = new CultureInfo("fr-BE") {
            DateTimeFormat = {
                ShortDatePattern = "dd/MM/yyyy",
                DateSeparator = "/"
            },
            NumberFormat = {
                NumberDecimalSeparator = "."
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

            NavigateTo<MainViewModel, User, PridContext>();
        });
        
        Register(this, Messages.MSG_LOGOUT, () => {
            Logout();
            NavigateTo<LoginViewModel, User, PridContext>();
        });

        Register<User>(this, Messages.MSG_SIGNUP, user => {
            Login(user);
            NavigateTo<SignupViewModel, User, PridContext>();
        });

        NavigateTo<LoginViewModel, User, PridContext>();
    }

    public static void PrepareDatabase() {
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
        // var user = (from u in Context.Users
        //     where u.Email.Equals("xapigeolet@epfc.eu")
        //     select u).First();
        //
        // Console.WriteLine(user.FullName);

    }
}