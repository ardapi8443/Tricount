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


}