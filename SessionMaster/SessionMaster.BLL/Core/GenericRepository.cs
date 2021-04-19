using SessionMaster.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SessionMaster.BLL.Core
{
    public class GenericRepository : IGenericRepository<BaseEntity>
    {
        public BaseEntity Add(BaseEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(BaseEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public BaseEntity Find(Expression<Func<BaseEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BaseEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public BaseEntity GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public BaseEntity Update(BaseEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
