using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Zidium.Api
{
    public static class ZidiumHttpClientFactory
    {
        public static IHttpClientFactory HttpClientFactory { get; private set; }

        public static void AddZidiumHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient("Zidium")
                .ConfigureHttpClient(httpClient =>
                {
                    httpClient.BaseAddress = new Uri(Client.Instance.Config.Access.Url);
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(".NET API " + Tools.ApiVersion);
                    httpClient.Timeout = Client.Instance.Config.Access.Timeout;
                });

            services.AddLogging(builder => builder.AddFilter("System.Net.Http.HttpClient.Zidium", LogLevel.None));
        }

        public static void SetHttpClientFactory(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }
    }
}
