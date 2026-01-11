using DBScriptsConvert.Models;
using DBScriptsConvert.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace DBScriptsConvert.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConvertController : ControllerBase
    {
        //½d¨Ò¡Gand(or(name:equals("adam"), and(age:equals(32),name:equals("john")), not(gender:equals("male")))

        private IConvertService _convertService;

        public ConvertController(IConvertService convertService)
        {
            _convertService = convertService;
        }

        /// <summary>
        /// Âà´«»yªk
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        [HttpGet("ConvertData")]
        public string ConvertData([FromQuery] ConvertParam param)
        {
            var result = _convertService.Convert(param.Script);
            
            return result.Script;
        }

    }
}
