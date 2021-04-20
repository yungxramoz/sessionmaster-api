using SessionMaster.DAL;
using SessionMaster.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SessionMaster.BLL.Core
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> 
        where TEntity : BaseEntity
    {
        protected readonly SessionMasterContext _context;

        public GenericRepository(SessionMasterContext context)
        {
            _context = context;
        }
        
        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void Remove(Guid id)
        {
            var entity = GetById(id);
            if (entity == null)
            {
                //TODO throw proper exception
                throw new Exception("Entity not found");
            }

            _context.Set<TEntity>().Remove(entity);
        }

        public TEntity Find(Expression<Func<TEntity, bool>> expression)
        {
            return _context.Set<TEntity>().Find(expression);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }

        public TEntity GetById(Guid id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }
    }
}
