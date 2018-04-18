using PoweredSoft.Types.Converters;
using PoweredSoft.Types.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoweredSoft.Types
{
    public static class Converter
    {
        public static List<ITypeConverter> Converters { get; internal set; } = new List<ITypeConverter>
        {
            new StringToDateConverter(),
            new StringToGuidConverter()
        };

        public static void RegisterConverter(ITypeConverter converter)
        {
            lock (Converters)
            {
                Converters.Add(converter);
            }
        }

        public static void ReplaceConverter(ITypeConverter converter, Type source, Type destination)
        {
            lock (Converters)
            {
                Converters.RemoveAll(t => t.CanConvert(source, destination));
                Converters.Add(converter);
            }
        }

        public static object To(this object source, Type type)
        {
            object ret = null;

            // safe if null.
            if (source == null)
                return ret;

            // establish final type.
            var notNullType = Nullable.GetUnderlyingType(type);
            var finalType = notNullType ?? type;
            var converter = Converters.FirstOrDefault(t => t.CanConvert(source.GetType(), finalType));
            if (converter != null)
                ret = converter.Convert(source, finalType);
            else
                ret = Convert.ChangeType(source, finalType);

            if (notNullType != null)
                ret = Activator.CreateInstance(type, new object[] { ret });

            return ret;
        }
    }
}
