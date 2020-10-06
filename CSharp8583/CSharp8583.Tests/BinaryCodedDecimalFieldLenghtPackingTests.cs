using System.Linq;
using CSharp8583.Attributes;
using CSharp8583.Common;
using CSharp8583.Extensions;
using CSharp8583.Messages;
using Xunit;

namespace CSharp8583.Tests
{
    public class BinaryCodedDecimalFieldLenghtPackingTests
    {
        [Theory]
        [InlineData(LengthType.LVAR, 1)]
        [InlineData(LengthType.LLVAR, 1)]
        [InlineData(LengthType.LLLVAR, 2)]
        [InlineData(LengthType.LLLLVAR, 2)]
        public void TestByteCountRetrievalForLengthType(LengthType lengthType, int expected)
        {
            Assert.Equal(expected, lengthType.ToBytePackCount());
        }


        [Theory]
        [InlineData("19", LengthType.LVAR, "19")] // 1 byte
        [InlineData("19", LengthType.LLVAR, "19")] // 1 byte
        [InlineData("19", LengthType.LLLVAR, "0019")] // 2 byte
        [InlineData("19", LengthType.LLLLVAR, "0019")] // 2 byte
        public void TestConvertToBinaryCodedDecimalExtension(string value, LengthType lengthType, string expected)
        {
            var result = value.ConvertToBinaryCodedDecimal(false, lengthType.ToBytePackCount());

            string actual = string.Concat(result.Select(b => b.ToString("X2")));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestBinaryCodedDecimalFieldEncoding()
        {
            var expected = "30313030303030303030303030303031303030300003414243"; // (4 mti) + (16 bitmap) + (2 byte (field length) + payload)

            var serialiser = new Iso8583();

            var message = new TestIsoMessage
            {
                De48AdditionalData = "ABC"
            };

            var bytes = serialiser.Build(message, "0100");

            string actual = string.Concat(bytes.Select(b => b.ToString("X2")));

            

            Assert.Equal(expected, actual);
        }


        internal class TestIsoMessage : BaseMessage
        {
            [IsoField(position: IsoFields.F48, maxLen: 999, lengthType: LengthType.LLLVAR, contentType: ContentType.ANS, LenDataType = DataType.BCD)]
            public string De48AdditionalData { get; set; }
        }
    }
}
