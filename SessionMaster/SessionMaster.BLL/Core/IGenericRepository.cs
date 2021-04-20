using SessionMaster.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SessionMaster.BLL.Core
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        TEntity GetById(Guid id);
        IEnumerable<TEntity> GetAll();
        TEntity Find(Expression<Func<TEntity, bool>> expression);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Remove(TEntity entity);
        void Remove(Guid id);
    }
}
