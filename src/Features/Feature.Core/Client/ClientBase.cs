namespace Feature.Core.Client
{
    public class ClientBase
    {
        protected readonly IHttpClientFactory _httpClientFactory;

        public ClientBase(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
    }
}
