using Microsoft.EntityFrameworkCore;
using sftpservice.Core.IRepositories;
using sftpservice.Persistence.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Core.Repositories
{
    /// <summary>
    /// It is generic class which implements all the methods defined in the <see  cref="IGenericRepository{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly SFTPDbContext _context;
        private readonly DbSet<T> _dbCollection;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        //public GenericRepository(SFTPDbContext context)
        //{
        //    _context = context;
        //    _dbCollection = _context.Set<T>();
        //}
        public GenericRepository(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _context = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<SFTPDbContext>();
            _dbCollection = _context.Set<T>();

        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? linqExpress = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<string>? includes = null)
        {
            IQueryable<T> query = _dbCollection;
            if (linqExpress != null)
            {
                query = query.Where(linqExpress);
            }

            if (includes != null)
            {
                foreach (var includePropery in includes)
                {
                    query = query.Include(includePropery);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return await query.ToListAsync();
        }

        public async Task<int> AddRangeAsync(List<T> entities)
        {
            await _dbCollection.AddRangeAsync(entities);
            int state = await _context.SaveChangesAsync();
            return state;
        }
    }
}
