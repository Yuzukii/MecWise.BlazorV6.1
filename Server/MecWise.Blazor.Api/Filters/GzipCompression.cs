using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Filters;
using System.Linq;
using MecWise.Blazor.Common;

namespace MecWise.Blazor.Api.Filters {
    public class GzipCompressionAttribute : ActionFilterAttribute {
        public override void OnActionExecuted(HttpActionExecutedContext actionContext) {
            
            if (actionContext.Request.Headers.AcceptEncoding.Contains(new StringWithQualityHeaderValue("gzip"))) {
                var content = actionContext.Response.Content;

                // do not compress response from microservice (RequestHandler = "microservice"), its already compressed
                string requestHandler = string.Empty;
                if (content.Headers.Contains("RequestHandler")) {
                    requestHandler = content.Headers.GetValues("RequestHandler").Last().ToStr();
                }

                if (requestHandler != "microservice") { 
                    var bytes = content == null ? null : content.ReadAsByteArrayAsync().Result;
                    var zlibbedContent = bytes == null ? new byte[0] :
                    CompressionHelper.GzipByte(bytes);
                    actionContext.Response.Content = new ByteArrayContent(zlibbedContent);
                    actionContext.Response.Content.Headers.Remove("Content-Type");
                    actionContext.Response.Content.Headers.Add("Content-encoding", "gzip");
                    actionContext.Response.Content.Headers.Add("Content-Type", "application/json");
                }
            }
            
            base.OnActionExecuted(actionContext);
        }
    }

    public class CompressionHelper {
        public static byte[] GzipByte(byte[] str) {
            if (str == null) {
                return null;
            }

            using (var output = new MemoryStream()) {
                using (var compressor = new Ionic.Zlib.GZipStream(output, Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestSpeed)) {
                    compressor.Write(str, 0, str.Length);
                }
                return output.ToArray();
            }
        }
    }
}