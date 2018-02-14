using PMApi.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Web.Http;

namespace PMApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            ConfigureOAuth(app);

            app.UseCors(CorsOptions.AllowAll);

            //app.MapSignalR();

            WebApiConfig.Register(config);
            app.UseWebApi(config);
            //SwaggerConfig.Register();

        }

        public void ConfigureOAuth(IAppBuilder app)
        {

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/v1/Authenticate"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                //AccessTokenProvider=new CustomTokenProvider(),
                Provider = new ApplicationOAuthProvider()
            };


            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}