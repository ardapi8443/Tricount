using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using prbd_2324_g01.View;
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


    public  double TotalExp() {
            double res = new();
            foreach (Operation ope in Operations) {
                res += ope.Amount;
            }
            return Math.Round(res, 2);
           
    }
    
    [NotMapped]
    public virtual Boolean HaveOpe {
        get {
            Context.Entry(this)
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
            Context.Entry(this)
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
            Context.Entry(this)
           .Collection(t => t.Subscribers)
           .Load();

            if(Subscribers.Count == 1) return 0;
            return Subscribers.Count - 1;
        }
    }
    [NotMapped]
    public virtual Boolean HaveFriends {
        get {
            Context.Entry(this)
              .Collection(t => t.Subscribers)
              .Load();
            if(Subscribers.Count == 1) return false;
            return true;
        }
    }

    [NotMapped]
    public virtual string FriendMessage { get; set; }
    
    [NotMapped]
    public virtual bool IsNew { get; set; }

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

    public static string ValidateTitle(string title) {
        if (string.IsNullOrEmpty(title)) {
            return "Title is required.";
        } else if (title.Length < 3) {
            return "Minimum 3 characters required.";
        }

        return null;
    }

    public string IsDuplicateTitle(string newTitle) {
        if (Context.Tricounts.Any(t => t.Title.Equals(newTitle) && this.Id != t.Id && t.Creator == this.Creator)) {
            return "Title must be unique by Creator.";
        }

        return null;
    }

    public static string ValidateDescription(string description) {
        if (!string.IsNullOrEmpty(description) && description.Length < 3) {
            return "Minimum 3 characters required.";
        }

        return null;
    }
    
    public static string ValidateDate(DateTime date) {
        if (date > DateTime.Today) {
            return "cannot be in the future";
        }

        return null;
    }

    public Tricount(bool IsNew, string Title, string Description, int Creator, DateTime Created_at) {
        this.IsNew = IsNew;
        this.Title = Title;
        this.Description = Description;
        this.Creator = Creator;
        this.CreatedAt = Created_at;
        // this.Subscribers.Add(CreatorFromTricount);
        
    }
    
    public override bool Validate() {
        //pour add tricount

        ClearErrors();

        if (string.IsNullOrWhiteSpace(Title)) {
            
            AddError(nameof(Title), "required");
            
        } else if (!Description.IsNullOrEmpty() &&  Description.Length < 3) {
            
            AddError(nameof(Description), "length must be >= 3");
            
        } else {
            // On ne v�rifie l'unicit� du pseudo que si l'entit� est en mode d�tach� ou ajout�, car
            // dans ces cas-l�, il s'agit d'un nouveau membre.
            if ((IsDetached || IsAdded) && Context.Tricounts.Any(t => t.Title == Title))
                AddError(nameof(Title), "title already exists");
        }
 

        return !HasErrors;
    }


    public void Persist() {
        Context.Update(this);
        Context.SaveChanges();
    }
    public void delete() {
        var tricount = Context.Tricounts.Find(this.Id);
                
        Context.Tricounts.Remove(tricount);
        Context.SaveChanges();
    }
    public static List<Tricount> TricountsByMember(User user) {
        
        List<Tricount> res;
        
        res = user.Role == Role.Administrator ? 
            Context.Tricounts.ToList() : 
            Context.Tricounts.Where(t => t.Creator == user.UserId || t.Subscribers.Any(s => s.UserId == user.UserId)).ToList();
        res.Sort(
            (tricount1, tricount2) => {

                if (tricount1.HaveOpe && tricount2.HaveOpe) {

                    var latestOperationDate1 = tricount1.Operations.Max(operation => operation.OperationDate);
                    var latestOperationDate2 = tricount2.Operations.Max(operation => operation.OperationDate);

                    if (latestOperationDate1 == latestOperationDate2) {
                        
                        return tricount2.CreatedAt.CompareTo(tricount1.CreatedAt);
                        
                    } else {
                        return latestOperationDate2.CompareTo(latestOperationDate1);
                    }
                    
                }  else if (tricount1.HaveOpe) {
                    
                    return -1;

                } else if (tricount2.HaveOpe) {
                    
                    return 1;

                } else {
                    return tricount2.CreatedAt.CompareTo(tricount1.CreatedAt);
                }

            });
   
        
        return res;
    }

    public double UserExpenses(User user) {
        
        double res = new();

        foreach (Operation ope in Operations) {
            double TotalWeight = Context.Repartitions
                .Where(r => r.OperationId == ope.OperationId)
                .Sum(r => r.Weight);
            
            double UserWeight = Context.Repartitions
                .Where(r => r.OperationId == ope.OperationId && r.UserId == user.UserId)
                .Sum(r => r.Weight);
            
            double ratio = UserWeight / TotalWeight;
            
           res =  res + (ope.Amount * ratio);
           
        }
        return Math.Round(res, 2);
    }
    
    public override string ToString() {
        return ($"{this.Title} : {this.Creator}");
    }   

    public static dynamic GetTricountById(int id) {
        var q = from t in Context.Tricounts
                where t.Id == id
                select t;
        return q.First();
    }

    public List<User> GetUsersNotSubscribed() {
        
        List<User> usersNotSubscribed = Context.Users
            .Where(user => !Context.Subscriptions
                               .Any(sub => sub.UserId == user.UserId && sub.TricountId == this.Id) 
                           && user.Role == Role.Viewer)
            .OrderBy(x => x.FullName)
            .ToList();

        return usersNotSubscribed;
    }
        
    public HashSet<User> GetSubscribers() {
        HashSet<User> res = new HashSet<User>();

        var Sub = Context.Subscriptions.Where(s => s.TricountId == this.Id);

        foreach (Subscription s in Sub) {
            res.Add(User.UserById(s.UserId));
        }

        return res;
    }

    public ICollection<User> GetSubscribersForOperation() {
        var query = from t in Context.Tricounts
            where t.Id == Id
            select t.Subscribers;
        var users = query.First();

        return users.OrderBy(u => u.FullName).ToList();
    }

    public List<Template> GetTemplates() {
        var q2 = from t in Context.Templates
            where t.Tricount == this.Id
            select t;
        return q2.OrderBy(t =>t.Title).ToList();
    }

    public List<User> GetUserTemplateItems() {
        // we populate the TemplateItems
        var queryUsersID = from s in Context.Subscriptions
            where s.TricountId == this.Id
            select s.UserId;
        //transform the list of user ids to a list of users
        return queryUsersID.ToList().Select(u => Context.Users.Find(u)).OrderBy(t => t.FullName).ToList();

    }

    public void Add() {
        Context.Add(this);
    }
    
    public dynamic  GetAllSubscribersBalance() {
        var operations = from o in Context.Operations
            where o.TricountId == Id
            group o by o.UserId into g
            orderby g.Key
            select new {
                UserId = g.Key,
                Amount = g.Sum(x => x.Amount)
            };
           
        var query2 = from user in Subscribers
            join op in operations on user.UserId equals op.UserId into operationDetails
            from subOp in operationDetails.DefaultIfEmpty()
            orderby user.FullName
            select new {
                UserId = user.UserId,
                Amount = subOp != null ? subOp.Amount : 0.00 
            };
        
        return query2;
    }
    
    public IQueryable<Operation> GetAllOperation() {
        var q = from o in Context.Operations
            where o.TricountId == this.Id
            select o;
        
        return q.OrderByDescending(x => x.OperationDate)
            .ThenBy(x => x.Title);
    }
}

