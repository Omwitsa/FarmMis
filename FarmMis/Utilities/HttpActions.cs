using RestSharp;

namespace FarmMis.Utilities
{
    public class HttpActions
    {
        private readonly string _portalUrl;
        public HttpActions(string portalUrl)
        {
            _portalUrl = portalUrl;
        }

        public async Task<string> Get(string resourceUrl)
        {
            var restClient = new RestClient(_portalUrl);
            var restRequest = new RestRequest(resourceUrl, Method.Get) { RequestFormat = DataFormat.Json };
            var data = await restClient.ExecuteGetAsync(restRequest);
            return data.Content;
        }

        public async Task<string> Post(string resourceUrl, object entity)
        {
            var restClient = new RestClient(_portalUrl);
            var restRequest = new RestRequest(resourceUrl, Method.Post) { RequestFormat = DataFormat.Json };
            restRequest.AddBody(entity);
            var response = await restClient.ExecutePostAsync(restRequest);
            return response.Content;
        }
    }
}
