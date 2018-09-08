using Sitecore.Commerce.Core;
using Plugin.Plumber.Catalog.Attributes;
using Plugin.Plumber.Catalog.Attributes.Validation;

namespace Plugin.Plumber.Catalog.Sample.Components
{
    [EntityView("Warranty Information")]
    [ItemDefinition("Refrigerator")]
    public class WarrantyComponent : Component
    {
        [Property("Warranty length (months)")]
        [MinMaxValidation(minValue: 12, maxValue: 24)]
        public int WarrantyLengthInMonths { get; set; }

        [Property("Additional warranty information")]
        [RegExValidation(pattern: "^(Days|Months|Years)$")]
        public string WarrantyInformation { get; set; }
    }
}
