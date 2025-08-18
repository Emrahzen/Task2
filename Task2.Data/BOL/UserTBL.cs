using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2.Data.BOL
{
    public class UserTBL
    {
        [Key]
        public int userId { get; set; }
        public string userEmail { get; set; }
        public string userPassword { get; set; }
        public string telephone { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
