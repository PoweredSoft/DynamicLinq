using PoweredSoft.Types.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace PoweredSoft.Types.Converters
{
    public class StringToDateConverter : ITypeConverter
    {
        public bool CanConvert(Type source, Type destination) => source == typeof(string) && destination == typeof(DateTime);
        public object Convert(object source, Type destination) => DateTime.Parse((string)source);
    }
}
