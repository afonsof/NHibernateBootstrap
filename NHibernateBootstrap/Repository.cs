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

        public virtual IQueryable<TEntity> AsQueryable()
        {
            return Uow.Session.Query<TEntity>();
        }

        public virtual TEntity Find(int id)
        {
            return AsQueryable().FirstOrDefault(e => e.Id == id);
        }

        public virtual void Add(TEntity obj)
        {
            Uow.BeginTransaction();
            Uow.Session.Save(obj);
        }

        public virtual void Edit(TEntity obj)
        {
            Uow.BeginTransaction();
            Uow.Session.Update(obj);
        }

        public virtual void Remove(TEntity obj)
        {
            Uow.BeginTransaction();
            Uow.Session.Delete(obj);
        }

        public virtual void Remove(int id)
        {
            Remove(Find(id));
        }

        public void Commit()
        {
            Uow.Commit();
        }
    }
}