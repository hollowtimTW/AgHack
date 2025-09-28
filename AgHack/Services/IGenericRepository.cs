using System.Linq.Expressions;

namespace AgHack.Services
{
    /// <summary>
    /// 通用資料存取層介面
    /// </summary>
    /// <typeparam name="T">實體類型</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// 根據ID取得實體
        /// </summary>
        /// <param name="id">實體ID</param>
        Task<T?> GetByIdAsync(int id);
        
        /// <summary>
        /// 取得所有實體
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>
        /// 根據條件查找實體
        /// </summary>
        /// <param name="predicate">查詢條件</param>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// 根據條件取得單一實體
        /// </summary>
        /// <param name="predicate">查詢條件</param>
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// 檢查是否存在符合條件的實體
        /// </summary>
        /// <param name="predicate">查詢條件</param>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// 計算實體數量
        /// </summary>
        /// <param name="predicate">查詢條件（可選）</param>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        
        /// <summary>
        /// 取得分頁資料
        /// </summary>
        /// <typeparam name="TKey">排序鍵類型</typeparam>
        /// <param name="filter">篩選條件</param>
        /// <param name="orderBy">排序條件</param>
        /// <param name="ascending">是否升序排列</param>
        /// <param name="page">頁碼</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <param name="includes">包含的導航屬性</param>
        Task<(IEnumerable<T> items, int totalCount)> GetPagedAsync<TKey>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, TKey>>? orderBy = null,
            bool ascending = true,
            int page = 1,
            int pageSize = 10,
            params Expression<Func<T, object>>[] includes);
        
        /// <summary>
        /// 新增實體
        /// </summary>
        /// <param name="entity">要新增的實體</param>
        void Add(T entity);
        
        /// <summary>
        /// 更新實體
        /// </summary>
        /// <param name="entity">要更新的實體</param>
        void Update(T entity);
        
        /// <summary>
        /// 刪除實體
        /// </summary>
        /// <param name="entity">要刪除的實體</param>
        void Delete(T entity);
        
        /// <summary>
        /// 儲存變更
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}