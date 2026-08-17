using System;
using System.Collections.Generic;
using System.Text;
using filecabinet;
namespace TestProject1
{
    public class ConsoleReaderTests
    {
        [Fact]
        public void StringConverter_NonEmptyInput_TrimAndReturnsTrue()
        {
            var result = ConsoleReader.stringConverter("   abc ");

            Assert.True(result.Item1);
            Assert.Equal("abc",result.Item3);
        }
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void StringConverter_EmptyOrWhitespaceInput_ReturnsFalse(string input)
        {
            var result = ConsoleReader.stringConverter(input);

            Assert.False(result.Item1);
            Assert.Equal(string.Empty, result.Item3);
        }
        [Fact]
        public void DecimalConverter_ValidInput_ReturnsTrueAndParsedValue()
        {
            var result = ConsoleReader.DecimalConverter("70.5");

            Assert.True(result.Item1);
            Assert.Equal(70.5m, result.Item3);
        }

        [Fact]
        public void DecimalConverter_InvalidInput_ReturnsFalseAndDefault()
        {
            var result = ConsoleReader.DecimalConverter("abc");

            Assert.False(result.Item1);
            Assert.Equal(0m, result.Item3);
        }
        [Fact]
        public void DateTimeConverter_ValidInput_ReturnsTrueAndParsedDate()
        {
            var result = ConsoleReader.DateTimeConverter("1990-01-01");

            Assert.True(result.Item1);
            Assert.Equal(new DateTime(1990, 1, 1), result.Item3);
        }

        [Fact]
        public void DateTimeConverter_InvalidInput_ReturnsFalseAndDefault()
        {
            var result = ConsoleReader.DateTimeConverter("текст");

            Assert.False(result.Item1);
            Assert.Equal(default(DateTime), result.Item3);
        }

        [Fact]
        public void ShortConverter_ValidInput_ReturnsTrueAndParsedValue()
        {
            var result = ConsoleReader.ShortConverter("5");

            Assert.True(result.Item1);
            Assert.Equal((short)5, result.Item3);
        }

        [Fact]
        public void ShortConverter_InvalidInput_ReturnsFalseAndDefault()
        {
            var result = ConsoleReader.ShortConverter("abc");

            Assert.False(result.Item1);
            Assert.Equal(default(short), result.Item3);
        }

        [Fact]
        public void CharConverter_SingleCharInput_ReturnsTrueAndParsedChar()
        {
            var result = ConsoleReader.CharConverter("A");

            Assert.True(result.Item1);
            Assert.Equal('A', result.Item3);
        }

        [Fact]
        public void CharConverter_MultiCharInput_ReturnsFalseAndDefault()
        {
            var result = ConsoleReader.CharConverter("AB");

            Assert.False(result.Item1);
            Assert.Equal(default(char), result.Item3);
        }
    }
}
