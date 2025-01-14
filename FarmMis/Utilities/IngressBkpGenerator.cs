using AAAErp.IProvider;
using AAAErp.Models;
using AAAErp.Utilities;
using AAAErp.ViewModel;
using MySql.Data.MySqlClient;
using System.IO.Compression;

namespace AAAErp.Utilities
{
    public abstract class IngressBkpGenerator
    {
        protected readonly CoreDbContext _context;
        protected readonly IWebHostEnvironment _env;
        protected readonly IRawQuery _rawQuery;

        public IngressBkpGenerator(CoreDbContext context, IWebHostEnvironment env, IRawQuery rawQuery)
        {
            _context = context;
            _env = env;
            _rawQuery = rawQuery;
        }

        public abstract Task<ReturnData<dynamic>> Backup(string siteName);
    }
}

public class BackupUpdator : IngressBkpGenerator
{
    public BackupUpdator(CoreDbContext context, IWebHostEnvironment env, IRawQuery rawQuery) : base(context, env, rawQuery)
    {
    }

    public override async Task<ReturnData<dynamic>> Backup(string siteName)
    {
        try
        {
            var site = _context.Sites.FirstOrDefault(s => s.Name.ToUpper().Equals(siteName.ToUpper()));
            if (site == null)
                return new ReturnData<dynamic>
                {
                    Message = "Sorry, Site could not be found",
                    Success = false
                };

            var query = "select * from attendance;";
            var queryResp = await _rawQuery.ExecuteSqlQuery(new DbConnectionVm
            {
                Query = query,
                Server = site.IngressServer,
                UserName = site.IngressUserName,
                Password = site.IngressPassword,
                Db = site.IngressDb,
            });

            var setup = _context.SysSetup.FirstOrDefault();
            return new ReturnData<dynamic>
            {
                Message = "Ingress Backuped successfully",
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new ReturnData<dynamic>
            {
                Success = false,
                Message = "Sorry, An error occurred"
            };
        }
    }
}

public class BackupCopierNUpdator : IngressBkpGenerator
{
    public BackupCopierNUpdator(CoreDbContext context, IWebHostEnvironment env, IRawQuery rawQuery) : base(context, env, rawQuery)
    {
    }

    public override async Task<ReturnData<dynamic>> Backup(string siteName)
    {
        try
        {
            var site = _context.Sites.FirstOrDefault(s => s.Name.ToUpper().Equals(siteName.ToUpper()));
            if (site == null)
                return new ReturnData<dynamic>
                {
                    Message = "Sorry, Site could not be found",
                    Success = false
                };

            var setup = _context.SysSetup.FirstOrDefault();
            setup.BackupLoc = string.IsNullOrEmpty(setup.BackupLoc) ? Path.Combine(_env.WebRootPath, "MysqlBackup") : setup.BackupLoc;
            string newPath = Path.Combine(setup.BackupLoc, site.Name, "Backup");
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);
            string filePath = Path.Combine(newPath, "backup.sql");

            var backupResp = await BackupIngress(filePath, setup, site);
            if (!backupResp.Success)
                return new ReturnData<dynamic>
                {
                    Message = backupResp.Message,
                    Success = false
                };
           

            var zipResp = await ZipBackup(setup, site);
            if (!zipResp.Success)
                return new ReturnData<dynamic>
                {
                    Message = zipResp.Message,
                    Success = false
                };
            

            var importResp = await ImportBackup(filePath, setup, site);
            if (!importResp.Success)
                return new ReturnData<dynamic>
                {
                    Message = importResp.Message,
                    Success = false
                };

            return new ReturnData<dynamic>
            {
                Message = "Ingress Backuped successfully",
                Success = true
            };
        }
        catch (Exception)
        {
            return new ReturnData<dynamic>
            {
                Message = "Sorry, An error occurred",
                Success = false
            };
        }
    }

