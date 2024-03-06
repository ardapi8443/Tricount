using PRBD_Framework;

namespace prbd_2324_g01.Model;

public class User : EntityBase<PridContext>
{

    public enum Role
    {
        Viewer = 0,
        Administrator = 1
    }

    public int UserId { get; private set; }
    public  string FullName { get; private set; }
    public string HashedPassword { get; private set; }
    public Role Role { get; private set; } = Role.Viewer;
    public string emal {  get; private set; }

    public virtual ICollection<Tricount> Tricounts = new HashSet();
    public virtual ICollection<Operation> Operations  = new HashSet();
    public virtual ICollection<Repartitions> Repartitions { get; set; } = new HashSet();

    public User (string FullName, string HashedPassword, string email) 
    {
        this.FullName = FullName;
        this.HashedPassword = HashedPassword;
        this.emal = email;
       
    }

    public User() { }
}