using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class Subscription : EntityBase<PridContext> {

    [Required, ForeignKey(nameof(User))]
    public int UserId {  get; set; }
    public virtual User UserFromSubscription { get;  set; }

    [Required, ForeignKey(nameof(Tricount))]
    public int TricountId { get;  set; }
    public virtual Tricount TricountFromSubscription { get; set; }

    public Subscription(int User, int Tricount) {
        this.UserId = User;
        this.TricountId = Tricount;
    }

    public Subscription() { }

    public void Delete() {
        Context.Subscriptions.Remove(this);
        Context.SaveChanges();
    }
}