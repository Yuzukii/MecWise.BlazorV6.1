using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MecWise.Blazor.Api.MicroService.Controllers {
    public class Authenticate {
        public static bool Auth(string token) {
            var secureDataFormat = new TicketDataFormat(new MachineKeyProtector());
            AuthenticationTicket ticket = secureDataFormat.Unprotect(token);
            //JObject tokenStore = new JObject();
            //tokenStore.Add("token", token);
            //tokenStore.Add("userId", ticket.Identity.Claims.Last().Value);
            //tokenStore.Add("expire", ticket.Properties.ExpiresUtc);

            if (ticket.Properties.ExpiresUtc >= DateTime.UtcNow) {
                return true;
            }
            
            return false;
        }
    }

    public class MachineKeyProtector : IDataProtector {
        private readonly string[] _purpose =
        {
            typeof(OAuthAuthorizationServerMiddleware).Namespace,
            "Access_Token",
            "v1"
        };

        public byte[] Protect(byte[] userData) {
            throw new NotImplementedException();
        }

        public byte[] Unprotect(byte[] protectedData) {
            return System.Web.Security.MachineKey.Unprotect(protectedData, _purpose);
        }
    }
}