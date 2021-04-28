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

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression);

        TEntity Add(TEntity entity);

        TEntity Update(TEntity entity);

        void Remove(Guid id);
    }
}