using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SessionMaster.Common.Exceptions;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SessionMaster.BLL.Core
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly SessionMasterContext _context;
        internal DbSet<TEntity> _dbSet;

        public GenericRepository(SessionMasterContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual TEntity Add(TEntity entity)
        {
            _dbSet.Add(entity);
            return entity;
        }

        public virtual void Remove(Guid id)
        {
            var entity = GetById(id);

            _context.Set<TEntity>().Remove(entity);
        }

        public virtual TEntity GetById(Guid id)
        {
            var entity = _dbSet.Find(id);

            if (entity == null)
            {
                throw new NotFoundException("Resource not found");
            }

            return entity;
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                //funktioniert für Include und ThenInclude
                query = include(query);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity Update(TEntity entity)
        {
            //Validate if entity exists
            GetById(entity.Id);

            _dbSet.Update(entity);
            return entity;
        }
    }
}