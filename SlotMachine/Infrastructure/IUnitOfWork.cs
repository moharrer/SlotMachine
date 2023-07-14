using MongoDB.Driver;

namespace SlotMachine.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IClientSessionHandle StartTransaction();
        IClientSessionHandle GetSession();
        void Commit();
        void Rollback();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMongoClient _client;
        private IClientSessionHandle _session;

        public UnitOfWork(IMongoClient client)
        {
            _client = client;
        }

        public IClientSessionHandle StartTransaction()
        {
            _session = _client.StartSession();
            _session.StartTransaction();

            return _session;
        }

        public IClientSessionHandle GetSession()
        {
            return _session;
        }

        public void Commit()
        {
            _session.CommitTransaction();
        }

        public void Rollback()
        {
            _session.AbortTransaction();
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }


}
