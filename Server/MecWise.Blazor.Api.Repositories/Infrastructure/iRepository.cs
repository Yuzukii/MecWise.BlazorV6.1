using System.Collections.Generic;

namespace MecWise.Blazor.Api.Repositories
{
    public interface iRepository<T>
    {
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
