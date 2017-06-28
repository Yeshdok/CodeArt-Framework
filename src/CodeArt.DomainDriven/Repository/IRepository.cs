using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DomainDriven
{
    public interface IRepository
    {
        ///// <summary>
        ///// ��������ӵ��ִ�
        ///// </summary>
        ///// <param name="obj"></param>
        //void Add(IAggregateRoot obj);

        ///// <summary>
        ///// �޸Ķ����ڲִ��е���Ϣ
        ///// </summary>
        ///// <param name="obj"></param>
        //void Update(IAggregateRoot obj);

        ///// <summary>
        ///// �Ӳִ���ɾ������
        ///// </summary>
        ///// <param name="obj"></param>
        //void Delete(IAggregateRoot obj);

        ///// <summary>
        ///// ��������
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <param name="level">��������</param>
        //void Lock(IAggregateRoot obj, QueryLevel level);

        /// <summary>
        /// ���ݱ�Ų��Ҷ���
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        IAggregateRoot Find(object id, QueryLevel level);


        //#region �ھ۱߽��ڵĸ߼�ʵ�����

        //void AddEntityPro(IEntityObjectPro obj);

        //void UpdateEntityPro(IEntityObjectPro obj);

        //void DeleteEntityPro(IEntityObjectPro obj);

        ///// <summary>
        ///// �����ھ۸��⣬������󶼲��ܴ�����ѯ���߼����ö���Ҳ����ˣ����Բ�ѯ�߼����ö����ǲ��ܴ��������
        ///// ���ȴ�����ѯ�ھ۸�������ִ�и߼���Ա�Ĳ�ѯ����
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="rootId"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //IEntityObjectPro FindEntityPro(object rootId, object id);

        //#endregion
    }



    public interface IRepository<TRoot> : IRepository
        where TRoot : class, IAggregateRoot, IRepositoryable
    {
        /// <summary>
        /// ��������ӵ��־ò���
        /// </summary>
        /// <param name="obj"></param>
        void Add(TRoot obj);

        /// <summary>
        /// �޸Ķ����ڳ־ò��е���Ϣ
        /// </summary>
        /// <param name="obj"></param>
        void Update(TRoot obj);

        /// <summary>
        /// �ӳ־ò���ɾ������
        /// </summary>
        /// <param name="obj"></param>
        void Delete(TRoot obj);

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="level">��������</param>
        void Lock(TRoot obj, QueryLevel level);

        /// <summary>
        /// ���ݱ�Ų��Ҷ���
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        new TRoot Find(object id, QueryLevel level);


        #region �ھ۱߽��ڵĸ߼�ʵ�����

        void AddEntityPro<T>(T obj) where T : class, IEntityObjectPro<TRoot>;

        void UpdateEntityPro<T>(T obj) where T : class, IEntityObjectPro<TRoot>;

        void DeleteEntityPro<T>(T obj) where T : class, IEntityObjectPro<TRoot>;

        /// <summary>
        /// �����ھ۸��⣬������󶼲��ܴ�����ѯ���߼����ö���Ҳ����ˣ����Բ�ѯ�߼����ö����ǲ��ܴ��������
        /// ���ȴ�����ѯ�ھ۸�������ִ�и߼���Ա�Ĳ�ѯ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        T FindEntityPro<T>(object rootId, object id) where T : class, IEntityObjectPro<TRoot>;

        #endregion
    }
}
