using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Vegetable.Core.Database;
using Vegetable.Entities;

namespace Vegetable.Core
{
    public class RepoBase
    {
        internal PostgreDbContext _context { get; }

        public RepoBase(PostgreDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// For read-only/delete scenarios! Do not use result for modify.
        /// Created for checking if Entity exist for current OwnerId
        /// </summary>
        internal async Task<TEntity> GetExistingObjectAsNoTracking<TEntity>(Guid ownerId, TEntity baseEntity) where TEntity : BaseEntity
        {
            var existingObject = await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == baseEntity.Id && x.OwnerId == ownerId);
            if (existingObject == null)
                throw new UnauthorizedAccessException(string.Format("There is no {0} with Id={1} for Owner with OwnerId={2}.", typeof(TEntity).Name, baseEntity.Id, ownerId));
            return existingObject;
        }

        /// <summary>
        /// For read-only/delete scenarios! Do not use result for modify.
        /// Created for checking if Entity exist for current OwnerId
        /// </summary>
        internal async Task<TEntity> GetExistingObjectAsNoTracking<TEntity>(Guid ownerId, Guid baseEntityId) where TEntity : BaseEntity
        {
            var existingObject = await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == baseEntityId && x.OwnerId == ownerId);
            if (existingObject == null)
                throw new UnauthorizedAccessException(string.Format("There is no {0} with Id={1} for Owner with OwnerId={2}.", typeof(TEntity).Name, baseEntityId, ownerId));
            return existingObject;
        }
    }
}