using PRBD_Framework;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace prbd_2324_g01.Model;

public class User : EntityBase<PridContext> {
    public int UserId { get; set; }
    public  string FullName { get; set; }
    public string HashedPassword { get; set; }
    public Role Role { get; set; } = Role.Viewer;
    public string Email {  get; set; }
    public virtual ICollection<Tricount> TricountCreated { get; set; } = new HashSet<Tricount>();
    public virtual ICollection<Tricount> Tricounts { get; set; }= new HashSet<Tricount>();
    public virtual ICollection<Operation> OperationsCreated { get; set; } = new HashSet<Operation>();
    public virtual ICollection<Operation> Operations { get; set; } = new HashSet<Operation>();
    public virtual ICollection<Template> Templates { get; set; } = new HashSet<Template>();
    public User (int id, string FullName, string HashedPassword, string email) {
        this.UserId = id;
        this.FullName = FullName;
        this.HashedPassword = HashedPassword;
        this.Email = email;
       
    }
    public User() { }

    public static string ValidateEmailForLogin(string email) {
        if (string.IsNullOrEmpty(email))
            return "required";
        else if (!email.Contains('@') || !email.Contains('.'))
            return "email not valid";
        else if (!IsMailExist(email))
            return "does not exist";
        return null;
    }

    public static string ValidateEmailForSignup(string email) {
        if (string.IsNullOrEmpty(email))
            return "required";
        else if (!email.Contains('@') || !email.Contains('.'))
            return "email not valid";
        else if (IsMailExist(email))
            return "Already used";
        
        return null;
    }
    
    private static bool IsMailExist(string Email) {
        return (from u in Context.Users
            where u.Email.Equals(Email)
            select u.Email).Any();
    }

    public static string ValidatePasswordForLogin(string passwd) {
        if (string.IsNullOrEmpty(passwd))
            return "required";
        else if (passwd.Length < 3)
            return "length must be >= 3";
        return null;
    }

    public static string ValidatePasswordForSignup(string passwd) {
        if (string.IsNullOrWhiteSpace(passwd)) {
            return "Password is required.";
        } else if (!ValidatePasswordComplexity(passwd))
            return "Password does not meet requirements.";
        else if (passwd.Length < 3)
            return "length must be >= 3";

        return null;
    }
    
    private static bool ValidatePasswordComplexity(string password) {
        string PasswordPattern =
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}";
        return new Regex(PasswordPattern).IsMatch(password);
    }

    public static string ValidatePseudo(string pseudo) {
        if (string.IsNullOrEmpty(pseudo))
            return "required";
        else if (pseudo.Length < 3 )
            return "length must be >= 3";
        else if (IsPseudoExist(pseudo))
            return "This Pseudo is not available";
        return null;
    }
    
    private static bool IsPseudoExist(string pseudo) {
        return Context.Users.Any(u => u.FullName.Equals(pseudo));
    }

    public static User GetUserById(int id) {
        var q = from u in Context.Users
            where u.UserId == id
            select u;
        return q.First();
    }

    public override bool Validate() {
        //pour login

        //comment ne retirer les erreurs que d'un champs particulier ?
        //      (nameof(Email) => ne fonctionne pas
        ClearErrors();

        var user = from u in Context.Users
                   where u.Email.Equals(Email)
                   select u.Email;

        if (string.IsNullOrEmpty(Email))
            AddError(nameof(Email), "required");
        else if (!Email.Contains("@") || !Email.Contains("."))
            AddError(nameof(Email), "email not valid");
        /*else if (!user.Any())
            AddError(nameof(Email), "does not exist");*/
        else
            // On ne vérifie l'unicité du pseudo que si l'entité est en mode détaché ou ajouté, car
            // dans ces cas-là, il s'agit d'un nouveau membre.
            if ((IsDetached || IsAdded) && Context.Users.Any(m => m.Email == Email))
            AddError(nameof(Email), "email already used");

        return !HasErrors;
    }


    public void UpdatePassword(String str) {

        this.HashedPassword = SecretHasher.Hash(str);
        this.Persist();
    }
    public void Persist() {

        Context.Update(this);
        Context.SaveChanges();
    }

    public static User UserById(int id) {
        return Context.Users.FirstOrDefault(user => user.UserId == id);
    }
    
    public static User GetUserByFullName(string Fullname) {
        return  Context.Users.FirstOrDefault(u => u.FullName == Fullname);
    }

    public double getExpenseByTricount(int id) {
        var q = from o in Context.Operations
                let tricountId = id
                let userId = this.UserId
                where o.TricountId == tricountId
                  &&  o.UserId == userId
                  select o;
        return q.Sum(x => x.Amount);
    }

    public static List<User> GetAllUser() {
        return  Context.Users.ToList();
    }

    //renvoie la balance du user pour un tricount donné déjà arrondie à 2 décimales
    public double GetBalanceByTricount(int tricountId) {
        //Pour calculer la balance globale du tricount, vous devez considérer que chaque participant au tricount possède un "compte" virtuel dont le solde est nul au départ.
        //Ensuite, pour chacune des dépenses du tricount, vous effectuez les opérations suivantes :

        //répartir le montant de la dépense proportionnellement au poids de chaque participant à la dépense et imputer la part de chacun en négatif sur son compte ;
        //imputer le montant total de la dépense en positif sur le compte de celui qui a fait le paiement.
        
        //au départ, balance null
        double balance = 0;
        
        var operations = from o in Context.Operations
                where o.TricountId == tricountId
                select o;

        //pour chaque opérations :
        foreach (var operation in operations) {
            //trouver le poids total de l'opération
            int poidsTotal = Context.Repartitions
                .Where(r => r.OperationId == operation.OperationId)
                .Sum(r => r.Weight);
            
            //pour la répartition du user :
            var repartition = from r in Context.Repartitions
                where r.OperationId == operation.OperationId && r.UserId == this.UserId
                select r;
            
            //si le user participe à l'opération (=> à une répartition)
            if (!repartition.IsNullOrEmpty()) {
                //on lui impute le montant de l'opération au prorata de son poids
                balance -= operation.Amount * repartition.First().Weight / poidsTotal;
                
                //si le user a payé l'opération, on lui rajoute le montant de l'opération
                if (operation.Initiator.UserId == this.UserId) {
                    balance += operation.Amount;
                    //Console.WriteLine("INITIATOR");
                }
            }
            if (tricountId == 4) {
                //Console.WriteLine( "operation : " + operation.Title + "\nbalance : " + balance + 
                //                   "\noperation amount : " + operation.Amount + "\npoids total : " + poidsTotal);
                //Console.WriteLine();
            }
        }

        //Console.WriteLine($"Balance : " + balance + " user : " + this.FullName + " tricount : " + tricountId);
            
        
        return Math.Round(balance, 2);
    }

    public static List<User> GetAllUserButOne(User creator) {
        List<User> AllButOne = Context.Users
            .Where(user =>user.UserId != creator.UserId  && user.Role == Role.Viewer)
            .OrderBy(x => x.FullName)
            .ToList();

        return AllButOne;
    }

    public static int GetUserIdFromUserName(string UserName) {
        return Context.Users.Where(u => u.FullName == UserName).Select(u => u.UserId).FirstOrDefault();
    }

    public static User GetUserByMail(string mail) {
        return (from u in Context.Users
            where u.Email.Equals(mail)
            select u).First();
    }
    
    public static string GetHashPassword(string Email) {
        return (from u in Context.Users
            where u.Email.Equals(Email)
            select u.HashedPassword).First();
    }

    public void AddToContext() {
        Context.Add(this);
    }
}