using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class Repartition : EntityBase<Model> {
    
    public int RepartitionId { get; set; }
    
    [Required, ForeignKey(nameof(UserEntity))]
    public int User { get; set; }
    
    [Required, ForeignKey(nameof(OperationEntity))]
    public int Operation { get; set; }
    public int Weight { get; set; }
    
    public virtual User UserEntity { get; set; }
    public virtual Operation OperationEntity { get; set; }

    public Repartition (int userId, int Operation, int Weight) {
        this.User = userId;
        this.Operation = Operation;
        this.Weight = Weight;
    }

    public Repartition () {}
}