using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class Subscription : EntityBase<PridContext>
{
    [Required, ForeignKey(nameof(User))]
    public int User {  get; private set; }
    public virtual User UserFromSubscription { get; private set; }

    [Required, ForeignKey(nameof(Tricount))]
    public int Tricount { get; private set; }
    public virtual Tricount TricountFromSubscription { get; private set; }

    public Subscription(int User, int Tricount) 
    {
        this.User = User;
        this.Tricount = Tricount;
    }

    public Subscription() { }
}