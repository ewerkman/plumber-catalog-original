using Sitecore.Commerce.Core;
using Plugin.Plumber.Catalog.Attributes;

namespace Plugin.Plumber.Catalog.Sample.Components
{
    [EntityView("Notes for the user")]
    [ItemDefinition("Refrigerator")]
    [ItemDefinition("Product")]
    public class NotesComponent : Component
    {
        [Property("Warranty Information", isRequired: true)]
        public string WarrantyInformation { get; set; } = string.Empty;

        [Property("Internal Notes")]
        public string InternalNotes { get; set; } = string.Empty;
    }
}
