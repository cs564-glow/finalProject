using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Importer
{
    public class NullableDoubleConverter : DoubleConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text == @"\N")
            {
                return null;
            }
            return base.ConvertFromString(text, row, memberMapData);
        }
    }
}