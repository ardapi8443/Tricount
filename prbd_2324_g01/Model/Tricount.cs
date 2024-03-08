using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class Tricount : EntityBase<PridContext> 
{
    public int TricountId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    [Required, ForeignKey(nameof(User))]
    public int Creator { get; private set; }

    public virtual ICollection<User> Subscribers { get; private set; } = new HashSet<User>(); 
    public virtual ICollection<Operation> Operations { get; private set; } = new HashSet<Operation>();
    public virtual ICollection<Repartition> Repartitions { get; private set; } = new HashSet<Repartition>();

    public Tricount(string Title, string Description, int Creator) 
    {
        this.Title = Title;
        this.Description = Description;
        this.Creator = Creator;
        this.CreatedAt = DateTime.Now;
    }

    public Tricount(string Title, string Description, int Creator, DateTime Created_at) {
        this.Title = Title;
        this.Description = Description;
        this.Creator = Creator;
        this.CreatedAt = Created_at;
    }

    public Tricount() { }   
}