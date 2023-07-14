using Core;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace SlotMachine.Infrastructure
{
    public class MongoRepository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
    {
        private readonly IMongoCollection<TEntity> _collection;

        public MongoRepository(IMongoDatabase database)
        {
            //var mongoSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
            //var database = new MongoClient(mongoSettings).GetDatabase(settings.DatabaseName);
            
            _collection = database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public virtual IQueryable<TEntity> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public virtual IEnumerable<TEntity> FilterBy(
            Expression<Func<TEntity, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).ToEnumerable();
        }

        public virtual IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TEntity, bool>> filterExpression,
            Expression<Func<TEntity, TProjected>> projectionExpression)
        {
            return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
        }

        public virtual TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).FirstOrDefault();
        }

        public virtual Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return Task.Run(() => _collection.Find(filterExpression).FirstOrDefaultAsync());
        }

        public virtual Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression, IClientSessionHandle session)
        {
            return Task.Run(() => _collection.Find(session, filterExpression).FirstOrDefaultAsync());
        }

        public virtual TEntity FindById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);
            return _collection.Find(filter).SingleOrDefault();
        }

        public virtual Task<TEntity> FindByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);
                return _collection.Find(filter).SingleOrDefaultAsync();
            });
        }


        public virtual void InsertOne(TEntity document)
        {
            _collection.InsertOne(document);
        }

        public virtual void InsertOne(TEntity document, IClientSessionHandle session)
        {
            _collection.InsertOne(session, document);
        }

        public virtual Task InsertOneAsync(TEntity document)
        {
            return Task.Run(() => _collection.InsertOneAsync(document));
        }

        public virtual Task InsertOneAsync(TEntity document, IClientSessionHandle session)
        {
            return Task.Run(() => _collection.InsertOneAsync(session, document));
        }

        public void ReplaceOne(TEntity document)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
            _collection.FindOneAndReplace(filter, document);
        }

        public void ReplaceOne(TEntity document, IClientSessionHandle session)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
            _collection.FindOneAndReplace(session, filter, document);
        }

        public virtual async Task ReplaceOneAsync(TEntity document)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.FindOneAndReplaceAsync(filter, document);
        }

        public virtual async Task ReplaceOneAsync(TEntity document, IClientSessionHandle session)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.FindOneAndReplaceAsync(session, filter, document);
        }

        public void DeleteOne(Expression<Func<TEntity, bool>> filterExpression)
        {
            _collection.FindOneAndDelete(filterExpression);
        }

        public void DeleteOne(Expression<Func<TEntity, bool>> filterExpression, IClientSessionHandle session)
        {
            _collection.FindOneAndDelete(session, filterExpression);
        }

        public Task DeleteOneAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return Task.Run(() => _collection.FindOneAndDeleteAsync(filterExpression));
        }
        public Task DeleteOneAsync(Expression<Func<TEntity, bool>> filterExpression, IClientSessionHandle session)
        {
            return Task.Run(() => _collection.FindOneAndDeleteAsync(session, filterExpression));
        }
        public void DeleteById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);
            _collection.FindOneAndDelete(filter);
        }

        public Task DeleteByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);
                _collection.FindOneAndDeleteAsync(filter);
            });
        }

    }

}
