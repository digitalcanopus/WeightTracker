using ApiGateway.Services;

namespace ApiGateway.Helpers
{
    public static class ServiceEndpointsHelper
    {
        public static EndpointsService GetServiceEndpoints(IConfiguration configuration)
        {
            var microserviceEndpoints = new EndpointsService();

            configuration.GetSection("Services").Bind(microserviceEndpoints);

            var userApiHost = Environment.GetEnvironmentVariable("USER_API_HOST");
            var weightApiHost = Environment.GetEnvironmentVariable("WEIGHT_API_HOST");

            if (!string.IsNullOrEmpty(userApiHost))
                microserviceEndpoints.UserMicroservice = userApiHost;

            if (!string.IsNullOrEmpty(weightApiHost))
                microserviceEndpoints.WeightMicroservice = weightApiHost;

            return microserviceEndpoints;
        }
    }
}
