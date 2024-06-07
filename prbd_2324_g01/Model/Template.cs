using Microsoft.EntityFrameworkCore;
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
   
   public virtual ICollection<TemplateItem> TemplateItems { get; set; }

    public Template(int TemplateId, string Title, int Tricount) { 
    
        this.TemplateId = TemplateId;
        this.Title = Title;
        this.Tricount = Tricount;    
    }

    public Template() { }

    public static bool ValidateUnicity(string title, int tricountId, int templateId) {
        return Context.Templates.Any(t => t.Title == title && t.Tricount == tricountId && t.TemplateId != templateId);
    }

    public static string ValidateTitle(string title) {
        if (string.IsNullOrEmpty(title)) {
            return "Title is required.";
        } else if (title.Length < 3) {
            return "Minimum 3 characters required.";
        }

        return null;
    }

    public static List<Template> templateByTricount(int Tricount) {
        return Context.Templates.Where(template => template.Tricount == Tricount).ToList();
        
    }

    public void Add() {
        Context.Templates.Add(this);
    }

    public void Deleted() {
        Context.Templates.Remove(this);
        Context.SaveChanges();
    }

    public override string ToString() {
        return $"{Title} - {Tricount} ";
    }

    public void Save() {
        Context.Templates.Add(this);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
    }

    public List<TemplateItem> GetTemplateItems() {
       return Context.TemplateItems
            .AsNoTracking()
            .Where(ti => ti.Template == this.TemplateId) 
            .Include(ti => ti.UserFromTemplateItem) 
            .DefaultIfEmpty()
            .ToList();
    }

    public static Template GetTemplateById(int id) {
        return Context.Templates.Find(id);
        
    }

    public bool Exist(User u) {
        return !Context.TemplateItems.Local.Any(
            ti =>
                ti.User == u.UserId && ti.Template == this.TemplateId);
    }

    public void Delete() {
        Context.Templates.Remove(this);
    }
}