using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;

namespace Plugin.Plumber.Catalog.Attributes.Validation
{
    public class RegExValidationAttribute : ValidationAttribute
    {
        public string Pattern { get; private set; }

        public RegExValidationAttribute(string pattern)
        {
            this.Pattern = pattern;
        }

        public override async Task<bool> Validate(string value, PropertyInfo prop, PropertyAttribute propertyAttribute, CommerceContext commerceContext)
        {
            Match m = Regex.Match(value, Pattern, RegexOptions.IgnoreCase);
            if(m.Success)
            {
                return true;
            }
            else
            {
                KnownResultCodes errorCodes = commerceContext.GetPolicy<KnownResultCodes>();
                var str = await commerceContext.AddMessage(errorCodes.ValidationError, "InvalidPropertyValueRegEx", new object[1]
                      {
                                        propertyAttribute?.DisplayName ?? prop.Name
                      }, $"Value for property '{ propertyAttribute?.DisplayName ?? prop.Name }' ('{value}') should match '{this.Pattern}'.");

                return false;
            }
        }
    }
}
