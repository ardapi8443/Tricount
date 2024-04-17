using prbd_2324_g01.Model;
using prbd_2324_g01;
using PRBD_Framework;

namespace Msn.ViewModel;

public abstract class ViewModelCommon : ViewModelBase<User, PridContext>
{
    public static bool IsAdmin => App.IsLoggedIn && App.CurrentUser is Administrator;

    public static bool IsNotAdmin => !IsAdmin;
}
