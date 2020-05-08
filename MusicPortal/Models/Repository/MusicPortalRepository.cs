using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MusicPortal.Models.Repository
{
    public class MusicPortalRepository : IRepository, IDisposable
    {
        MusicPortalContext context;
        public MusicPortalRepository()
        {
            context = new MusicPortalContext();
        }
        public async Task<bool> CreateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                await Task.Run(() =>
                {
                    context.Set<TEntity>().Add(entity);
                    context.SaveChanges();
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                await Task.Run(() =>
                {
                    if (entity is Artist)
                        (entity as Artist).Songs.Clear();
                    if (entity is Genre)
                        (entity as Genre).Songs.Clear();
                    context.Entry<TEntity>(entity).State = EntityState.Deleted;
                    context.SaveChanges();
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<TEntity> GetAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            TEntity entity = null;
            try
            {
                await Task.Run(() =>
                {
                    entity = context.Set<TEntity>().Where(predicate).FirstOrDefault();
                });
                return entity;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IQueryable<TEntity>> GetListAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            IQueryable<TEntity> result = null;
            try
            {
                await Task.Run(() =>
                {
                    result = context.Set<TEntity>().Where(predicate);
                });
                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IQueryable<TEntity>> GetListAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy) where TEntity : class
        {
            IQueryable<TEntity> result = null;
            try
            {
                await Task.Run(() =>
                {
                    result = context.Set<TEntity>().Where(predicate).OrderBy(orderBy);
                });
                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IQueryable<TEntity>> GetListAsync<TEntity, TKey>(Expression<Func<TEntity, TKey>> orderBy) where TEntity : class
        {
            IQueryable<TEntity> result = null;
            try
            {
                await Task.Run(() =>
                {
                    result = context.Set<TEntity>().OrderBy(orderBy);
                });
                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IQueryable<TEntity>> GetListAsync<TEntity>() where TEntity : class
        {
            IQueryable<TEntity> result = null;
            try
            {
                await Task.Run(() =>
                {
                    result = context.Set<TEntity>();
                });
                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                await Task.Run(() =>
                {
                    context.Entry<TEntity>(entity).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsUniqueUserValue(string value)
        {
            return !(context.Users.Where(x => x.Email == value).Any() || context.Users.Where(x => x.Login == value).Any());
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}