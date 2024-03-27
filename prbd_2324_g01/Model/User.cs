using PRBD_Framework;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace prbd_2324_g01.Model;

public class User : EntityBase<PridContext> {
    //private static int countUser { get; set; } = 1;
    public int UserId { get; set; }
    public  string FullName { get; set; }
    public string HashedPassword { get; set; }
    public Role Role { get; set; } = Role.Viewer;
    public string email {  get; set; }
    public virtual ICollection<Tricount> TricountCreated { get; set; } = new HashSet<Tricount>();
    public virtual ICollection<Tricount> Tricounts { get; set; }= new HashSet<Tricount>();
    public virtual ICollection<Operation> OperationsCreated { get; set; } = new HashSet<Operation>();
    public virtual ICollection<Operation> Operations { get; set; } = new HashSet<Operation>();
    public virtual ICollection<Template> Templates { get; set; } = new HashSet<Template>();

  
    public User (int id, string FullName, string HashedPassword, string email) {
        this.UserId = id;
        this.FullName = FullName;
        this.HashedPassword = HashedPassword;
        this.email = email;
       
    }

    public User() {
    }

    public void UpdatePwd(String str) {

        this.HashedPassword = SecretHasher.Hash(str);
        this.Persist();

    }
    public void Persist() {

        PridContext.Context.Update(this);
        PridContext.Context.SaveChanges();
    }

    public static User UserById(int id) {
        return PridContext.Context.Users.FirstOrDefault(user => user.UserId == id);
    }
}