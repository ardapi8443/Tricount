using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class Repartition : EntityBase<Model> {
    [Required]
    public int Weight {  get; set; }

    [Required, ForeignKey(nameof(User))]
    public int User { get; private set; }
    public virtual User UserFromRepartition { get; private set; }

    [Required, ForeignKey(nameof(Tricount))]
    public int Operation { get; private set; }
    public virtual Operation OperationFromRepartition { get; private set; }




    public Repartition (int userId, int Operation, int Weight) {
        this.User = userId;
        this.Tricount = Tricount;
        this.Weight = Weight;
    }

    public Repartition () {}
}