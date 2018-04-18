using System;
using System.Collections.Generic;
using System.Text;

namespace PoweredSoft.Types.Interface
{
    public interface ITypeConverter
    {
        bool CanConvert(Type source, Type destination);
        object Convert(object source, Type destination);
    }
}
