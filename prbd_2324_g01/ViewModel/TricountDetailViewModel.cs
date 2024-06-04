using Microsoft.IdentityModel.Tokens;
using Msn.ViewModel;
using prbd_2324_g01.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_2324_g01.ViewModel
{
   public class TricountDetailViewModel : ViewModelCommon {
        public Tricount Tricount { get; set; }
        public string UpdatedDescription { get; set; }
        public User CurrentUser { get; set; }
        public string FriendMessage { get; set; }
        public string OpeMessage { get; set; }
        public string TotalExp { get; set; }
        public string UserExp { get; set; }
        public string UserBal { get; set; }

        public TricountDetailViewModel(Tricount tricount) {

            Tricount = tricount;
            this.CurrentUser = App.CurrentUser;

            if(Tricount.Description.IsNullOrEmpty()) {
                UpdatedDescription = "No Description";
            } else {
                UpdatedDescription = Tricount.Description;
            }
            
            if (!Tricount.haveFriends) {
                FriendMessage = "With no friend";
            } else if (Tricount.NumFriends == 1) {
                FriendMessage = "With " + Tricount.NumFriends + " friend";
            } else {
                FriendMessage = "With " + Tricount.NumFriends + " friends"; ;
            }

            if (!Tricount.HaveOpe) {
                OpeMessage = "No operation";
            } else if(Tricount.Operations.Count == 1) {
                OpeMessage = Tricount.Operations.Count + " operation";
            } else {
                OpeMessage = Tricount.Operations.Count + " operations";
            }

            TotalExp = Tricount.TotalExp.ToString("0.00") + " €";
            UserExp = Tricount.UserExpenses(CurrentUser).ToString("0.00") + " €";
            //var userBalance = Tricount.ConnectedUserBal(CurrentUser);
            var userBalance = CurrentUser.GetBalanceByTricount(Tricount.Id);
            UserBal = $"{userBalance:0.00} €";
        }
        
    }
}