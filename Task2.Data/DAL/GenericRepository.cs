using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task2.Data.DBContext;

namespace Task2.Data.DAL
{
    public class GenericRepository<T> where T : class
    {
        private ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(T obj)
        {
            _context.Add(obj);
            _context.SaveChanges();

            _context.Entry<T>(obj).State = EntityState.Detached; 
        }
        public void CreateRange(List<T> obj)
        {
            _context.AddRange(obj);
            _context.SaveChanges();

        }

        public void Delete(T id)
        {
            _context.Set<T>().Remove(id);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var tmp = GetById(id);

            _context.Set<T>().Remove(tmp);
            _context.SaveChanges();
        }

        public void DeleteSelectedAll(IEnumerable<T> obj)
        {
            _context.Set<T>().RemoveRange(obj);
            _context.SaveChanges();
        }
        public void DeleteAll()
        {
            var dbSet = _context.Set<T>();
            _context.RemoveRange(dbSet);
            _context.SaveChanges();
        }

        public IQueryable<T> GetAll()
        {
            DbSet<T> set = _context.Set<T>();
            return set;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = GetAll().Where(predicate);
            return includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public T GetById(int id)
        {
            var obj = _context.Set<T>().Find(id);
            return obj;
        }

        public void Update(T obj)
        {
            _context.Update<T>(obj);

            _context.SaveChanges();

            _context.Entry<T>(obj).State = EntityState.Detached;

        }

        public void UpdateRange(List<T> obj)
        {
            _context.UpdateRange(obj.AsEnumerable());
            _context.SaveChanges();
        }
    }
}
