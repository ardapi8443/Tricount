using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class Template : EntityBase<PridContext> {
    
    public int TemplateId {  get; set; }
    public string Title { get;  set; }
    
    [Required, ForeignKey(nameof(Tricount))]
    //[Required, ForeignKey(nameof(TricountFromTemplate))]
    public int Tricount { get; set; }

    public virtual ICollection<User> Users { get;  set; } = new HashSet<User>();

   public virtual Tricount TricountFromTemplate { get; set; }

    public Template(int TemplateId, string Title, int Tricount) { 
    
        this.TemplateId = TemplateId;
        this.Title = Title;
        this.Tricount = Tricount;    
    }

    public Template() { }

    public static List<Template> templateByTricount(int Tricount) {
        return PridContext.Context.Templates.Where(template => template.Tricount == Tricount).ToList();
        
    }

}