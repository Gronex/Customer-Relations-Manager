using System;

namespace Infrastructure.DataAccess.Exceptions
{
    public class DuplicateException : Exception
    {
        public DuplicateException() : base()
        {
            
        }

        public DuplicateException(string message) : base(message)
        {
        }
    }
}
