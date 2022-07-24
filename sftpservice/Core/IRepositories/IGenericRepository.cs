using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Core.IRepositories
{
    /// <summary>
    /// It is generic interface which helps to create loosely coupled solution and gives the data related operations interfaces.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T> where T : class
    {
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? linqExpress = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<string>? includes = null);
        Task<int> AddRangeAsync(List<T> entities);
    }
}
