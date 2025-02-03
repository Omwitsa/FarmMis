
using FarmMis.ViewModel;

namespace FarmMis.IProvider
{
    public interface IPortalProxy
    {
        Task<string> GetPackList(string farm, string date);
        Task<string> PostScannedPacklist(PacklistLineVm packlistLineVm);
    }
}
