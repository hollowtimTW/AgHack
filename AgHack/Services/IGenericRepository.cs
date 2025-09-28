using System.Linq.Expressions;

namespace AgHack.Services
{
    /// <summary>
    /// �q�θ�Ʀs���h����
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// �ھ�ID���o����
        /// </summary>
        /// <param name="id">����ID</param>
        Task<T?> GetByIdAsync(int id);
        
        /// <summary>
        /// ���o�Ҧ�����
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>
        /// �ھڱ���d�����
        /// </summary>
        /// <param name="predicate">�d�߱���</param>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// �ھڱ�����o��@����
        /// </summary>
        /// <param name="predicate">�d�߱���</param>
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// �ˬd�O�_�s�b�ŦX���󪺹���
        /// </summary>
        /// <param name="predicate">�d�߱���</param>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// �p�����ƶq
        /// </summary>
        /// <param name="predicate">�d�߱���]�i��^</param>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        
        /// <summary>
        /// ���o�������
        /// </summary>
        /// <typeparam name="TKey">�Ƨ�������</typeparam>
        /// <param name="filter">�z�����</param>
        /// <param name="orderBy">�ƧǱ���</param>
        /// <param name="ascending">�O�_�ɧǱƦC</param>
        /// <param name="page">���X</param>
        /// <param name="pageSize">�C������</param>
        /// <param name="includes">�]�t���ɯ��ݩ�</param>
        Task<(IEnumerable<T> items, int totalCount)> GetPagedAsync<TKey>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, TKey>>? orderBy = null,
            bool ascending = true,
            int page = 1,
            int pageSize = 10,
            params Expression<Func<T, object>>[] includes);
        
        /// <summary>
        /// �s�W����
        /// </summary>
        /// <param name="entity">�n�s�W������</param>
        void Add(T entity);
        
        /// <summary>
        /// ��s����
        /// </summary>
        /// <param name="entity">�n��s������</param>
        void Update(T entity);
        
        /// <summary>
        /// �R������
        /// </summary>
        /// <param name="entity">�n�R��������</param>
        void Delete(T entity);
        
        /// <summary>
        /// �x�s�ܧ�
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}