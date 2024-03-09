using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class Template : EntityBase<Model> {
    
    public int TemplateId {  get; set; }
    private string Title { get;  set; }
    
    [Required, ForeignKey(nameof(Tricount))]
    private int Tricount { get; set; }
    public virtual ICollection<User> Users { get; private set; } = new HashSet<User>();

    public virtual Tricount TricountFromTemplate { get; set; }

    public Template(int TemplateId, string Title, int Tricount) { 
    
        this.TemplateId = TemplateId;
        this.Title = Title;
        this.Tricount = Tricount;    
    }

    public Template() { }

}