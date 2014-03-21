using System;
using NHibernate;

namespace NHibernateBootstrap
{
    public abstract class UnitOfWorkBase : IDisposable
    {
        private ITransaction _transaction;

        private ISession _session;

        public ISession Session
        {
            get
            {
                if (_session != null) return _session;

                using (var sessionFactory = NHibernateBuilder.GetSessionFactory())
                {
                    _session = sessionFactory.OpenSession();
                }
                return _session;
            }
        }

        public void BeginTransaction()
        {
            if (_transaction == null)
            {
                _transaction = Session.BeginTransaction();
            }
        }

        public void Commit()
        {
            if (_transaction == null) return;
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }
            if (_session != null)
            {
                _session.Dispose();
            }
        }
    }
}