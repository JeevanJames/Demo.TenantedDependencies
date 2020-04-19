
using Microsoft.Extensions.Configuration;

namespace Console.Code
{
    public sealed class TenantIdentificationStrategy : ITenantIdentificationStrategy
    {
        private readonly IConfiguration _configuration;

        public TenantIdentificationStrategy(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string? GetTenant()
        {
            return _configuration["Tenant"] ?? "msft";
        }
    }
}
 