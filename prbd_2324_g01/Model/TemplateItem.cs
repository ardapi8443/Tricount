using prbd_2324_g01.Model;
using PRBD_Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class TemplateItem : EntityBase<Model> {

    [Required]
    public int TemplateItemId { get; set; }
    
    [Required]
    public int Weight { get; set; }

    [Required, ForeignKey(nameof(User))]
    public int User { get; private set; }
    public virtual User UserFromTemplateItem { get; private set; }

    [Required, ForeignKey(nameof(Template))]
    public int Template { get; private set; }
    public virtual Template TemplateFromTemplateItem { get; private set; }

    public TemplateItem(int weight, int User, int Template) { 
        this.Weight = weight;
        this.User = User;
        this.Template = Template;
    }

    // A parameterless constructor is needed for Entity Framework
    public TemplateItem() { }
}

