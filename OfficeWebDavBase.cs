using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OfficeWebDav
{
    [OfficeWebDavHeadersFilter]
    public abstract class OfficeWebDavBaseController : Controller
    {
        [HttpGet]
        [Route("{filename}")]
        [Route("{sessionid}/{filename}")]
        public abstract  Task<IActionResult> GetFile(string sessionid = "", string filename = "");

        [HttpPut]
        [Route("{filename}")]
        [Route("{sessionid}/{filename}")]
        public abstract Task<IActionResult> PutFile(string sessionid = "", string filename = "");

        [AcceptVerbs("LOCK")]
        [Route("{fileName}")]
        [Route("{sessionid}/{filename}")]
        public virtual IActionResult Lock(string sessionid = "", string filename = "")
        {
            return LockFile(Guid.NewGuid());
        }

        [HttpHead]
        [HttpOptions]
        [Route("")]
        [Route("{fileName}")]
        [Route("{sessionid}/{filename}")]
        public virtual IActionResult Options()
        {
            return Ok();
        }

        [AcceptVerbs("UNLOCK")]
        [Route("{fileName}")]
        [Route("{sessionid}/{filename}")]
        public virtual IActionResult Unlock(string sessionid = "", string filename = "")
        {
            return StatusCode(200);
        }


        protected IActionResult LockFile(Guid lockToken, string author = "Anonymous", int timeout = 3600)
        {
            var responseXML = @"
                <?xml version=""1.0"" encoding=""utf-8""?>
                <D:prop xmlns:D=""DAV:"">
                    <D:lockdiscovery>
                        <D:activelock>
                            <D:locktype>
                                <write/>
                            </D:locktype>
                            <D:lockscope>
                                <exclusive/>
                            </D:lockscope>
                            <D:locktoken>
                                <D:href>urn:uuid:{0}</D:href>
                            </D:locktoken>
                            <D:lockroot>
                                <D:href>{1}</D:href>
                            </D:lockroot>
                            <D:depth>infinity</D:depth>
                            <D:owner>
                                <a:href xmlns:a=""DAV:"">{2}</a:href>
                            </D:owner>
                            <D:timeout>Second-{3}</D:timeout>
                        </D:activelock>
                    </D:lockdiscovery>
                </D:prop>            
            ";

            var fileUrl = getRequestUrl();

            var responseContent = string.Format(responseXML, lockToken.ToString(), fileUrl, author, timeout);
            Response.ContentType = "application/xml; charset=utf-8";

            return Content(responseContent);
        }

        protected string getRequestUrl()
        {
            return string.Concat(
                        Request.Scheme,
                        "://",
                        Request.Host.ToUriComponent(),
                        Request.PathBase.ToUriComponent(),
                        Request.Path.ToUriComponent(),
                        Request.QueryString.ToUriComponent());
        }
    }
}
