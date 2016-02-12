using System;

namespace Infrastructure.DataAccess.Exceptions
{
    public class DuplicateKeyException : Exception
    {
        public DuplicateKeyException() : base()
        {
            
        }

        public DuplicateKeyException(string message) : base(message)
        {
        }
    }
}
