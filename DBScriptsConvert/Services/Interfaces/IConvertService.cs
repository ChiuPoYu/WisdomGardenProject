using DBScriptsConvert.Models;

namespace DBScriptsConvert.Services.Interfaces
{
    /// <summary>
    /// 轉換Service
    /// </summary>
    public interface IConvertService
    {

        /// <summary>
        /// 轉換服務
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public ConvertData Convert(string script);
    }
}
