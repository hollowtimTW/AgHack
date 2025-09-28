using Microsoft.EntityFrameworkCore;
using AgHack.Models;
using System.Linq.Expressions;

namespace AgHack.Services
{
    /// <summary>
    /// 通用資料存取層實作
    /// </summary>
    /// <typeparam name="T">實體類型</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AgHackContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AgHackContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// 根據ID取得實體
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// 取得所有實體
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// 根據條件查找實體
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// 根據條件取得單一實體
        /// </summary>
        public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate);
        }

        /// <summary>
        /// 檢查是否存在符合條件的實體
        /// </summary>
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// 計算實體數量
        /// </summary>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
        }

        /// <summary>
        /// 取得分頁資料
        /// </summary>
        public virtual async Task<(IEnumerable<T> items, int totalCount)> GetPagedAsync<TKey>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, TKey>>? orderBy = null,
            bool ascending = true,
            int page = 1,
            int pageSize = 10,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // 套用包含的導航屬性
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // 套用篩選條件
            if (filter != null)
                query = query.Where(filter);

            // 在分頁前計算總數
            var totalCount = await query.CountAsync();

            // 套用排序和分頁
            if (orderBy != null)
            {
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        /// <summary>
        /// 新增實體
        /// </summary>
        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// 更新實體
        /// </summary>
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// 刪除實體
        /// </summary>
        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// 儲存變更
        /// </summary>
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}