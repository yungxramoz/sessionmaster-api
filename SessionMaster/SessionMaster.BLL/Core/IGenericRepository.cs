using SessionMaster.DAL.Entity;
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

        TEntity Add(TEntity entity);

        TEntity Update(TEntity entity);

        void Delete(TEntity entity);
        void Delete(Guid id);
    }
}
