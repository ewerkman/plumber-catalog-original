using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Plumber.Catalog.Attributes.Validation
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class MinMaxValidationAttribute : System.Attribute
    {
        public long MinValue { get; private set; }
        public long MaxValue { get; private set; }

        public MinMaxValidationAttribute(long minValue = int.MinValue, long maxValue = int.MaxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }
    }
}
