using Microsoft.EntityFrameworkCore;
using prbd_2324_g01.Model;
using PRBD_Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_g01.Model;

public class Operation : EntityBase<PridContext> {

    [Key]
    public int OperationId { get; set; }
    
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
    
    public double Amount { get; set; } 
    
    public DateTime OperationDate { get;  set; }

    [Required, ForeignKey(nameof(User))] 
    public int UserId { get;  set; }
    public virtual User Initiator { get;  set; }

    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();

    [Required, ForeignKey(nameof(Tricount))]
    public int TricountId { get; set; }

    //public virtual ICollection<Repartition> Repartitions { get;  set; } = new HashSet<Repartition>();
    //public virtual Tricount TricountFromOperation { get;  set; }



    public Operation(int id, string title, int tricount, double amount, DateTime operation_date, int initiator) {
        this.OperationId = id;
        this.Title = title;
        this.TricountId = tricount;
        this.Amount = amount;
        this.OperationDate = operation_date;
        this.UserId = initiator;
    }

    public Operation() { }

    public static List<Operation> OperationByTricount(int tricount) {
        return Context.Operations.Where(ope => ope.TricountId == tricount).ToList();
    }

    public static Operation NewestOperationByTricount(int tricount) {

        return Context.Operations
                .Where(ope => ope.TricountId == tricount)
                .OrderBy(ope => ope.OperationDate)
                .FirstOrDefault();
    }

    public ICollection<User> GetUsers() {
        var query = from o in Context.Operations
            where o.OperationId == OperationId
            select o.Users;

        var users = query.First();
        return users.OrderBy(u => u.FullName).ToList();
    }

    public List<Repartition> GetRepartitionItems() {
        return Context.Repartitions
            .AsNoTracking()
            .Where(r => r.OperationId == this.OperationId)
            .ToList();
    }

    public List<User> GetUserTemplateItems() {
        return Context.Tricounts
            .Where(t => t.Id == this.TricountId)
            .SelectMany(t => t.Subscribers)
            .OrderBy(t => t.FullName)
            .ToList();

    }

    public void Delete() {
        // Fetch the operation
        var operation = Context.Operations.Find(this.OperationId);
        // Delete the operation
        Context.Operations.Remove(operation);
        Context.SaveChanges();
    }

    public void Add() {
        Context.Operations.Add(this);
    }

    public static Operation GetOperationById(int id) {
        return Context.Operations.Find(id);
    }
}