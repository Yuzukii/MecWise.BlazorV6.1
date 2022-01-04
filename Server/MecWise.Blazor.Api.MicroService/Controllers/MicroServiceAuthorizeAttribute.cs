using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace MecWise.Blazor.Api.MicroService.Controllers {
    public class MicroServiceAuthorizeAttribute : AuthorizeAttribute {
        public override void OnAuthorization(HttpActionContext actionContext) {
            if (AuthorizeRequest(actionContext)) {
                return;
            }
            HandleUnauthorizedRequest(actionContext);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext) {
            //Code to handel unauthorized requests
            HttpContext.Current.Response.AddHeader("AuthenticationStatus", "NotAuthorized");
            JObject errObj = new JObject();
            errObj.Add("error", "invalid_token");
            actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, errObj);
            return;
        }

        private bool AuthorizeRequest(HttpActionContext actionContext) {
            try {
                if (actionContext.Request.Headers.Authorization != null) {
                    var token = actionContext.Request.Headers.Authorization.Parameter;
                    return Authenticate.Auth(token);
                }
            }
            catch (Exception) {
                return false;
            }
            
            return false;
        }
    }

}