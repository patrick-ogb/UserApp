using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UserApp.Models;

namespace UserApp.ViewModels
{
    public class UserCreateViewModel
    {
        [Required]
        [Display(Name = "Firt Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        public List<IFormFile> Photos { get; set; }

        public IEnumerable<UserClass> UserList { get; set; }
        public string FileType { get; set; }
    }
}
