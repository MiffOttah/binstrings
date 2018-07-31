using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    public class BinStringTypeConverter : TypeConverter
    {
        public BinStringTypeConverter()
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                return new BinString(str);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is BinString binstr)
            {
                if (destinationType == typeof(string))
                {
                    return binstr.ToBase64String();
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    var ctor = typeof(BinString).GetConstructor(new Type[] { typeof(string) });
                    return new InstanceDescriptor(ctor, new object[] { binstr.ToBase64String() });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
