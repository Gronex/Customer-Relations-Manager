using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess.Exceptions
{
    public class NotAllowedException : Exception
    {
        public NotAllowedException()
        {
            
        }

        public NotAllowedException(string message) : base(message)
        {
        }
    }
}
