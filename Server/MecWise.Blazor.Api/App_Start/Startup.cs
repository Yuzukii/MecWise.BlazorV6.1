using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using MecWise.Security;
using MecWise.Blazor.Api.Services;
using MecWise.Blazor.Common;
using Newtonsoft.Json.Linq;
using System.Linq;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.DataHandler;

[assembly: OwinStartup(typeof(MecWise.Blazor.Api.Startup))]

namespace MecWise.Blazor.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Enable CORS
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
                        
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            var oauthProvider = new OAuthAuthorizationServerProvider
            {
                OnGrantRefreshToken = async context => {
                    var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
                    newIdentity.AddClaim(new Claim("user", "newValue"));

                    var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
                    context.Validated(newTicket);

                    await Task.FromResult("");
                    return; 
                },
                OnGrantResourceOwnerCredentials = async context =>
                {
                    //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });         
                    var reqObj = await context.Request.ReadFormAsync();
                    bool validUser = false;
                    if (reqObj["login_type"] == LoginType.oidcTokenAccess.ToStr()) {
                        validUser = this.IsValidOpenIdConnectUser(context.UserName);
                    }
                    else if (reqObj["login_type"] == LoginType.tokenAccess.ToStr())
                    {
                        validUser = this.IsValidTokenUser(context.UserName);
                    }
                    else
                    {
                        validUser = this.IsValidUser(context.UserName, context.Password);
                    }
                    if (validUser)
                    {
                        var claimsIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                        claimsIdentity.AddClaim(new Claim("user", context.UserName));
                        context.Validated(claimsIdentity);
                        await Task.FromResult("");
                        return;
                    }
                    context.Rejected();
                },
                OnValidateClientAuthentication = async context => 
                {
                    string clientId;
                    string clientSecret;
                    if (context.TryGetBasicCredentials(out clientId, out clientSecret))
                    {
                        string OAuthClientID = "mecwise.blazor";
                        string OAuthSecretKey = "st@rvisi0n";

                        if (clientId == OAuthClientID && clientSecret == OAuthSecretKey)
                        {
                            context.Validated();
                        }
                    }
                    await Task.FromResult("");
                },
                OnTokenEndpointResponse = async context => {
                    //var dataFile = "D:\\temp\\accessToken.txt";
                    //JObject tokenStore = new JObject();
                    //tokenStore.Add("token", context.AccessToken);
                    //tokenStore.Add("userId", context.Identity.Claims.Last().Value);
                    //System.IO.File.WriteAllText(dataFile, tokenStore.ToString());

                    //var secureDataFormat = new TicketDataFormat(new MachineKeyProtector());
                    //AuthenticationTicket ticket = secureDataFormat.Unprotect(context.AccessToken);
                    
                    await Task.FromResult("");
                }
            };

            TimeSpan expireTime;
            if (ApiSetting.SessionExpireTime == 0) // never expire
            {
                expireTime = new TimeSpan(3650, 0, 0, 0);
            }
            else {
                expireTime = new TimeSpan(0, ApiSetting.SessionExpireTime, 0); // set token expire time, default as 1 hour
            }

            var oauthOptions = new OAuthAuthorizationServerOptions 
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/accesstoken"),
                AccessTokenExpireTimeSpan = expireTime,  
                Provider = oauthProvider,
                RefreshTokenProvider = new ApplicationRefreshTokenProvider()
            };

            app.UseOAuthAuthorizationServer(oauthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }

        public bool IsValidTokenUser(string encryptedStr)
        {           
            LoginService loginService = new LoginService();
            return loginService.IsValidTokenUser(encryptedStr);
        }

        public bool IsValidOpenIdConnectUser(string encryptedStr) {
            LoginService loginService = new LoginService();
            return loginService.IsValidOpenIdConnectUser(encryptedStr);
        }

        public bool IsValidUser(string userId, string password)
        {
            MecWiseLogon mecwiseLogon = new MecWiseLogon();
            mecwiseLogon.ConnectionString = ApiSetting.ConnectionString;
            mecwiseLogon.UserName = userId;
            mecwiseLogon.Password = password;
            return mecwiseLogon.Logon();
        }
    }


    public class ApplicationRefreshTokenProvider : AuthenticationTokenProvider
    {
        public override void Create(AuthenticationTokenCreateContext context)
        {
            // set refresh token expire time, same as access token expire time
            DateTimeOffset expireTime;
            if (ApiSetting.SessionExpireTime == 0) // never expire
            {
                expireTime = new DateTimeOffset(DateTime.Now.AddDays(3650)); 
            }
            else
            {
                expireTime = new DateTimeOffset(DateTime.Now.AddMinutes(ApiSetting.SessionExpireTime)); ; // set token expire time, default as 1 hour
            }

            context.Ticket.Properties.ExpiresUtc = expireTime;
            context.SetToken(context.SerializeTicket());
        }

        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }
    }
}