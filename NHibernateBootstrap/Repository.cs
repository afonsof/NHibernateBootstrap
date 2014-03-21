using System.Linq;
using NHibernate.Linq;

namespace NHibernateBootstrap
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : IHaveId
    {
        protected readonly UnitOfWorkBase Uow;

        public Repository(UnitOfWorkBase uow)
        {
            Uow = uow;
        }

        public TEntity Find(int id)
        {
            return Uow.Session.Query<TEntity>().FirstOrDefault(e => e.Id == id);
        }

        public virtual void Add(TEntity obj)
        {
            Uow.BeginTransaction();
            Uow.Session.Save(obj);
        }

        public void Remove(TEntity obj)
        {
            Uow.BeginTransaction();
            Uow.Session.Delete(obj);
        }

        public virtual void Edit(TEntity obj)
        {
            Uow.BeginTransaction();
            Uow.Session.Update(obj);
        }

        public void Remove(int id)
        {
            Remove(Find(id));
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return Uow.Session.Query<TEntity>();
        }
    }
}