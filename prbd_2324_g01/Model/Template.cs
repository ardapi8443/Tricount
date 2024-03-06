using PRBD_Framework;

namespace prbd_2324_g01.Model;

public class Template : EntityBase<PridContext>
{
   
    private int TemplateId {  get; private set; }
    private string Title { get; private set; }
    
    [Required, ForeignKey(nameof(Tricount))]
    private int Tricount { get; private set; }

    public Template(int TemplateId, string Title, int Tricount) { 
    
        this.TemplateId = TemplateId;
        this.Title = Title;
        this.Tricount = Tricount;    
    }

    public Template() { }

}