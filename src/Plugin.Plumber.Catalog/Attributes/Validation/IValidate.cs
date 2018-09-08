using Sitecore.Commerce.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace Plugin.Plumber.Catalog.Attributes.Validation
{
    public interface IValidate
    {
        Task<bool> Validate(string value, PropertyInfo prop, PropertyAttribute propertyAttribute, CommerceContext commerceContext);
    }
}
