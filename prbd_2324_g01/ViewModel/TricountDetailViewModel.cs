﻿using prbd_2324_g01.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_2324_g01.ViewModel
{
   public class TricountDetailViewModel : PRBD_Framework.ViewModelBase<User, PridContext>
    {
        public Tricount Tricount { get; set; }
        public User ConnectedUSer { get; set; }
        public string Updated_Description { get; set; }

        public string FriendMessage { get; set; }
        public string OpeMessage { get; set; }
        public string TotalExp { get; set; }
        public string UserExp { get; set; }
        public string UserBal { get; set; }

        public TricountDetailViewModel(Tricount Tricount, User ConnectedUSer) {

            this.Tricount = Tricount;
            this.ConnectedUSer = ConnectedUSer;

            if(Tricount.Description.Length == 0 ) {
                Updated_Description = "No Description";
            } else {
                Updated_Description = Tricount.Description;
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

            TotalExp = Tricount.TotalExp.ToString() + " €";
            UserExp = Tricount.ConnectedUserExp(this.ConnectedUSer) + " €";
            UserBal = Tricount.ConnectedUserBal(this.ConnectedUSer) + " €";
        }
        
        
    }
}