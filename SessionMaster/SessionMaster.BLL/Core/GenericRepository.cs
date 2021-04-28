﻿using SessionMaster.Common.Exceptions;
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

        public GenericRepository(SessionMasterContext context)
        {
            _context = context;
        }

        public virtual TEntity Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            return entity;
        }

        public virtual void Remove(Guid id)
        {
            var entity = GetById(id);

            _context.Set<TEntity>().Remove(entity);
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression)
        {
            return _context.Set<TEntity>().Where(expression);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }

        public virtual TEntity GetById(Guid id)
        {
            var entity = _context.Set<TEntity>().Find(id);

            if (entity == null)
            {
                throw new NotFoundException("Resource not found");
            }

            return entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            //Validate if entity exists
            GetById(entity.Id);

            _context.Set<TEntity>().Update(entity);
            return entity;
        }
    }
}