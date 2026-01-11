using DBScriptsConvert.Models;
using DBScriptsConvert.Services;
using Xunit;

namespace DBScriptsConvert.Tests.Services
{
    public class ConvertTest
    {
        private readonly ConvertService _convertService;

        public ConvertTest()
        {
            _convertService = new ConvertService();
        }

        #region Equals 邏輯測試

        [Fact]
        public void Convert_SimpleEquals_ShouldConvertCorrectly()
        {
            // Arrange
            string input = "name:equals(\"adam\")";
            string expected = "WHERE name = \"adam\"";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        [Fact]
        public void Convert_MultipleEquals_ShouldConvertCorrectly()
        {
            // Arrange
            string input = "name:equals(\"adam\"), age:equals(32)";
            string expected = "WHERE name = \"adam\", age = 32";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        [Fact]
        public void Convert_NotEquals_ShouldConvertToNotEqual()
        {
            // Arrange
            string input = "name:!equals(\"adam\")";
            string expected = "WHERE name <> \"adam\"";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        [Fact]
        public void Convert_EqualsWithoutPropertyName_ShouldHandleCorrectly()
        {
            // Arrange
            string input = "equals(\"adam\")";
            // 預期會搜尋所有預設欄位
            string expected = "WHERE (name = \"adam\" OR description = \"adam\" OR age = \"adam\")";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        #endregion

        #region 無屬性名稱搜尋測試

        [Fact]
        public void Convert_EqualsWithoutPropertyName_ShouldSearchAllFields()
        {
            // Arrange
            string input = "equals(\"adam\")";
            
            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Contains("name = \"adam\"", result.Script);
            Assert.Contains("description = \"adam\"", result.Script);
            Assert.Contains("age = \"adam\"", result.Script);
            Assert.Contains("OR", result.Script);
        }

        [Fact]
        public void Convert_MixedEqualsWithAndWithoutPropertyName_ShouldConvertCorrectly()
        {
            // Arrange
            string input = "and(equals(\"adam\"), age:equals(32))";
            
            // Act
            var result = _convertService.Convert(input);

            // Assert
            // 應該包含所有欄位的搜尋和特定的 age 欄位
            Assert.Contains("name = \"adam\"", result.Script);
            Assert.Contains("description = \"adam\"", result.Script);
            Assert.Contains("age = 32", result.Script);
            Assert.Contains("AND", result.Script);
        }

        [Fact]
        public void Convert_EqualsWithoutPropertyNameInComplexExpression_ShouldConvertCorrectly()
        {
            // Arrange
            string input = "or(equals(\"john\"), name:equals(\"adam\"))";
            
            // Act
            var result = _convertService.Convert(input);

            // Assert
            // 第一個參數應該搜尋所有欄位
            Assert.Contains("name = \"john\"", result.Script);
            Assert.Contains("description = \"john\"", result.Script);
            Assert.Contains("age = \"john\"", result.Script);
            // 第二個參數只搜尋 name 欄位
            Assert.Contains("name = \"adam\"", result.Script);
            Assert.Contains("OR", result.Script);
        }

        #endregion

        #region NOT 邏輯測試

        [Fact]
        public void Convert_NotWithEquals_ShouldConvertToNotEqual()
        {
            // Arrange
            string input = "not(gender:equals(\"male\"))";
            string expected = "WHERE gender <> \"male\"";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        [Fact]
        public void Convert_NotWithEqualSign_ShouldConvertToNotEqual()
        {
            // Arrange
            string input = "not(gender = \"male\")";
            string expected = "WHERE gender <> \"male\"";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        #endregion

        #region AND 邏輯測試

        [Fact]
        public void Convert_SimpleAnd_ShouldConvertCorrectly()
        {
            // Arrange
            string input = "and(name:equals(\"adam\"), age:equals(32))";
            string expected = "WHERE (name = \"adam\" AND age = 32)";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        [Fact]
        public void Convert_NestedAnd_ShouldConvertCorrectly()
        {
            // Arrange
            string input = "and(name:equals(\"adam\"), and(age:equals(32), gender:equals(\"male\")))";
            string expected = "WHERE (name = \"adam\" AND (age = 32 AND gender = \"male\"))";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        #endregion

        #region OR 邏輯測試

        [Fact]
        public void Convert_SimpleOr_ShouldConvertCorrectly()
        {
            // Arrange
            string input = "or(name:equals(\"adam\"), name:equals(\"john\"))";
            string expected = "WHERE (name = \"adam\" OR name = \"john\")";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        [Fact]
        public void Convert_NestedOr_ShouldConvertCorrectly()
        {
            // Arrange
            string input = "or(or(name:equals(\"adam\"), name:equals(\"john\")), age:equals(32))";
            string expected = "WHERE ((name = \"adam\" OR name = \"john\") OR age = 32)";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        #endregion

        #region 複雜組合測試

        [Fact]
        public void Convert_ComplexExpression_ShouldConvertCorrectly()
        {
            // Arrange
            string input = "and(or(name:equals(\"adam\"), age:equals(32)), not(gender:equals(\"male\")))";
            string expected = "WHERE ((name = \"adam\" OR age = 32) AND gender <> \"male\")";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        [Fact]
        public void Convert_VeryComplexExpression_ShouldConvertCorrectly()
        {
            // Arrange
            string input = "and(or(name:equals(\"adam\"), and(age:equals(32), name:equals(\"john\"))), not(gender:equals(\"male\")))";
            string expected = "WHERE ((name = \"adam\" OR (age = 32 AND name = \"john\")) AND gender <> \"male\")";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        #endregion

        #region 邊界條件測試

        [Fact]
        public void Convert_EmptyString_ShouldReturnWhereOnly()
        {
            // Arrange
            string input = "";
            string expected = "WHERE ";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        [Fact]
        public void Convert_NumericValue_ShouldHandleCorrectly()
        {
            // Arrange
            string input = "age:equals(32)";
            string expected = "WHERE age = 32";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.Equal(expected, result.Script);
        }

        #endregion

        #region 結果類型測試

        [Fact]
        public void Convert_ShouldReturnConvertData()
        {
            // Arrange
            string input = "name:equals(\"adam\")";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ConvertData>(result);
            Assert.NotNull(result.Script);
        }

        [Fact]
        public void Convert_ResultShouldStartWithWhere()
        {
            // Arrange
            string input = "name:equals(\"adam\")";

            // Act
            var result = _convertService.Convert(input);

            // Assert
            Assert.StartsWith("WHERE ", result.Script);
        }

        #endregion
    }
}