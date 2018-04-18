using PoweredSoft.Types.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace PoweredSoft.Types.Converters
{
    public class StringToGuidConverter : ITypeConverter
    {
        public bool CanConvert(Type source, Type destination) => source == typeof(string) && destination == typeof(Guid);
        public object Convert(object source, Type destination) => Guid.Parse((string)source);
    }
}
