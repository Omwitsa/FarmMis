using FarmMis.Controllers;
using FarmMis.IProvider;
using FarmMis.Utilities;
using FarmMis.ViewModel;
using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace FarmMis.Provider
{
    public class PortalProxy : IPortalProxy
    {
        private HttpActions _httpActions;
        private readonly ILogger<HomeController> _logger;
        public PortalProxy(IConfiguration _configuration, ILogger<HomeController> logger)
        {
            _logger = logger;
            string portalUrl = _configuration["Urls:PortalUrl"];
            _httpActions = new HttpActions(portalUrl);
        }
        public Task<string> GetPackList(string farm, string date)
        {
            var response = _httpActions.Get($"orders/packlist/packlist_data.php?farm={farm}&date={date}");
            return response;
        }

        public Task<string> PostScannedPacklist(PacklistLineVm packlistLineVm)
        {
            var response = _httpActions.Post("orders/packlist/function.php", packlistLineVm);
            return response;
        }

        /*
         public Task<string> ApplyLeave(LeaveApplication application)
        {
            var response = _httpActions.Post("leave/applyLeave", application);
            return response;
        } */
    }
}
