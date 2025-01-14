using AAAErp.ViewModel;
using System.Data;

namespace AAAErp.IProvider
{
    public interface IRawQuery
    {
        Task ExecuteCommand(DbConnectionVm dbConnection);
        Task<DataTable> ExecuteSqlQuery(DbConnectionVm dbConnection);
    }
}
