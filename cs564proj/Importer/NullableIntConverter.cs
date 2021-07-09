using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

/// <summary>
/// in our data set, \N is used to indicate null data.
/// </summary>
namespace Importer
{
    class NullableIntConverter : Int32Converter
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
