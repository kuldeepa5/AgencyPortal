using DevExpress.ExpressApp.Security;
using Microsoft.Extensions.Options;
using System;

namespace AgencyPortaXaf.Blazor.Server.Module
{
    public class CustomAuthenticationStandardProvider : AuthenticationStandardProviderV2
    {
        public CustomAuthenticationStandardProvider(IOptions<AuthenticationStandardProviderOptions> options,
        IOptions<SecurityOptions> securityOptions) :
            base(options, securityOptions)
        { }
        protected override AuthenticationBase CreateAuthentication(Type userType, Type logonParametersType)
        {
            return new CustomAuthentication();
        }
    }
}
