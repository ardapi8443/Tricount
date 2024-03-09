using PRBD_Framework;

namespace prbd_2324_g01.Model;

public class User : EntityBase<PridContext> {
    private static int countUser { get; set; } = 1;
    public int UserId { get; private set; }
    public  string FullName { get; private set; }
    public string HashedPassword { get; private set; }
    public Role Role { get; set; } = Role.Viewer;
    public string email {  get; private set; }

    // need to put accesors for redifinition :: framework
    public virtual ICollection<Tricount> Tricounts { get; private set; }= new HashSet<Tricount>();
    public virtual ICollection<Operation> Operations { get; private set; } = new HashSet<Operation>();
    public virtual ICollection<Repartition> Repartitions { get; private set; } = new HashSet<Repartition>();

    public User (string FullName, string HashedPassword, string email) {
        this.UserId = countUser++;
        this.FullName = FullName;
        this.HashedPassword = HashedPassword;
        this.email = email;
       
    }

    public User() { }
}