using System.Linq;

namespace NHibernateBootstrap
{
    public interface IRepository<TEntity> where TEntity : IHaveId
    {
        TEntity Find(int id);
        IQueryable<TEntity> AsQueryable();

        void Add(TEntity obj);
        void Edit(TEntity obj);
        void Remove(TEntity obj);
        void Remove(int id);
        void Commit();
    }
}