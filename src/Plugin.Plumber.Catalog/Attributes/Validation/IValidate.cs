using Sitecore.Commerce.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace Plugin.Plumber.Catalog.Attributes.Validation
{
    /// <summary>
    ///     
    /// </summary>
    public interface IValidate
    {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prop"></param>
        /// <param name="propertyAttribute"></param>
        /// <param name="commerceContext"></param>
        /// <returns></returns>
        Task<bool> Validate(string value, PropertyInfo prop, PropertyAttribute propertyAttribute, CommerceContext commerceContext);
    }
}
