using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2.Data.DTO
{
    public class UserAuth
    {
        public string token { get; set; }

        public int userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string userEmail { get; set; }
        public string telephone { get; set; }
    }
}
