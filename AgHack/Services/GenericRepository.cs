using Microsoft.EntityFrameworkCore;
using AgHack.Models;
using System.Linq.Expressions;

namespace AgHack.Services
{
    /// <summary>
    /// �q�θ�Ʀs���h��@
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
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
        /// �ھ�ID���o����
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// ���o�Ҧ�����
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// �ھڱ���d�����
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// �ھڱ�����o��@����
        /// </summary>
        public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate);
        }

        /// <summary>
        /// �ˬd�O�_�s�b�ŦX���󪺹���
        /// </summary>
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// �p�����ƶq
        /// </summary>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
        }

        /// <summary>
        /// ���o�������
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

            // �M�Υ]�t���ɯ��ݩ�
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // �M�οz�����
            if (filter != null)
                query = query.Where(filter);

            // �b�����e�p���`��
            var totalCount = await query.CountAsync();

            // �M�αƧǩM����
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
        /// �s�W����
        /// </summary>
        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// ��s����
        /// </summary>
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// �R������
        /// </summary>
        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// �x�s�ܧ�
        /// </summary>
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}