using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.FtpClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Backup
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = args[0];
                var user = args[1];
                var pwd = args[2];
                var backupDir = args[3];

                string ftpDownloadDir = CreateTempDir();
                string backupBackupDir = CreateTempDir();

                try
                {
                    DownloadBackup(host, user, pwd, ftpDownloadDir);
                    BackupPreviousBackup(backupDir, backupBackupDir);
                    BackupFilesFromFtp(ftpDownloadDir, backupDir);
                    Directory.Delete(backupBackupDir, true);
                }
                finally
                {
                    Directory.Delete(ftpDownloadDir, true);
                }            
            }
            catch (Exception ex)
            {
                Environment.ExitCode = -1;
                MessageBox.Show(ex.Message, "JGDH Backup");
                EventLog.WriteEntry("JGDH Backup", ex.Message, EventLogEntryType.Error);
            }            
        }

        private static void BackupFilesFromFtp(string ftpDownloadDir, string backupDir)
        {
            foreach (var file in Directory.EnumerateFiles(ftpDownloadDir))
            {
                File.Copy(file, Path.Combine(backupDir, Path.GetFileName(file)));
            }
        }

        private static void BackupPreviousBackup(string backupDir, string backupBackupDir)
        {
            var previousBackupFiles = Directory.GetFiles(backupDir);
            foreach (var file in previousBackupFiles)
            {
                File.Copy(file, Path.Combine(backupBackupDir, Path.GetFileName(file)));
            }
            foreach (var file in previousBackupFiles)
            {
                File.Delete(file);
            }
        }

        private static void DownloadBackup(string host, string user, string pwd, string ftpDownloadDir)
        {
            using (var ftp = new FtpClient())
            {
                ftp.Host = host;
                ftp.Credentials = new NetworkCredential { UserName = user, Password = pwd };
                ftp.Connect();
                foreach (var file in ftp.GetNameListing("/site/wwwroot/App_Data/Backup"))
                {
                    using (var readStream = ftp.OpenRead(file, FtpDataType.Binary))
                    using (var writeStream = File.Create(Path.Combine(ftpDownloadDir, Path.GetFileName(file))))
                    {
                        readStream.CopyTo(writeStream);
                    }
                }
            }
        }

        private static string CreateTempDir()
        {
            string tmpDir;
            do
            {
                tmpDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            }
            while (Directory.Exists(tmpDir));
            Directory.CreateDirectory(tmpDir);
            return tmpDir;
        }
    }
}
