﻿using HealthCare.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HealthCare.Repository.Repository
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IDbContextFactory<CamcoDbContext> _contextFactory;
        private readonly ILogger _logger;

        public Repository(IDbContextFactory<CamcoDbContext> contextFactory, ILoggerFactory loggerFactory)
        {
            _contextFactory = contextFactory;
            _logger = loggerFactory.CreateLogger(typeof(TEntity));
        }

        public virtual int Insert(TEntity entity)
        {
            int id = 0;

            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    context.Set<TEntity>().Add(entity);
                    context.SaveChanges();

                    id = (int)entity.GetType().GetProperty("Id").GetValue(entity, null);
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.InnerException.InnerException.Message);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.ToUpper().Contains("VIOLATION OF UNIQUE KEY"))
                {
                    _logger.LogError("Duplicate unique key");
                }
                else
                {
                    _logger.LogError("OTHER ERROR " + ex.Message);
                }
            }

            return id;
        }

        public virtual async Task<int> InsertAsync(TEntity entity)
        {
            int id = 0;
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var local = context.Set<TEntity>().Local
                        .FirstOrDefault(x => x.GetType().GetProperty("Id").Equals(entity.GetType().GetProperty("Id")));

                    if (local != null)
                        context.Entry(local).State = EntityState.Detached;

                    context.Entry(entity).State = EntityState.Modified;

                    await context.Set<TEntity>().AddAsync(entity);
                    await context.SaveChangesAsync();

                    id = (int)entity.GetType().GetProperty("Id").GetValue(entity, null);
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.ToUpper().Contains("VIOLATION OF UNIQUE KEY"))
                {
                    _logger.LogError("Duplicate unique key");
                }
                else
                {
                    _logger.LogError("OTHER ERROR " + ex.Message);
                }
            }

            return id;
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                await context.Set<TEntity>().AddRangeAsync(entities);
                await context.SaveChangesAsync();
            }
        }
        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return context.Set<TEntity>().Where(predicate);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetListAsync()
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var list = await context.Set<TEntity>().ToListAsync();

                    return list;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual IEnumerable<TEntity> GetList()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return context.Set<TEntity>().ToList();
            }
        }

        public virtual TEntity GetById(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return context.Set<TEntity>().Find(id);
            }
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var EntityLookUp = (await context.Set<TEntity>().FindAsync(id));
                context.Entry(EntityLookUp).State = EntityState.Detached;
                return EntityLookUp;
            }
        }

        public virtual void Remove(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var entityLookup = context.Set<TEntity>().Find(id);
                if (entityLookup != null)
                {
                    context.Set<TEntity>().Remove(entityLookup);
                    context.SaveChanges();
                }
            }
        }

        public virtual void Remove(TEntity entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                int id = (int)entity.GetType().GetProperty("Id").GetValue(entity, null);
                var entityLookup = context.Set<TEntity>().Find(id);
                if (entityLookup != null)
                {
                    context.Set<TEntity>().Remove(entityLookup);
                    context.SaveChanges();
                }
            }
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.Set<TEntity>().RemoveRange(entities);
                context.SaveChanges();
            }
        }

        public virtual Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return context.Set<TEntity>().SingleOrDefaultAsync(predicate);
            }
        }

        public virtual void Update(TEntity entity)
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var local = context.Set<TEntity>().Local
                    .FirstOrDefault(x => x.GetType().GetProperty("Id").Equals(entity.GetType().GetProperty("Id")));

                    if (local != null)
                        context.Entry(local).State = EntityState.Detached;

                    context.Entry(entity).State = EntityState.Modified;

                    context.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.InnerException.InnerException.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var local = context.Set<TEntity>().Local
                    .FirstOrDefault(x => x.GetType().GetProperty("Id").Equals(entity.GetType().GetProperty("Id")));

                    if (local != null)
                        context.Entry(local).State = EntityState.Detached;

                    context.Entry(entity).State = EntityState.Modified;

                    await context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.InnerException.InnerException.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(int skip, int limit)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Set<TEntity>().AsNoTracking().Skip(skip).Take(limit).ToListAsync();
            }
        }

        public async Task<int> CountAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Set<TEntity>().CountAsync();
            }
        }

        public virtual async Task<int> CountAsync(string search)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Set<TEntity>().CountAsync();
            }
        }
        public virtual IEnumerable<TEntity> GetAll()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return context.Set<TEntity>().AsNoTracking().ToList();
            }
        }

        public virtual IEnumerable<TEntity> GetAll(int skip, int limit)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return context.Set<TEntity>().AsNoTracking().Skip(skip).Take(limit).ToList();
            }
        }

        public virtual async Task RemoveAsync(TEntity entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                int id = (int)entity.GetType().GetProperty("Id").GetValue(entity, null);
                var entityLookup = await context.Set<TEntity>().FindAsync(id);
                if (entityLookup != null)
                {
                    context.Set<TEntity>().Remove(entityLookup);
                }
            }
        }

        public virtual async Task<IEnumerable<TEntity>> SearchAsync(Func<TEntity, bool> Conditions)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var Result = context.Set<TEntity>().AsNoTracking().Where(Conditions).ToList();
                return await Task.FromResult(Result);
            }
        }
    }
}