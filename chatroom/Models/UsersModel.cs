using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace chatroom.Models
{
    public class UsersModel
    {
        public int Id { get; set; }
  
        public string FirstName { get; set; }
     
        public string LastName { get; set; }
         public string UserName { get; set; }
     
        public string Email { get; set; }
   
        public string Password { get; set; }
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ProfileP { get; set; }
        public string ProfilePicture { get; set; }
    }
}