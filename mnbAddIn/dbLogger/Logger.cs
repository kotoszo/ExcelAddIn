using System;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Collections.Generic;

namespace dbLogger
{
    public enum Columns { logId, userName, logTime, reason }
    public class Logger : IDisposable
    {
        private OleDbConnection connection;
        private string lastTimeStamp;
        public Logger()
        {
            if (!db.Handler.IsDbAlive())
            {
                db.Handler.DbMaker(GetConnection());
                db.Handler.DbColumnMaker(GetCommand(""));
            }
            connection = GetConnection();
        }

        public void Dispose()
        {
            connection.Close();
        }
        private OleDbConnection GetConnection()
        {
            if (connection == null)
            {
                string connectionString = 
                    @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\\Db.accdb;Persist Security Info=False;";
                connection = new OleDbConnection
                {
                    ConnectionString = connectionString
                };
            }
            return connection;
        }
        public Dictionary<Columns, string> Select(Columns columnName)
        {
            string query = string.Format("SELECT * FROM logDb WHERE {0} = ?", columnName);
            OleDbCommand command = GetCommand(query);
            command.Connection.Open();
            if(command.Connection.State == System.Data.ConnectionState.Open)
            {
                command.Parameters.Add("?", OleDbType.VarChar).Value = lastTimeStamp;
                Dictionary<Columns, string> res = new Dictionary<Columns, string>();
                try
                {
                    OleDbDataReader result = command.ExecuteReader();
                    while (result.Read())
                    {
                        if (!result.IsDBNull(0))
                        {
                            res.Add(Columns.logId, result.GetInt32(0).ToString());
                            res.Add(Columns.userName, result.GetString(1));
                            res.Add(Columns.logTime, result.GetString(2));
                            string reason = "";
                            if(!result.IsDBNull(3))
                            {
                                reason = result.GetString(3);
                            }
                            res.Add(Columns.reason, reason);
                        }
                    }
                }
                catch (OleDbException e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    command.Connection.Close();
                }
                return res;
            }
            return null;
        }
        public void SaveNewLog()
        {
            string userName = Environment.UserName;
            string timeStamp = GetTimestamp(DateTime.Now);
            lastTimeStamp = timeStamp;
            string query = "INSERT INTO logDb (userName, logTime) VALUES(?, ?)";
            OleDbCommand command = GetCommand(query);
            command.Connection.Open();
            if(connection.State == System.Data.ConnectionState.Open)
            {
                command.Parameters.Add("?", OleDbType.VarWChar).Value = userName;
                command.Parameters.Add("?", OleDbType.VarWChar).Value = timeStamp;
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (OleDbException e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void Update(Columns columnName, Dictionary<Columns, string> logged)
        {
            string query = string.Format("UPDATE logDb SET {0} = ? WHERE logID = ?", columnName);
            OleDbCommand command = GetCommand(query);
            command.Connection.Open();
            if(command.Connection.State == System.Data.ConnectionState.Open)
            {
                command.Parameters.Add("?", OleDbType.VarWChar).Value = logged[columnName];
                command.Parameters.Add("?", OleDbType.VarWChar).Value = logged[Columns.logId];
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (OleDbException e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }
        private OleDbCommand GetCommand(string query)
        {
            return new OleDbCommand()
            {
                Connection = connection,
                CommandText = query
            };
        }
        private string GetTimestamp(DateTime dt)
        {
            return dt.ToString("yyyyMMddHHmm");
        }
    }
}