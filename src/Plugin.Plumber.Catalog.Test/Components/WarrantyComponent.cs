using Sitecore.Commerce.Core;
using Plugin.Plumber.Catalog.Attributes;
using Plugin.Plumber.Catalog.Attributes.Validation;

namespace Plugin.Plumber.Catalog.Sample.Components
{
    [EntityView("Warranty Information")]
    [AllSellableItems]
    public class WarrantyComponent : Component
    {
        [Property("Warranty length (months)", showInList:true)]
        [MinMaxValidation(minValue: 12, maxValue: 24)]
        public int WarrantyLengthInMonths { get; set; }

        [Property("Additional warranty information", showInList:true)]
        [RegExValidation(pattern: "^(Days|Months|Years)$")]
        public string WarrantyInformation { get; set; }
    }
}
