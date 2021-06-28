using System;
using System.Text.RegularExpressions;
using FileHelpers;

namespace Importer
{
    public class ConverterRemoveYearFromTitle : ConverterBase
    {
        public override object StringToField(string from)
        {
            return Regex.Replace(from, @" \(\d{4}\)", "");
        }
    }
}