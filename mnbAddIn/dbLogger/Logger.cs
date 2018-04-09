using System;
using System.Data;
using dbLogger.db;
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
            if (!Handler.IsDbAlive())
            {
                Handler.DbMaker(GetConnection());
                Handler.DbColumnMaker(GetCommand(""));  // TODO fix it.
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
                    @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+Handler.path+";Persist Security Info=False;";
                connection = new OleDbConnection
                {
                    ConnectionString = connectionString
                };
            }
            return connection;
        }
        public DataTable AllSelect()
        {
            var dataTable = new DataTable();
            string query = "SELECT * FROM logDb";
            OleDbCommand command = GetCommand(query);
            command.Connection.Open();
            if (command.Connection.State == ConnectionState.Open)
            {
                return ExecuteSelect(command);
            }
            return null;
        }
        public DataTable SingleSelect(Columns columnName)
        {
            string query = string.Format("SELECT * FROM logDb WHERE {0} = ?", columnName);
            OleDbCommand command = GetCommand(query);
            command.Connection.Open();
            if(command.Connection.State == ConnectionState.Open)
            {
                command.Parameters.Add("?", OleDbType.VarChar).Value = lastTimeStamp;
                return ExecuteSelect(command);
            }
            return null;
        }
        private DataTable ExecuteSelect(OleDbCommand command)
        {
            DataTable dataTable = new DataTable();
            try
            {
                OleDbDataReader reader = command.ExecuteReader();
                dataTable.Load(reader);
            }
            catch (OleDbException e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                command.Connection.Close();
            }
            return dataTable;
        }
        public void SaveNewLog()
        {
            string userName = Environment.UserName;
            string timeStamp = GetTimestamp(DateTime.Now);
            lastTimeStamp = timeStamp;
            string query = "INSERT INTO logDb (userName, logTime) VALUES(?, ?)";
            OleDbCommand command = GetCommand(query);
            command.Connection.Open();
            if(connection.State == ConnectionState.Open)
            {
                command.Parameters.Add("?", OleDbType.VarWChar).Value = userName;
                command.Parameters.Add("?", OleDbType.VarWChar).Value = timeStamp;
                ExecuteCommand(command);
            }
        }

        public void Update(Columns columnName, Dictionary<Columns, string> logged)
        {
            string query = string.Format("UPDATE logDb SET {0} = ? WHERE logID = ?", columnName);
            OleDbCommand command = GetCommand(query);
            command.Connection.Open();
            if(command.Connection.State == ConnectionState.Open)
            {
                command.Parameters.Add("?", OleDbType.VarWChar).Value = logged[columnName];
                command.Parameters.Add("?", OleDbType.VarWChar).Value = logged[Columns.logId];
                ExecuteCommand(command);
            }
        }

        public void Update(DataTable dTable, Columns columnName, HashSet<string> idsToUpdate)
        {
            List<DataRow> tempList = new List<DataRow>();
            foreach (DataRow item in dTable.Rows)
            {
                if (idsToUpdate.Contains(item["logId"].ToString()))
                {
                    tempList.Add(item);
                }
            }
            foreach (var row in tempList)
            {
                string query = string.Format("UPDATE logDb SET {0} = ? WHERE logID = ?", columnName);
                OleDbCommand command = GetCommand(query);
                command.Connection.Open();
                if (command.Connection.State == ConnectionState.Open)
                {
                    command.Parameters.Add("?", OleDbType.VarWChar).Value = row["reason"];
                    command.Parameters.Add("?", OleDbType.VarWChar).Value = row["logId"];
                    ExecuteCommand(command);
                }
            }
        }

        private void ExecuteCommand(OleDbCommand command)
        {
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