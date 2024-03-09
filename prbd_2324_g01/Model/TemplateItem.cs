using PRBD_Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class TemplateItem : EntityBase<Model> {
    
    public int TemplateItemId { get; set; }
    
    [Required]
    public int Weight { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }
    
    [Required]
    public int TemplateId { get; set; }
    
    [ForeignKey(nameof(TemplateId))]
    public virtual Template Template { get; set; }

    public TemplateItem(int weight, int userId, int templateId) { 
        this.Weight = weight;
        this.UserId = userId;
        this.TemplateId = templateId;
    }

    // A parameterless constructor is needed for Entity Framework
    public TemplateItem() { }
}