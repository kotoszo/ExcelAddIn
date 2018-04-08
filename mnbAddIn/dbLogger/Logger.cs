using System;
using System.IO;
using System.Linq;
using System.Data.OleDb;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

namespace dbLogger
{
    public enum Columns { userName, logTime, reason }
    public class Logger : IDisposable
    {
        private OleDbConnection connection;
        private string dbFullPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "\\db\\Db.accdb";
        private string lastTimeStamp;
        public Logger()
        {
            connection = GetConnection();
        }

        public void Dispose()
        {
            connection.Close();
        }

        private void NormalizePath()
        {
            var text = dbFullPath.Skip(6);
            string newPath = "";
            foreach (char item in text)
            {
                newPath += item;
            }
            dbFullPath = newPath;
        }
        private OleDbConnection GetConnection()
        {
            if (connection == null)
            {
                NormalizePath();
                string connectionString = string.Format(
                    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False;", dbFullPath);
                connection = new OleDbConnection
                {
                    ConnectionString = connectionString
                };
            }
            return connection;
        }
        public void GetLog()
        {
            
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
                        res.Add(Columns.userName, result.GetString(0));
                        res.Add(Columns.logTime, result.GetString(1));
                        string reason = "";
                        if(!result.IsDBNull(2))
                        {
                            reason = result.GetString(2);
                        }
                        res.Add(Columns.reason, reason);
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
            string query = string.Format("UPDATE logDb SET {0} = ? WHERE logTime = ?", columnName);
            OleDbCommand command = GetCommand(query);
            command.Connection.Open();
            if(command.Connection.State == System.Data.ConnectionState.Open)
            {
                command.Parameters.Add("?", OleDbType.VarWChar).Value = logged[columnName];
                command.Parameters.Add("?", OleDbType.VarWChar).Value = logged[Columns.logTime];
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