    private async Task<ReturnData<string>> BackupIngress(string filePath, SysSetup setup, Site site)
    {
        try
        {
            string constring = $"server={site.IngressServer};user={site.IngressUserName};pwd={Decryptor.Decrypt(site.IngressPassword)};database={site.IngressDb};";
            var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var backupMonthStartDate = month.AddMonths(-setup.IngressBackMonths);
            var dic = new Dictionary<string, string>();
            dic["attendance"] = $"SELECT * FROM `attendance` WHERE date >= (STR_TO_DATE('{backupMonthStartDate}', '%d/%m/%Y'));";
            dic["leavetype"] = "SELECT * FROM `leavetype`;";
            dic["schedule"] = "SELECT * FROM `schedule`;";
            dic["user"] = "SELECT * FROM `user`;";
            dic["user_group"] = "SELECT * FROM `user_group`;";
            dic["user_info"] = "SELECT * FROM `user_info`;";

            using (MySqlConnection connection = new MySqlConnection(constring))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    using (MySqlBackup backup = new MySqlBackup(command))
                    {
                        command.Connection = connection;
                        command.CommandTimeout = 0;
                        connection.Open();

                        List<string> lstHeaders = new List<string>();
                        lstHeaders.Add("USE `" + site.HoIngressDb + "`;");
                        backup.ExportInfo.SetDocumentHeaders(lstHeaders);
                        backup.ExportInfo.TablesToBeExportedDic = dic;
                        backup.ExportInfo.EnableComment = true;
                        backup.ExportInfo.ExportFunctions = false;
                        backup.ExportInfo.ExportViews = false;
                        backup.ExportInfo.ExportTriggers = false;
                        backup.ExportInfo.ExportEvents = false;
                        backup.ExportInfo.ExportProcedures = false;
                        backup.ExportInfo.ExportRoutinesWithoutDefiner = false;
                        backup.ExportInfo.ResetAutoIncrement = true;
                        backup.ExportInfo.MaxSqlLength = 1024 * 1024;

                        List<string> lstFooters = new List<string>();
                        lstFooters.Add("");
                        backup.ExportInfo.SetDocumentFooters(lstFooters);

                        backup.ExportToFile(filePath);
                    }
                }
            }

            return new ReturnData<string>
            {
                Success = true,
            };
        }
        catch (Exception ex)
        {
            return new ReturnData<string>
            {
                Success = false,
                Message = "Sorry, An error occurred"
            };
        }
    }

    private async Task<ReturnData<string>> ZipBackup(SysSetup setup, Site site)
    {
        try
        {
            setup.BackupLoc = string.IsNullOrEmpty(setup.BackupLoc) ? Path.Combine(_env.WebRootPath, "MysqlBackup") : setup.BackupLoc;
            string sourcePath = Path.Combine(setup.BackupLoc, site.Name, "Backup");
            var date = $"{DateTime.Today.Year}-{DateTime.Today.Month}-{DateTime.Today.Day}";
            var backupName = $"backup.sql__{date}__.zip";
            string destinationPath = Path.Combine(setup.BackupLoc, site.Name, backupName);
            if (!Directory.Exists(sourcePath))
                Directory.CreateDirectory(sourcePath);

            FileInfo file = new FileInfo(destinationPath);
            if (file.Exists)
                file.Delete();

            ZipFile.CreateFromDirectory(
                sourcePath,
                destinationPath
            );

            site.LastBackup = backupName;
            _context.SaveChanges();
            return new ReturnData<string>
            {
                Success = true,
            };
        }
        catch (Exception ex)
        {
            return new ReturnData<string>
            {
                Success = false,
                Message = "Sorry, An error occurred"
            };
        }
    }

    private async Task<ReturnData<string>> ImportBackup(string filePath, SysSetup setup, Site site)
    {
        try
        {
            string constring = $"server={setup.IngressServer};user={setup.IngressUserName};pwd={Decryptor.Decrypt(setup.IngressPassword)};database={site.HoIngressDb};";
            using (MySqlConnection connection = new MySqlConnection(constring))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    using (MySqlBackup backup = new MySqlBackup(command))
                    {
                        command.Connection = connection;
                        command.CommandTimeout = 0;
                        connection.Open();
                        backup.ImportFromFile(filePath);
                        connection.Close();
                    }
                }
            }

            return new ReturnData<string>
            {
                Success = true,
            };
        }
        catch (Exception ex)
        {
            return new ReturnData<string>
            {
                Success = false,
                Message = "Sorry, An error occurred"
            };
        }
    }

}
