using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using MecWise.Blazor.Api.Services;
using Newtonsoft.Json.Linq;
using System.Web;
using MecWise.Blazor.Common;
using MecWise.Blazor.Api.Filters;

namespace MecWise.Blazor.Api.Controllers
{
    [Authorize]
    public class MenuController : ApiController
    {
        MenuService _menuService;

        public MenuController() {
            string[] sessionHeader = System.Web.HttpContext.Current.Request.Headers.GetValues("Session");
            if (sessionHeader != null) {
                string jsonStr = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(sessionHeader[0]));
                SessionState session = JObject.Parse(jsonStr).ToObject<SessionState>();
                _menuService = new MenuService(session);
            }
            else {
                _menuService = new MenuService();
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("Menu/GetNumberOfUnReadNoti/{compCode}/{empeId}")]
        public HttpResponseMessage GetNumberOfUnReadNoti(string compCode, string empeId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _menuService.GetNumberOfUnReadNoti(compCode, empeId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("Menu/GetMenu/{userId}/{langId}/{includeDetails}/{isMobile}")]
        public HttpResponseMessage GetMenu(string userId, int langId, bool includeDetails, bool isMobile)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, 
                    JArray.FromObject(_menuService.GetMenuPages(userId, langId, includeDetails, isMobile)));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("Menu/GetMenu/{userId}/{langId}/{includeDetails}/{menuId}/{isMobile}")]
        public HttpResponseMessage GetMenu(string userId, int langId, bool includeDetails, decimal menuId, bool isMobile)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    JArray.FromObject(_menuService.GetMenuPages(userId, langId, includeDetails, menuId, isMobile)));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("Menu/GetMenu/{userId}/{langId}/{menuId}/{menuItemId}/{menuSubItemId}/{isMobile}")]
        public HttpResponseMessage GetMenu(string userId, int langId, decimal menuId, decimal menuItemId, decimal menuSubItemId, bool isMobile)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _menuService.GetMenuPage(userId, langId, menuId, menuItemId, menuSubItemId, isMobile));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("Menu/GetMenuMobile/{userId}/{langId}/{includeDetails}/{isMobile}")]
        public HttpResponseMessage GetMenuMobile(string userId, int langId, bool includeDetails, bool isMobile) {
            try {
                return Request.CreateResponse(HttpStatusCode.OK,
                    JArray.FromObject(_menuService.GetMenuPages(userId, langId, includeDetails, isMobile)));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("Menu/GetQuickAccessMenu/{userId}/{langId}/isMobile")]
        public HttpResponseMessage GetQuickAccessMenu(string userId, int langId, bool isMobile) {
            try {
                return Request.CreateResponse(HttpStatusCode.OK, _menuService.GetQuickAccessMenu(userId, langId, isMobile));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
