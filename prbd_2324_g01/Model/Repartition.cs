using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class Repartition : EntityBase<PridContext> {

    [Required]
    public int Weight {  get; set; }

    [Required, ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public virtual User UserFromRepartition { get; set; }

    [Required, ForeignKey(nameof(Operation))]
    public int OperationId { get; set; }
    public virtual Operation OperationFromRepartition { get; set; }


    public Repartition (int userId, int Operation, int Weight) {
        this.UserId = userId;
        this.OperationId = Operation;
        this.Weight = Weight;
    }

    public Repartition () {}

    public static int getExpenseByUserAndTricount(int UserId, int TricountId) {
        
        return PridContext.Context.Repartitions.Count(rep => rep.UserId == UserId && rep.OperationFromRepartition.TricountId == TricountId);


    }
}