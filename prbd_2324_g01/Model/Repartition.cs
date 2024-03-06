using PRBD_Framework;

namespace prbd_2324_g01.Model;

public class Repartition : EntityBase<PridContext>
{
    [Required, ForeignKey(nameof(User))]
    private int User { get; set; }
    [Required, ForeignKey(nameof(Operation))]
    private int Operation { get; set; }
    private int Weight { get; set; }

    public Repartition (int Operation, int Weight) 
    {
        this.Operation = Operation;
        this.Weight = Weight;
    }

    public Repartition () {}
}