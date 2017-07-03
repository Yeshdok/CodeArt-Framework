using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace CodeArt.DomainDriven
{
    internal sealed class TransactionScopeManager : ITransactionManager
    {
        private TransactionScope _scope = null;

        public TransactionScopeManager()
        {
        }

        /// <summary>
        /// ��ʼ����
        /// </summary>
        public void Begin()
        {
            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadUncommitted;
            _scope = new TransactionScope(TransactionScopeOption.Required, option);
        }

        /// <summary>
        /// �ع�����
        /// </summary>
        public void RollBack()
        {
            _scope.Dispose();
        }

        /// <summary>
        /// �ύ����
        /// </summary>
        public void Commit()
        {
            _scope.Complete();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
