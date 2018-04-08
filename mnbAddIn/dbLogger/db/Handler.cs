using ADOX;
using System.IO;
using System.Linq;
using System.Data.OleDb;
using System.Reflection;
using System;

namespace dbLogger.db
{
    internal static class Handler
    {
        public static string path;
        private static OleDbConnection connection;
        private static OleDbCommand command;

        public static void DbMaker(OleDbConnection connection)
        {
            Handler.connection = connection;
            Catalog cat = new Catalog();
            string createStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path;
            cat.Create(createStr);
            cat = null;
        }
        internal static void DbColumnMaker(OleDbCommand oleDbCommand)
        {
            command = oleDbCommand;
            command.Connection = connection;
            command.CommandText = "CREATE TABLE logDb (" +
                "[logId] AUTOINCREMENT NOT NULL PRIMARY KEY ," +
                "[userName] VARCHAR(75) NOT NULL ," +
                "[logTime] VARCHAR(40) NOT NULL ," +
                "[reason] VARCHAR(255))";
            command.Connection.Open();
            if(command.Connection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (OleDbException e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message);
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }
        /// <summary>
        /// The dbFullPath contains this: 'file:\\C:...' etc for some reason, so i had to remove it.
        /// </summary>
        /// <returns></returns>
        private static string NormalizePath()
        {
            string dbFullPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "\\Db.accdb";
            var text = dbFullPath.Skip(6);
            string newPath = "";
            foreach (char item in text)
            {
                newPath += item;
            }
            return newPath;
        }
        internal static bool IsDbAlive()
        {
            path = NormalizePath();
            return File.Exists(path);
        }
    }
}
