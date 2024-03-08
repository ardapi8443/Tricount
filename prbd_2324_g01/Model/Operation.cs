using prbd_2324_g01.Model;
using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class Operation : EntityBase<PridContext> { 
    
    public int OperationId { get; private set; }
    
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
    
    public double Amount { get; private set; } 
    
    public DateTime Date { get; private set; } = DateTime.Now; 

    [Required, ForeignKey(nameof(User))] 
    public int Initiator { get; private set; }

    [Required, ForeignKey(nameof(User))] 
    public int Tricount { get; private set; } 

    public virtual ICollection<Repartition> Repartitions { get; private set; } = new HashSet<Repartition>(); 

    
    public Operation(int tricount, int initiator, string title, double amount) { 
        Tricount = tricount;
        Initiator = initiator;
        Title = title;
        Amount = amount;
    }

    public Operation() { } 
}