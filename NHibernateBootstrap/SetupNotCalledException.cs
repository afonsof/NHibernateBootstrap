using System;

namespace NHibernateBootstrap
{
    public class SetupNotCalledException : Exception
    {
        public SetupNotCalledException() : base("You need call the method Setup in the class NHibernateBuilder to use it.")
        {
            
        }
    }
}