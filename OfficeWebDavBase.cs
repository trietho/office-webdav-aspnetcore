using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OfficeWebDav
{
    [OfficeWebDavHeadersFilter]
    public abstract class OfficeWebDavBaseController: Controller
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
        public virtual Task<IActionResult> Lock(string sessionid = "", string filename = "")
        {
            var lockToken = Guid.NewGuid();
            Response.ContentType = "application/xml; charset=utf-8";
            Response.Headers.Add("Lock-Token", lockToken.ToString());
            
            return LockFile(Guid.NewGuid());
        }

        [HttpHead]
        [HttpOptions]
        [Route("")]
        [Route("{fileName}")]
        [Route("{sessionid}/{filename}")]
        public virtual Task<IActionResult> Options()
        {
            return Task.FromResult(Ok() as IActionResult);
        }

        [AcceptVerbs("UNLOCK")]
        [Route("{fileName}")]
        [Route("{sessionid}/{filename}")]
        public virtual Task<IActionResult> Unlock(string sessionid = "", string filename = "")
        {
            return Task.FromResult(StatusCode(200) as IActionResult);
        }


        protected Task<IActionResult> LockFile(Guid lockToken, string author = "Anonymous", int timeout = 3600)
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

            return Task.FromResult(Content(responseContent) as IActionResult);
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
