﻿using Dapper;
using DapperExtensions;
using SQLinq.Dapper;
using sweet.framework.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

/* =======================================================================
* 创建时间：2015/12/21 11:49:26
* 作者：sweet
* Framework: 4.0
* ========================================================================
*/

namespace insurance.assistant.repository
{
    public class DapperRepository<T> //: IRepository<T>
        where T : class, IEntity, new()
    {
        private readonly string _databaseName;

        public DapperRepository(string fileName)
        {
            this._databaseName = fileName;
        }

        protected DbConnection OpenConnection()
        {
            string connectionString = "Data Source=" + _databaseName;
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            conn.Open();
            return conn;
        }

        public T Insert(T entity)
        {
            using (IDbConnection conn = OpenConnection())
            {
                int id = conn.Insert(entity);
                return entity;
            }
        }

        public int InsertTransaction(IEnumerable<T> entityList)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> LoadEntities(Expression<Func<T, object>> whereLambda)
        {
            using (IDbConnection conn = OpenConnection())
            {
                var selector = new SQLinq.SQLinq<T>().Select(whereLambda);
                var list = conn.Query(selector);
                return list.AsQueryable();
            }
        }

        public IQueryable<T> LoadPageEntities<S>(int pageIndex, int pageSize, out int totalCount,
                                                 Expression<Func<T, object>> whereLambda,
                                                 Expression<Func<T, object>> orderLambda, bool isAsc = true)
        {
            using (IDbConnection conn = OpenConnection())
            {
                var selector = new SQLinq.SQLinq<T>()
                                         .Select(whereLambda);
                //查询总数
                string sql = selector.ToSQL().ToQuery();
                totalCount = conn.ExecuteScalar<int>(sql);

                selector = isAsc ? selector.OrderBy(orderLambda) : selector.OrderByDescending(orderLambda);
                selector = selector.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

                var list = conn.Query(selector);

                return list.AsQueryable();
            }
        }

        public bool Remove(T entity)
        {
            throw new NotImplementedException();

            using (IDbConnection conn = OpenConnection())
            {
                return conn.Delete(entity);
            }
        }

        public int RemoveTransaction(IEnumerable<T> entityList)
        {
            throw new NotImplementedException();
        }

        public bool SaveOrUpdate(T entity)
        {
            throw new NotImplementedException();
        }

        public int SaveOrUpdateTransaction(IEnumerable<T> entityList)
        {
            throw new NotImplementedException();
        }

        public bool Update(T entity)
        {
            using (IDbConnection conn = OpenConnection())
            {
                return conn.Update(entity);
            }
        }

        public int UpdateTransaction(IEnumerable<T> entityList)
        {
            throw new NotImplementedException();
        }
    }
}