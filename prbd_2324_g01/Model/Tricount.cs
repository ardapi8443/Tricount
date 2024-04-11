using Microsoft.EntityFrameworkCore;
using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;

namespace prbd_2324_g01.Model;

public class Tricount : EntityBase<PridContext> {


    [Key]
    public int Id { get; set; } 
    public string Title { get; set; }
    public string Description { get;  set; }
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<User> Subscribers { get;  set; } = new HashSet<User>();
    public virtual ICollection<Template> Templates { get;  set; } = new HashSet<Template>();
    public virtual ICollection<Operation> Operations { get; set; } = new HashSet<Operation>();

    [Required, ForeignKey(nameof(CreatorFromTricount))]
    public int Creator { get; set; }
    public virtual User CreatorFromTricount { get;  set; }


    public Tricount(int id, string Title, string Description, int Creator, DateTime Created_at) {
        this.Id = id;
        this.Title = Title;
        this.Description = Description;
        this.Creator = Creator;
        this.CreatedAt = Created_at;
    }

    public Tricount() { }

    public override bool Validate() {
        //pour add tricount

        ClearErrors();

        if (string.IsNullOrWhiteSpace(Title))
            AddError(nameof(Title), "required");
        else if (Description.Length < 3)
            AddError(nameof(Description), "length must be >= 3");
        else
            // On ne v�rifie l'unicit� du pseudo que si l'entit� est en mode d�tach� ou ajout�, car
            // dans ces cas-l�, il s'agit d'un nouveau membre.
            if ((IsDetached || IsAdded) && Context.Tricounts.Any(t => t.Title == Title))
            AddError(nameof(Title), "title already exists");

        return !HasErrors;
    }


    public void Persist() {
        PridContext.Context.Update(this);
        PridContext.Context.SaveChanges();
    }

    public void delete() {
        PridContext.Context.Remove(this);
        PridContext.Context.SaveChanges();
    }

    
    public override string ToString() {
        return ($"{this.Title} : {this.Creator}");
    }   

    public static IQueryable<int> GetTricountsIdByUser(User user) {
        var q = from t in PridContext.Context.Tricounts
                where t.Creator == user.UserId
                orderby t.CreatedAt
                select t.Id;

        return q;
    }

    public dynamic GetTricountCard() {
        //manque balance !!!
        //sous requete � faire � part dans user en envoyant le tricountId en param�tre
        int Id = this.Id;
        var q = from t in PridContext.Context.Tricounts
                where t.Id == Id
                let lastOperationDate = t.Operations.OrderByDescending(x => x.OperationDate).FirstOrDefault()
                let friendsCount = t.Subscribers.Count()
                let operationsCount = t.Operations.Count()
                let operationsAmount = t.Operations.Sum(x => x.Amount)
                select new { t.Title, t.Description, Creator = t.CreatorFromTricount.FullName, t.CreatedAt, 
                    lastOperationDate, friendsCount , operationsCount, operationsAmount };
        return q.ToList();
    }

    public static dynamic GetTricountOperations(int id) {
        var q = from o in PridContext.Context.Operations
                where o.TricountId == id
                select o;
        return q;
    }

    public static dynamic GetTricountById(int id) {
        var q = from t in PridContext.Context.Tricounts
                where t.Id == id
                select t;
        return q.First();
    }
    
    public double ConnectedUserExp(User user) {
        
        double res = new();

        foreach (Operation ope in Operations) {
            double TotalWeight = PridContext.Context.Repartitions
                .Where(r => r.OperationId == ope.OperationId)
                .Sum(r => r.Weight);
            
            double UserWeight = PridContext.Context.Repartitions
                .Where(r => r.OperationId == ope.OperationId && r.UserId == user.UserId)
                .Sum(r => r.Weight);
            
            double ratio = UserWeight / TotalWeight;
            
            res =  res + (ope.Amount * ratio);
           
        }
        return Math.Round(res, 2);
    }
    
    public double ConnectedUserBal(User user) {

        Double connectedUserExp = ConnectedUserExp(user);
        Double expenseUserCo = new();

        foreach (Operation ope in user.OperationsCreated) {

            if (ope.TricountId == Id) {
                expenseUserCo = expenseUserCo + ope.Amount;
            }
        }
        
        return Math.Round(expenseUserCo - connectedUserExp, 2);
    }
}