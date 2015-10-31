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

        public static void Backup(string sourceConnectionString, string appDataDirectory)
        {
            var now = DateTime.UtcNow;
            var backupDirectory = Path.Combine(appDataDirectory, "Backup");
            if (!Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);

            if (HasBackupForToday(now, backupDirectory)) return;
                        
            var backupFile = "{0}.sqlite".F(now.ToString("yyyy-MM-dd_hh-mm-ss"));
            backupFile = Path.Combine(backupDirectory, backupFile);
            Logger.I("Database backup to {0}", backupFile);

            string backupConnectionString = "data source={0}".F(backupFile);
            using (var backupConnection = new SQLiteConnection(backupConnectionString))
            using (var sourceConnection = new SQLiteConnection(sourceConnectionString))
            {
                backupConnection.Open();
                sourceConnection.Open();
                sourceConnection.BackupDatabase(backupConnection, "main", "main", -1, null, 0);
            }

            DeleteOldBackups(backupDirectory, 5);
        }

        private static bool HasBackupForToday(DateTime now, string backupDirectory)
        {
            var today = now.ToString("yyyy-MM-dd");
            return Directory.EnumerateFiles(backupDirectory).Any(f => Path.GetFileName(f).StartsWith(today));
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
