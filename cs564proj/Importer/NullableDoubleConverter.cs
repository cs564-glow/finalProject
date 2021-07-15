﻿using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Importer
{
    /// <summary>
    /// in our data set, \N is used to indicate null data.
    /// </summary>
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