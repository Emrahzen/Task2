using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task2.Data.BOL;
using Task2.Data.DAL;
using Task2.Data.DBContext;

namespace Task2.Data.BLL
{
    public class UserService : GenericRepository<UserTBL>
    {
        public UserService(ApplicationDbContext dbContext)
                : base(dbContext)
        {

        }
    }
}
