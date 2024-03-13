using prbd_2324_g01.Model;
using PRBD_Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class Operation : EntityBase<Model> {

    [Key]
    public int OperationId { get; set; }
    
    private string _title;
    public string Title {
        get => _title;
        set {
            if (value.Length > 256) { 
                throw new ArgumentException("Length exceeded"); 
            }
            _title = value; 
        }
    }
    
    public double Amount { get; set; } 
    
    public DateTime OperationDate { get;  set; }

    [Required, ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public virtual User Initiator { get;  set; }

    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();

    [Required, ForeignKey(nameof(Tricount))]
    public int TricountId { get; set; }

    //public virtual ICollection<Repartition> Repartitions { get;  set; } = new HashSet<Repartition>();
    //public virtual Tricount TricountFromOperation { get;  set; }



    public Operation(int id, string title, int tricount, double amount, DateTime operation_date, int initiator) {
        this.OperationId = id;
        this.Title = title;
        this.TricountId = tricount;
        this.Amount = amount;
        this.OperationDate = operation_date;
        this.UserId = initiator;
    }

    public Operation() { } 
}