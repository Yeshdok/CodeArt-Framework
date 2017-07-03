﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public static class SqlContext
    {
        public static ISqlConnectionProvider GetConnectionProvider()
        {
            if (_connectionProvider == null) _connectionProvider = SqlConnectionProvider.Instance;
            return _connectionProvider;
        }

        private static ISqlConnectionProvider _connectionProvider;

        public static void RegisterConnectionProvider(ISqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }


        private static IDatabaseAgent _agent;

        public static IDatabaseAgent GetAgent()
        {
            if (_agent == null) throw new InvalidOperationException(Strings.NoDatabaseAgent);
            return _agent;
        }

        /// <summary>
        /// 注册项目中会使用到的数据库的代理，不能重复注册
        /// </summary>
        /// <param name="agent"></param>
        public static void RegisterAgent(IDatabaseAgent agent)
        {
            if (_agent != null) return;
            _agent = agent;
        }


        /// <summary>
        /// 获得连接使用的数据库类型的名称
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        public static string GetDbType()
        {
            return GetAgent().Database;
        }


        static SqlContext()
        {
            _agent = DataAccessConfiguration.Current.GetDatabaseAgent() ?? new SQLServerAgent();
        }


    }
}