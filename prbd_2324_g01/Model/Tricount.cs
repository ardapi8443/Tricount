using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace prbd_2324_g01.Model;

public class Tricount : EntityBase<Model> {

    

    //private static int countTricount { get; set; } = 1;

    [Key]
    public int Id { get; set; } 
    public string Title { get; set; }
    public string Description { get;  set; }
    public DateTime CreatedAt { get; set; }
    //public virtual ICollection<Subscription> Subscriptions { get; set; } = new HashSet<Subscription>();
    public virtual ICollection<User> Subscribers { get;  set; } = new HashSet<User>();
    public virtual ICollection<Template> Templates { get;  set; } = new HashSet<Template>();
    public virtual ICollection<Operation> Operations { get; set; } = new HashSet<Operation>();
    //public virtual ICollection<Repartition> Repartitions { get; set; } = new HashSet<Repartition>();

    [Required, ForeignKey(nameof(CreatorFromTricount))]
    public int Creator { get; set; }

    //[Required]
    public virtual User CreatorFromTricount { get;  set; }

    //public Tricount( string Title, string Description, User Creator) {
    //    //this.Id = countTricount++;
    //    this.Title = Title;
    //    this.Description = Description;
    //    this.CreatorFromTricount = Creator;
    //    this.CreatedAt = DateTime.Now;
    //}

    public Tricount(int id, string Title, string Description, int Creator, DateTime Created_at) {
        this.Id = id;
        this.Title = Title;
        this.Description = Description;
        this.Creator = Creator;
        this.CreatedAt = Created_at;
    }

    override
    public string ToString() {
        return ($"{this.Title} : {this.Creator}");
    }

    public Tricount() { }   

    public static IOrderedQueryable<Tricount> GetTricountByUser(User user) {
        var q = from m in Model.Context.Tricounts
                    //where m.Creator == user.UserId
                orderby m.CreatedAt
                select m;

        return q;
    }
}