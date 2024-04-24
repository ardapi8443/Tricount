using Azure;
using Microsoft.EntityFrameworkCore;
using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.AccessControl;

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
    private  bool _haveOpe = false;

    [NotMapped]
    public virtual double TotalExp {
        get {
            double res = new();
            foreach (Operation ope in Operations) {
                res += ope.Amount;
            }

            return Math.Round(res, 2);
        }    
    }
    
    [NotMapped]
    public virtual Boolean HaveOpe {
        get {
            PridContext.Context.Entry(this)
              .Collection(t => t.Operations)
              .Load();
            if (Operations.Count == 0) {
                return false;
            } else { return true; }
        }
        set { _haveOpe = value; }
  
    }
    [NotMapped]
    public virtual DateTime? LatestOpe {
        
        get {
            PridContext.Context.Entry(this)
             .Collection(t => t.Operations)
             .Load();

            if (Operations.Count == 0) {
                return null;
            } else { 
                return Operations.OrderByDescending(x => x.OperationDate).First().OperationDate;
            }
        }
    }
    [NotMapped]
    public virtual int NumFriends {

        get {
            PridContext.Context.Entry(this)
           .Collection(t => t.Subscribers)
           .Load();

            if(Subscribers.Count == 1) return 0;
            return Subscribers.Count - 1;
        }
    }
    [NotMapped]
    public virtual Boolean haveFriends {
        get {
            PridContext.Context.Entry(this)
              .Collection(t => t.Subscribers)
              .Load();
            if(Subscribers.Count == 1) return false;
            return true;
        }
    }

    [NotMapped]
    public virtual string FriendMessage { get; set; }

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
    public static List<Tricount> tricountByMember(User user) {
        
        List<Tricount> res = PridContext.Context.Tricounts.Where(t => t.Creator == user.UserId || t.Subscribers.Any(s => s.UserId == user.UserId)).ToList();

        res.Sort(
            (tricount1, tricount2) => {

                if (tricount1.HaveOpe && tricount2.HaveOpe) {

                    var latestOperationDate1 = tricount1.Operations.Max(operation => operation.OperationDate);
                    var latestOperationDate2 = tricount2.Operations.Max(operation => operation.OperationDate);

                    return latestOperationDate2.CompareTo(latestOperationDate1);

                } else if (tricount1.HaveOpe) {

                    var latestOperationDate1 = tricount1.Operations.Max(operation => operation.OperationDate);
                    var creationTricount2 = tricount2.CreatedAt;

                    return creationTricount2.CompareTo(latestOperationDate1);

                } else if (tricount2.HaveOpe) {

                    var creationTricount1 = tricount1.CreatedAt;
                    var latestOperationDate2 = tricount2.Operations.Max(operation => operation.OperationDate);

                    return latestOperationDate2.CompareTo(creationTricount1);

                } else {
                    return tricount2.CreatedAt.CompareTo(tricount1.CreatedAt);
                }

            });
   
        
        return res;
    }

    public double UserExpenses(User user) {
        
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

        Double connectedUserExp = UserExpenses(user);
        Double expenseUserCo = new();

        foreach (Operation ope in user.OperationsCreated) {

            if (ope.TricountId == Id) {
                expenseUserCo = expenseUserCo + ope.Amount;
            }
        }
        
        return Math.Round(expenseUserCo - connectedUserExp, 2);
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
    
}

