using DBScriptsConvert.Models;
using DBScriptsConvert.Services.Interfaces;
using System.Text.RegularExpressions;

namespace DBScriptsConvert.Services
{
    /// <summary>
    /// 語法轉換Service
    /// </summary>
    public class ConvertService : IConvertService
    {
        // 定義預設要搜尋的欄位列表（可以從設定檔或資料庫讀取)
        private readonly List<string> _defaultSearchFields = new List<string>
        {
            "name", "description", "age"
        };

        /// <summary>
        /// 轉換邏輯
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public ConvertData Convert(string script)
        {
            var result = "WHERE ";

            #region Equals邏輯操作
            string pattern = @"(\w*):?!?equals\(([^)]+)\)";

            var matches = Regex.Matches(script, pattern);

            foreach (Match match in matches)
            {
                string fullEqualsOperator = match.Groups[0].Value;
                string propertyName = match.Groups[1].Value;
                string propertyValue = match.Groups[2].Value;

                // 檢查是否有屬性名稱
                if (string.IsNullOrEmpty(propertyName))
                {
                    // 情況1: equals("adam") - 沒有屬性名稱，搜尋所有欄位
                    var orConditions = _defaultSearchFields
                        .Select(field => $"{field} = {propertyValue}")
                        .ToList();
                    
                    string allFieldsCondition = $"({string.Join(" OR ", orConditions)})";
                    script = script.Replace(fullEqualsOperator, allFieldsCondition);
                }
                else
                {
                    // 情況2: name:equals("adam") - 有屬性名稱，搜尋特定欄位
                    if (fullEqualsOperator.Contains('!'))
                    {
                        script = script.Replace(fullEqualsOperator, $"{propertyName} <> {propertyValue}");
                    }
                    else
                    {
                        script = script.Replace(fullEqualsOperator, $"{propertyName} = {propertyValue}");
                    }
                }

                Console.WriteLine(script);
            }
            #endregion

            #region 否邏輯操作的Operator
            string patternNot = @"not\(([^=]+)\s*=?\s*([^)]+)\)";
            var matchesNot = Regex.Matches(script, patternNot);
            foreach (Match match in matchesNot)
            {
                string fullOperator = match.Groups[0].Value;
                string firstParameter = match.Groups[1].Value.Trim();
                string secondParameter = match.Groups[2].Value.Trim();

                if (fullOperator.Contains('='))
                {
                    script = script.Replace(fullOperator, $"{firstParameter} <> {secondParameter}");
                }
                else
                {
                    script = script.Replace(fullOperator, $"{firstParameter} !equals {secondParameter}");
                }

                Console.WriteLine(script);
            }
            #endregion

            #region 布林邏輯操作
            // 使用正則表達式找出所有 and( 和 or( 的位置
            var processOrder = new Dictionary<int, OperatorKeyWord>();
            string patternAndPosition = @"and\(";
            string patternOrPosition = @"or\(";

            // 找出所有 AND 的位置
            var matchesAnd = Regex.Matches(script, patternAndPosition);
            foreach (Match match in matchesAnd)
            {
                processOrder.Add(match.Index, OperatorKeyWord.And);
                Console.WriteLine($"位置 {match.Index}: AND");
            }

            // 找出所有 OR 的位置
            var matchesOr2 = Regex.Matches(script, patternOrPosition);
            foreach (Match match in matchesOr2)
            {
                processOrder.Add(match.Index, OperatorKeyWord.Or);
                Console.WriteLine($"位置 {match.Index}: OR");
            }

            // 按照位置排序處理 (右至左)
            processOrder.OrderByDescending(p => p.Key)
                .ToList()
                .ForEach(p =>
                {
                    switch (p.Value)
                    {
                        case OperatorKeyWord.And:
                            string patternAnd = @"and\(([^,]+),\s*([^)]+)\)";
                            var matcheAnd = Regex.Matches(script.Substring(p.Key), patternAnd).FirstOrDefault();
                            if (matcheAnd != null)
                            {
                                script = script.Replace(matcheAnd.Groups[0].Value, $"({matcheAnd.Groups[1].Value} AND {matcheAnd.Groups[2].Value})");
                            }
                            break;
                        case OperatorKeyWord.Or:
                            string patternOr = @"or\(([^,]+),\s*([^)]+)\)";
                            var matcheOr = Regex.Matches(script.Substring(p.Key), patternOr).FirstOrDefault();
                            if (matcheOr != null)
                            {
                                script = script.Replace(matcheOr.Groups[0].Value, $"({matcheOr.Groups[1].Value} OR {matcheOr.Groups[2].Value})");
                            }
                            break;
                    }
                });
            #endregion

            return new ConvertData
            {
                Script = result+=script
            };
        }
    }
}
