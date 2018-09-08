using Sitecore.Commerce.Core;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Plugin.Plumber.Catalog.Attributes.Validation
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class MinMaxValidationAttribute : ValidationAttribute
    {
        public long MinValue { get; private set; }
        public long MaxValue { get; private set; }

        public MinMaxValidationAttribute(long minValue = int.MinValue, long maxValue = int.MaxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        public override async Task<bool> Validate(string value, PropertyInfo prop, PropertyAttribute propertyAttribute, CommerceContext commerceContext)
        {
            try
            {
                if (decimal.TryParse(value, out decimal fieldValue))
                {
                    if (fieldValue < this.MinValue || fieldValue > this.MaxValue)
                    {
                        KnownResultCodes errorCodes = commerceContext.GetPolicy<KnownResultCodes>();
                        var str = await commerceContext.AddMessage(errorCodes.ValidationError, "InvalidPropertyValueRange", new object[1]
                              {
                                        propertyAttribute?.DisplayName ?? prop.Name
                              }, $"Value for property '{ propertyAttribute?.DisplayName ?? prop.Name }' ('{value}') should be between {this.MinValue} and '{this.MaxValue}'.");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    KnownResultCodes errorCodes = commerceContext.GetPolicy<KnownResultCodes>();
                    var str = await commerceContext.AddMessage(errorCodes.ValidationError, "InvalidPropertyValueRange", new object[1]
                          {
                                        propertyAttribute?.DisplayName ?? prop.Name
                          }, $"Value for property '{ propertyAttribute?.DisplayName ?? prop.Name }' is not a valid number '{value}'");
                    return false;
                }
            }
            catch (Exception)
            {
                // logger.LogWarning($"Could not convert property '{prop.Name}' with value '{fieldValue}' to type {prop.PropertyType}");
            }
            return false;
        }
    }
}
