﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using CodeArt.DomainDriven;
using CodeArt.Util;


namespace CodeArt.DomainDriven.DataAccess
{
    public partial class DataTable
    {
        private void Build()
        {
            IfUnBuilt(this.Name, () =>
            {
                var sql = CreateTable.Create(this).Build(null);
                SqlHelper.Execute(this.ConnectionName, sql);
            });
            AddRuntimeIndex(this);
        }

        private static Dictionary<string, bool> _built = new Dictionary<string, bool>();

        private static void IfUnBuilt(string name, Action action)
        {
            if (_built.ContainsKey(name)) return;
            lock (_built)
            {
                if (_built.ContainsKey(name)) return;
                _built.Add(name, true);
                action();
            }
        }

        /// <summary>
        /// 创建运行时记录的表信息
        /// </summary>
        internal static void RuntimeBuild()
        {
            _built.Clear();
            foreach (var index in _runtimeIndexs)
            {
                IfUnBuilt(index.Name, () =>
                {
                    var sql = CreateTable.Create(index).Build(null);
                    SqlHelper.Execute(index.ConnectionName, sql);
                });
            }
        }


        /// <summary>
        /// 删除表
        /// </summary>
        internal static void Drop()
        {
            foreach (var tableName in _indexs)
            {
                var sql = DropTable.Create(tableName).Build(null);
                SqlHelper.Execute(tableName, sql);
            }
            _built.Clear();
        }

        /// <summary>
        /// 找出当前应用程序可能涉及到的表名称
        /// </summary>
        private static IEnumerable<string> _indexs;


        static DataTable()
        {
            _indexs = GetIndexs();
        }

        private static IEnumerable<string> GetIndexs()
        {
            List<string> tables = new List<string>();
            foreach (var objectType in DomainObject.TypeIndex)
            {
                if (DomainObject.IsEmpty(objectType)) continue;
                var fileds = DataModel.GetObjectFields(objectType, false);
                AddIndexName(tables,objectType.Name);
                var relatedNames = GetRelatedNames(objectType, fileds);
                foreach (var relatedName in relatedNames)
                {
                    AddIndexName(tables, relatedName);
                }
            }
            return tables.Distinct();
        }

        private static void AddIndexName(List<string> tables,string tableName)
        {
            tables.Add(tableName);
            tables.Add(string.Format("{0}_{1}", Snapshot, tableName));
        }

        const string Snapshot = "Snapshot";


        #region 运行时表索引

        private static List<DataTable> _runtimeIndexs = new List<DataTable>();

        /// <summary>
        /// 全部表的索引
        /// </summary>
        public static IEnumerable<DataTable> RuntimeIndexs
        {
            get
            {
                return _runtimeIndexs;
            }
        }


        private static void AddRuntimeIndex(DataTable table)
        {
            lock (_runtimeIndexs)
            {
                _runtimeIndexs.Add(table);
            }
        }

        //private static void ClearIndex(DataTable table)
        //{
        //    lock (_runtimeIndexs)
        //    {
        //        _runtimeIndexs.Add(table);
        //    }
        //}

        #endregion

        #region 构建时索引，主要用于防止循环引用导致的死循环

        private static DataTable GetBuildtimeIndex(IDataField memberField, string rootTableName, string tableName)
        {
            string key = GetUniqueKey(memberField, rootTableName, tableName);
            if (_buildtimeIndex.TryGetValue(key, out var table))
                return table;
            return null;
        }

        private static void AddBuildtimeIndex(DataTable table)
        {
            _buildtimeIndex.TryAdd(table.UniqueKey, table);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberField">表所属的成员字段</param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static string GetUniqueKey(IDataField memberField, string rootTableName, string tableName)
        {
            if (memberField == null) return tableName; //这肯定是根
            var chain = new ObjectChain(memberField);
            //由于不同类型的表名不同，所以没有加入对象名称作为计算结果
            //由于isSnapshot的值不同表名也不同，所以没有加入对象名称作为计算结果
            return string.Format("{0}_{1}+{2}", rootTableName, chain.PathCode, tableName);
        }

        private static ConcurrentDictionary<string, DataTable> _buildtimeIndex = new ConcurrentDictionary<string, DataTable>(StringComparer.OrdinalIgnoreCase);

        #endregion
    }
}
