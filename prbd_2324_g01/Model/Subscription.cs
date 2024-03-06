using PRBD_Framework;

namespace prbd_2324_g01.Model;

public class Subscription : EntityBase<PridContext>
{
    [Required, ForeignKey(nameof(User))]
    private int User {  get; private set; }
    [Required, ForeignKey(nameof(Tricount))]
    private int Tricount { get; private set; }

    public Subscription(int User, int Tricount) 
    {
        this.User = User;
        this.Tricount = Tricount;
    }

    public Subscription() { }
}