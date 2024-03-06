using PRBD_Framework;

namespace prbd_2324_g01.Model;

public class TemplateItem : EntityBase<PridContext> 
{
    public int Weight { get; private set; }
    [Required, ForeignKey(nameof(User))]
    public int User { get; private set; }
    public virtual User UserFromTemplateItem {  get; private set; }
    
    [Required, ForeignKey(nameof(Template))]
    public int Template { get; private set; }
    public virtual Template TemplateFromTemplateItem { get; private set; }

    public TemplateItem(int Weight, int UserFrom, int Template) { 
        this.Weight = Weight;
        this.UserFrom = UserFrom;
        this.Template = Template;
    }

    public TemplateItem() { }
}