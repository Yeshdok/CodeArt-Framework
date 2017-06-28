using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt;
using CodeArt.Runtime;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// �߽��¼�������
    /// </summary>
    public enum BoundedEvent : byte
    {
        /// <summary>
        /// �����������֮��
        /// </summary>
        Constructed = 1,

        /// <summary>
        /// ������󱻸ı��
        /// </summary>
        Changed = 2,

        /// <summary>
        /// �ύ�����������ִ�֮ǰ
        /// </summary>
        PreAdd = 3,
        /// <summary>
        /// �ύ�����������ִ�֮��
        /// </summary>
        Added = 4,

        /// <summary>
        /// �ύ�޸Ĳ������ִ�֮ǰ
        /// </summary>
        PreUpdate = 5,
        /// <summary>
        /// �ύ�޸Ĳ������ִ�֮��
        /// </summary>
        Updated = 6,

        /// <summary>
        /// �ύɾ���������ִ�֮ǰ
        /// </summary>
        PreDelete = 7,
        /// <summary>
        /// �ύɾ���������ִ�֮��
        /// </summary>
        Deleted = 8,
        /// <summary>
        /// ͨ�õı߽��¼�
        /// </summary>
        Any = 9
    }
}