using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DataLayer.Surface
{
    /// <summary>This is the factory class for creating surface objects.
    /// </summary>
    public class Factory
    {
        //  web config: http://msdn.microsoft.com/en-us/library/ms178411.aspx
        //  app config: http://msdn.microsoft.com/en-us/library/system.configuration.configurationmanager.aspx
        //using (var conn = new SqlConnection(Properties.Settings.Default.ConnString))   // ConfigurationManager.ConnectionStrings[]
        //{
        //    conn.Open();
        //    try
        //    {
        //        ...
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}

        /// <summary>This struct is the ConTeXt for the database.
        /// It holds both the connection and the transaction.  We need to hold both since if we can't be sure to have a connection.
        /// 
        /// It has to be public since it is what is sent to the calling layer as a handle to the context we are in.
        /// Like so:
        /// The business layer knows and controls what should be in the same transaction.  But since the business layer must not know anything
        /// about the database mechanics (the SqlConnection and SqlTransaction are such mechanics) we encapsulate the connection
        /// and transaction to make them invisible in the business layer.  This means we can use an object of typ CTX  as a handle in the business 
        /// layer to let it control transaction scope.
        /// </summary>
        public struct CTX
        {
            internal SqlConnection Conn;
            internal SqlTransaction Trans;
            internal CTX(SqlConnection conn, SqlTransaction trans)
            {
                this.Conn = conn;
                this.Trans = trans;
            }
        }

        private SqlConnection _conn;
        private SqlTransaction _trans;
        private bool _useTransaction;

        internal SqlTransaction Trans { get { return _trans; } }
        internal SqlConnection Conn { get { return _conn; } }

        public Factory(bool useTransaction = false)
        {
            _useTransaction = useTransaction;
        }

        public void CloseConnection(bool commitTransaction = true)
        {
            if (null != _trans && commitTransaction)
            {
                _trans.Commit();
            }
            _conn.Close();
        }

        /// <summary>This method is used by the calling layer (typically business layer) to create a new surface object.
        /// The calls is typically something like:
        ///     var factory = new Datalayer.Surface.Factory(useTransaction: true);
        ///     var customerSurface = factory.Create &lt; Datalayer.Surface.Customer &gt; ();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Create<T>() where T : SurfaceBase, new()
        {
            var ret = new T();
            ret.Fact = this;
            return ret;
        }

        /// <summary>This method makes sure we have a connection, creating a new one if necessary.
        /// It then makes sure it is open, and opens it if necessary.
        /// It then makes sure we have a transaction, and opens it if necessary.
        /// It finally returns a CTX object so we have a handle to all this.
        /// </summary>
        /// <returns></returns>
        internal CTX MakeConnectionAndOpenAndPossiblyTransaction()
        {
            CreateOrUseConnection();
            
            if (_conn.State == System.Data.ConnectionState.Closed)
            {
                _conn.Open();
            }

            if (_useTransaction)
            {
                if (null == _trans)
                {
                    _trans = _conn.BeginTransaction();
                }
            }

            return new CTX(_conn, _trans);
        }

        /// <summary>This method creates a new connection if necessary.
        /// </summary>
        private void CreateOrUseConnection()
        {
            //_conn = _conn ?? new SqlConnection(Properties.Settings.Default.ConnString);
            //_conn = _conn ?? new SqlConnection( "MooD4L" );
            if (null == _conn)
            {
                string connectionStringKey = System.Configuration.ConfigurationManager.AppSettings["MooD4L"];
                if (null == connectionStringKey)
                {
                    throw new System.Configuration.ConfigurationErrorsException( "There is no appsetting called \"MooD4L\"" );
                }
                var setting = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringKey];
                if (null == setting)
                {
                    throw new System.Configuration.ConfigurationErrorsException( 
                        string.Format( "The appsetting \"{0}\" points to connection string \"{1}\" which does not exist.", 
                            "MooD4L", 
                            connectionStringKey )
                    );
                }
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringKey].ConnectionString;
                if ( string.IsNullOrWhiteSpace( connectionString ) )
                {
                    throw new Exception("no connsettings");
                }
                _conn = new SqlConnection(connectionString);
            }
        }

    }
}
