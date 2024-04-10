using Azure;
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
    public void Persist() {

        PridContext.Context.Update(this);
        PridContext.Context.SaveChanges();
    }
    public void delete() {
        PridContext.Context.Remove(this);
        PridContext.Context.SaveChanges();
    }
    public static List<Tricount> tricountByMember(User user) {
        return PridContext.Context.Tricounts.Where(t => t.Creator == user.UserId || t.Subscribers.Any(s => s.UserId == user.UserId)).ToList();
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


