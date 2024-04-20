using PRBD_Framework;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Windows.Controls;

namespace prbd_2324_g01.Model;

public class User : EntityBase<PridContext> {
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
    


    public static User GetUserById(int id) {
        var q = from u in PridContext.Context.Users
            where u.UserId == id
            select u;
        return q.First();
    }

    public override bool Validate() {
        //pour login

        //comment ne retirer les erreurs que d'un champs particulier ?
        //      (nameof(Email) => ne fonctionne pas
        ClearErrors();

        var user = from u in Context.Users
                   where u.email.Equals(email)
                   select u.email;

        if (string.IsNullOrEmpty(email))
            AddError(nameof(email), "required");
        else if (!email.Contains("@") || !email.Contains("."))
            AddError(nameof(email), "email not valid");
        else if (!user.Any())
            AddError(nameof(email), "does not exist");
        else
            // On ne vérifie l'unicité du pseudo que si l'entité est en mode détaché ou ajouté, car
            // dans ces cas-là, il s'agit d'un nouveau membre.
            if ((IsDetached || IsAdded) && Context.Users.Any(m => m.email == email))
            AddError(nameof(email), "email already used");

        return !HasErrors;
    }


    public void UpdatePassword(String str) {

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