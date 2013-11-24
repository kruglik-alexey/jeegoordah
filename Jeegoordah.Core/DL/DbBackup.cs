using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeegoordah.Core.Logging;

namespace Jeegoordah.Core.DL
{
    public static class DbBackup
    {
        private static readonly Logger Logger = Logger.For(typeof(DbBackup));

        public static void Backup(DbFactory dbFactory, string appDataDirectory)
        {
            var backupDirectory = Path.Combine(appDataDirectory, "Backup");
            if (!Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);
            var backupFile = "{0}.sqlite".F(DateTime.UtcNow.ToString("yyyy-MM-dd_hh-mm-ss"));
            backupFile = Path.Combine(backupDirectory, backupFile);
            Logger.I("Database backup to {0}", backupFile);

            string backupConnectionString = "data source={0}".F(backupFile);
            using (var backupConnection = new SQLiteConnection(backupConnectionString))
            using (var db = dbFactory.Open())
            {
                backupConnection.Open();
                var sourceConnection = (SQLiteConnection)db.Session.Connection;
                sourceConnection.BackupDatabase(backupConnection, "main", "main", -1, null, -1);
            }

            DeleteOldBackups(backupDirectory, 2);
        }

        private static void DeleteOldBackups(string backupDirectory, int numberToKeep)
        {
            var files = Directory.EnumerateFiles(backupDirectory).OrderBy(File.GetCreationTimeUtc).ToList();
            foreach (var file in files.Take(Math.Max(files.Count - numberToKeep, 0)))
            {
                Logger.I("Deleting old backup file {0}", file);
                File.Delete(file);
            }
        }
    }
}
