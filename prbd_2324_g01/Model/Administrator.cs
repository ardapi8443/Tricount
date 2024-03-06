using PRBD_Framework;

namespace prbd_2324_g01.Model;

public class Administrator : User 
{
    protected Administrator() { }

    public Administrator(string FullName, string HashedPassword, string email) 
    {
        // base == this(); java
        :base (FullName, HashedPassword, email);
        Role = Role.Administrator;
    }

}