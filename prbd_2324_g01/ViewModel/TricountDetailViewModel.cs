using prbd_2324_g01.Model;
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
        public Tricount tricount { get; set; }
        public string Updated_Description { get; set; }


        public TricountDetailViewModel(Tricount tricount) {

            this.tricount = tricount;

            if(tricount.Description.Length == 0 ) {
                Updated_Description = "No Description";
            } else {
                Updated_Description = tricount.Description;
            }




           //if (tricounts.Any()) { }

           //     if (tricount.Description.Length == 0) {
           //         tricount.Description = "No Description";
           //     }
           //     if (!tricount.haveFriends) {
           //         tricount.FriendMessage = "With no friend";
           //     } else if (tricount.NumFriends == 1) {
           //         tricount.FriendMessage = "With " + tricount.NumFriends + " friend";
           //     } else {
           //         tricount.FriendMessage = "With " + tricount.NumFriends + " friends"; ;
           //     }
            


        }
    }
}