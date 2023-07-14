using MongoDB.Driver;
using System.Linq.Expressions;

namespace SlotMachine.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> AsQueryable();

        IEnumerable<TEntity> FilterBy(
            Expression<Func<TEntity, bool>> filterExpression);

        IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TEntity, bool>> filterExpression,
            Expression<Func<TEntity, TProjected>> projectionExpression);

        TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression);

        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression);

        TEntity FindById(string id);

        Task<TEntity> FindByIdAsync(string id);

        void InsertOne(TEntity document);

        Task InsertOneAsync(TEntity document);

        void ReplaceOne(TEntity document);

        Task ReplaceOneAsync(TEntity document);

        void DeleteOne(Expression<Func<TEntity, bool>> filterExpression);

        Task DeleteOneAsync(Expression<Func<TEntity, bool>> filterExpression);

        void DeleteById(string id);

        Task DeleteByIdAsync(string id);

        void InsertOne(TEntity document, IClientSessionHandle session);
        Task InsertOneAsync(TEntity document, IClientSessionHandle session);
        void ReplaceOne(TEntity document, IClientSessionHandle session);
        Task ReplaceOneAsync(TEntity document, IClientSessionHandle session);
        void DeleteOne(Expression<Func<TEntity, bool>> filterExpression, IClientSessionHandle session);
        Task DeleteOneAsync(Expression<Func<TEntity, bool>> filterExpression, IClientSessionHandle session);
        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression, IClientSessionHandle session);

    }
}
