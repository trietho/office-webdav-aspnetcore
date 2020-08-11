## link examples 

ms-word:ofe|u|http://yoursite/yourcontroller/demo.docx

ms-word:ofe|u|http://yoursite/yourcontroller/sessionid/demo.docx

ms-excel:ofe|u|http://yoursite/yourcontroller/demo.xslx

## Sample controller

```cs

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeWebDav;

namespace MiniDav.Controllers
{

    [Route("[controller]")]
    public class FilesController : OfficeWebDavBaseController
    {
        private IWebHostEnvironment _env;
        public FilesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public override async Task<IActionResult> GetFile(string sessionid = "", string filename = "")
        {
            var filePath = Path.Combine(_env.ContentRootPath, "data/" + filename);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            //TODO: send the content-type properly for office documents
            return PhysicalFile(
                Path.Combine(_env.ContentRootPath, "data/" + filename),
                //"application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                );

        }

        public override async Task<IActionResult> PutFile(string sessionid = "", string filename = "")
        {
            using (var fileStream = System.IO.File.Create(Path.Combine(_env.ContentRootPath, "data/" + filename)))
            {
                await Request.Body.CopyToAsync(fileStream);
            }
            
            return Ok();
        }
    }
}
```