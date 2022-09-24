using System.Spatial;
using System.Xml;

namespace TestHelloWorld;

public static class BorrowedCode
{
    //Odata method
    private static string FormatRawLiteral(object value)
    {
        switch (value)
        {
            case string str:
                return str;
            case bool flag:
                return XmlConvert.ToString(flag);
            case byte num1:
                return XmlConvert.ToString(num1);
            case DateTime dateTime:
                return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.RoundtripKind);
            case Decimal num2:
                return XmlConvert.ToString(num2);
            case double num3:
                return LiteralFormatter.SharedUtils.AppendDecimalMarkerToDouble(XmlConvert.ToString(num3));
            case Guid _:
                return value.ToString();
            case short num4:
                return XmlConvert.ToString(num4);
            case int num5:
                return XmlConvert.ToString(num5);
            case long num6:
                return XmlConvert.ToString(num6);
            case sbyte num7:
                return XmlConvert.ToString(num7);
            case float num8:
                return XmlConvert.ToString(num8);
            case byte[] byteArray:
                return LiteralFormatter.ConvertByteArrayToKeyString(byteArray);
            case DateTimeOffset dateTimeOffset:
                return XmlConvert.ToString(dateTimeOffset);
            case TimeSpan timeSpan:
                return XmlConvert.ToString(timeSpan);
            case Geography geography:
                return WellKnownTextSqlFormatter.Create(true).Write((ISpatial) geography);
            case Geometry geometry:
                return WellKnownTextSqlFormatter.Create(true).Write((ISpatial) geometry);
            default:
                throw LiteralFormatter.SharedUtils.CreateExceptionForUnconvertableType(value);
        }
    }
}