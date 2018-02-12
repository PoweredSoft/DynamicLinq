using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Helpers
{
    public static class TypeHelpers
    {
        public static object ConvertFrom(Type type, object source)
        {
            object ret = null;

            // safe if null.
            if (source == null)
                return ret;

            // not nullable type.
            var notNullableType = Nullable.GetUnderlyingType(type);
            if (notNullableType == null)
            {
                ret = Convert.ChangeType(source, type);
                return ret;
            }

            // the ret.
            ret = Convert.ChangeType(source, notNullableType);
            return ret;
        }
    }
}
