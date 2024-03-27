using PRBD_Framework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Controls;

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

    //public virtual ICollection<Subscription> Subscriptions { get; set; } = new HashSet<Subscription>();

    public User (int id, string FullName, string HashedPassword, string email) {
        this.UserId = id;
        this.FullName = FullName;
        this.HashedPassword = HashedPassword;
        this.email = email;
       
    }

    public User() { 
    }

    public double getExpenseByTricount(int id) {
        var q = from o in PridContext.Context.Operations
                let tricountId = id
                let userId = this.UserId
                where o.TricountId == tricountId
                  &&  o.UserId == userId
                  select o;
        return q.Sum(x => x.Amount);
                
    }
}