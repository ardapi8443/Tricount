using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class Template : EntityBase<Model> {
    
    public int TemplateId {  get; set; }
    private string Title { get;  set; }
    
    [Required, ForeignKey(nameof(Tricount))]
    private int Tricount { get; set; }
    public virtual ICollection<TemplateItem> TemplateItems { get; private set; } = new HashSet<TemplateItem>();

    public Template(int TemplateId, string Title, int Tricount) { 
    
        this.TemplateId = TemplateId;
        this.Title = Title;
        this.Tricount = Tricount;    
    }

    public Template() { }

}