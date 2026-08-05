using filecabinet;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace Tests
{
    public class DefaultValidatorTests
    {
        //Arrange
        private static readonly RecordRequest ValidRequest = new(
            3, "Joe", "Ivanov", new DateTime(1950, 02, 12), 23, 120, 'a');
        private static readonly DefaultValidator Validator = new DefaultValidator();
        //Positive test
        [Fact]
        public void ValidateValidRequestWithoutException()
        {

            //Act
            var exception = Record.Exception(() => Validator.ValidateParameters(ValidRequest));

            //Assert
            Assert.Null(exception);
        }
        [Fact]
        public void ValidateEmptyFirstName()
        {
            //Arrange
            var invalidNameRequest = ValidRequest with { FirstName = "" };

            //Act
            var exception = Record.Exception(() => Validator.ValidateParameters(invalidNameRequest));
            //Assert
            Assert.NotNull(exception);
        }
        [Fact]
        public void ValidateEmptyLastname()
        {
            var invalidNameRequest = ValidRequest with { LastName = "" };
            var exception = Record.Exception(() => Validator.ValidateParameters(invalidNameRequest));
            Assert.NotNull(exception);
        }


        public static IEnumerable<object[]> GetInvalidDates()
        {
            yield return new object[] { new DateTime(1945, 05, 09) };
            yield return new object[] { new DateTime(2050, 01, 02) };
            yield return new object[] { DateTime.Now.AddDays(3) };
        }
        [Theory]
        [MemberData(nameof(GetInvalidDates))]
        public void ValidateInvalidDateOfBirth(DateTime invalidDate)
        {
            var invalidDateOfBirthRequest = new RecordRequest(3, "Joe", "Ivanov", invalidDate, 23, 120, 'a');

            Assert.Throws<ArgumentException>(() => Validator.ValidateParameters(invalidDateOfBirthRequest));

        }
        [Fact]
        public void ValidateInvalidArchiveId()
        {
            var invalidArchiveRequest = ValidRequest with { ArchiveId = -52 };
            Assert.Throws<ArgumentOutOfRangeException>(() => Validator.ValidateParameters(invalidArchiveRequest));
        }



        [Theory]
        [InlineData(-200)]
        [InlineData(1000000)]
        public void ValidateInvalidWeight(decimal invalidWeight)
        {
            var invalidWeightRequest = ValidRequest with { Weight = invalidWeight };
            Assert.Throws<ArgumentOutOfRangeException>(() => Validator.ValidateParameters(invalidWeightRequest));
        }
        [Fact]
        public void ValidateInvalidType()
        {
            var invalidTypeRequest = ValidRequest with { Type = ' ' };
            Assert.Throws<ArgumentException>(() => Validator.ValidateParameters(invalidTypeRequest));
        }
    }
}
