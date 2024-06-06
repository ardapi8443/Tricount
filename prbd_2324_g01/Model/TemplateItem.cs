using Microsoft.EntityFrameworkCore;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class TemplateItem : EntityBase<PridContext> {

    [Required]
    public int Weight { get; set; }

    [Required, ForeignKey(nameof(User))]
    public int User { get; set; }
    public virtual User UserFromTemplateItem { get; set; }

    [Required, ForeignKey(nameof(Template))]
    public int Template { get; set; }
    public virtual Template TemplateFromTemplateItem { get; set; }

    public TemplateItem(int weight, int User, int Template) { 
        this.Weight = weight;
        this.User = User;
        this.Template = Template;
    }

    // A parameterless constructor is needed for Entity Framework
    public TemplateItem() { }

    public void Add() {
        Context.TemplateItems.Add(this);
    }

    public static List<TemplateItem> GetAllItemByTemplateID(int id) {
        return Context.TemplateItems
            .Where(ti => ti.Template == id)
            .Include(ti => ti.UserFromTemplateItem)
            .ToList();
    }
}

