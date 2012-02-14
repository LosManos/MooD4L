using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DataLayer.Surface
{
    /// <summary>This is the base class for all surface objects.
    /// </summary>
    public class SurfaceBase
    {
        protected Factory _factory;

        /// <summary>The factory property is only set by the ... factory.
        /// </summary>
        internal Factory Fact { set { _factory = value; } }

        protected SurfaceBase() { }

        /// <summary>This method is used for creating a new connection.
        /// Use if for the cases where a transaction isn't needed, like simple selects.
        /// It returns a connection so it is easy to use it with a using statement.
        /// 
        /// CreateOpenConnection is even easier to use.
        /// </summary>
        /// <returns></returns>
        /// <see cref="CreateOpenConnection"/>
        protected internal SqlConnection CreateConnection()
        {
            return new SqlConnection(Properties.Settings.Default.ConnString);
        }

        /// <summary>This method is used for creating a new open conneciton.
        /// Use if for the cases where a transaction isn't needed, like simple selects.
        /// It returns a connection so it is easy to use it with a using statement.
        /// </summary>
        /// <returns></returns>
        protected internal SqlConnection CreateOpenConnection()
        {
            var ret = CreateConnection();
            ret.Open();
            return ret;
        }

        /// <summary>This method is used for making sure we have a transaction.  It creates whatever is needed but tries not to.
        /// If a transaction isn't needed, CreateConnection and CreateOpenConnection are easier to use.
        /// </summary>
        /// <returns></returns>
        protected internal Factory.CTX MakeConnectionAndTransaction()
        {
            return _factory.MakeConnectionAndOpenAndPossiblyTransaction();
        }
    }
}
