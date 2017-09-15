using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt;
using CodeArt.DTO;

namespace CodeArt.EasyMQ.Event
{
    /// <summary>
    /// Զ���¼�������
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// ����Զ�̸�ʽ���¼�
        /// </summary>
        /// <param name="event"></param>
        void Handle(DTObject @event);
    }
}