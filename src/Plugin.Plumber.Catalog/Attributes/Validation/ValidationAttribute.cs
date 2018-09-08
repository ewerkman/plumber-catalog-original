using Sitecore.Commerce.Core;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Plugin.Plumber.Catalog.Attributes.Validation
{
    public abstract class ValidationAttribute : Attribute, IValidate
    {
        public abstract Task<bool> Validate(string value, PropertyInfo prop, PropertyAttribute propertyAttribute, CommerceContext commerceContext);
    }
}
