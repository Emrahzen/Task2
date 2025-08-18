using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2.Data.DTO
{
    public class AppSettings
    {
        public string defaultUsername { get; set; }
        public string defaultPassword { get; set; }
        public string jwtToken { get; set; }
    }
}
