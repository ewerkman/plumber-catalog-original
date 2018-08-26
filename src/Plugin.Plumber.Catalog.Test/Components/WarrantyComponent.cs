using Sitecore.Commerce.Core;
using Plugin.Plumber.Catalog.Attributes;

namespace Plugin.Plumber.Catalog.Sample.Components
{
    [EntityView("Warranty Information")]
    [ItemDefinition("Refrigerator")]
    public class WarrantyComponent : Component
    {
        [Property("Warranty length (months)")]
        public int WarrantyLengthInMonths { get; set; }

        [Property("Additional warranty information")]
        public string WarrantyInformation { get; set; }
    }
}
