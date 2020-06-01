using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApp.Models;

namespace UserApp.ViewModels
{
    public class UserDetailsViewModel
    {
        public UserClass User { get; set; }
        public string PageTitle { get; set; }
        public string FileType { get; set; }
    }
}
