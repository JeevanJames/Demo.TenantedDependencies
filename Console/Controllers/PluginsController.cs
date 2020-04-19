using Console.Code;
using Microsoft.AspNetCore.Mvc;

namespace Console.Controllers
{
    [ApiController]
    [Route("plugins")]
    public class PluginsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> GetValue([FromServices]PluginDependency<IMyService> myServicePlugin)
        {
            IMyService myService = myServicePlugin.Get();
            return myService.GetValue();
        }
    }
}
