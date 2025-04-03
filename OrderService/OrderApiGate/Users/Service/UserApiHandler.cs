using Core;

namespace OrderApiGate.Users.Service
{
    public class UserApiHandler(IHttpClientFactory httpClientFactory, ApiGateConfig config, ILogger<UserApiHandler> logger)
        : AbstractApiHandler<User, WriteUser>(httpClientFactory, logger), ICrudHandler<User, WriteUser>
    {
        protected override string GetEndpointUrl() => $"{config.UserApiEndpoint}/users";
    }
}
