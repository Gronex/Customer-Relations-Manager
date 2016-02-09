using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using Core.DomainServices;

namespace UnitTests.Stubs
{
    public class UnitOfWorkStub : IUnitOfWork
    {
        public int Save()
        {
            //Do Nothing
            return 1;
        }

        public Task<int> SaveAsync()
        {
            //Do nothing asyncronusly
            return Task.FromResult(1);
        }
    }
}
