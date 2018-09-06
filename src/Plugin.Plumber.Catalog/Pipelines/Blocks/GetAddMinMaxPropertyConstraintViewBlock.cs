using Plugin.Plumber.Catalog.Attributes;
using Plugin.Plumber.Catalog.Attributes.Validation;
using Plugin.Plumber.Catalog.Commanders;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Plumber.Catalog.Pipelines.Blocks
{
    public class GetAddMinMaxPropertyConstraintViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ViewCommander viewCommander;
        private readonly CatalogSchemaCommander catalogSchemaCommander;

        public GetAddMinMaxPropertyConstraintViewBlock(ViewCommander viewCommander, CatalogSchemaCommander catalogSchemaCommander)
        {
            this.viewCommander = viewCommander;
            this.catalogSchemaCommander = catalogSchemaCommander;
        }

        public override async Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument cannot be null.");
            var request = this.viewCommander.CurrentEntityViewArgument(context.CommerceContext);

            // Only proceed if the current entity is a sellable item
            if (!(request.Entity is SellableItem))
            {
                return arg;
            }

            var sellableItem = (SellableItem)request.Entity;

            var catalogViewsPolicy = context.GetPolicy<KnownCatalogViewsPolicy>();
            var isCatalogView = request.ViewName.Equals(catalogViewsPolicy.Master, StringComparison.OrdinalIgnoreCase);
            var isVariationView = request.ViewName.Equals(catalogViewsPolicy.Variant, StringComparison.OrdinalIgnoreCase);
            var isEditView = arg.Action.StartsWith("Edit-", StringComparison.OrdinalIgnoreCase);

            // Make sure that we target the correct views
            if ((isCatalogView || isVariationView) && !isEditView)
            {
                return arg;
            }

            List<Type> applicableComponentTypes = await this.catalogSchemaCommander.GetApplicableComponentTypes(context, sellableItem);

            // See if we are dealing with the base sellable item or one of its variations.
            var variationId = string.Empty;
            if (isVariationView && !string.IsNullOrEmpty(arg.ItemId))
            {
                variationId = arg.ItemId;
            }

            var mainView = arg;
            
            var componentType = applicableComponentTypes.SingleOrDefault(comp => comp.FullName == mainView.Name);

            if(componentType == null)
            {
                return arg;
            }

            System.Attribute[] componentAttrs = System.Attribute.GetCustomAttributes(componentType);

            var component = sellableItem.Components.SingleOrDefault(comp => comp.GetType() == componentType);

            if (componentAttrs.SingleOrDefault(attr => attr is EntityViewAttribute) is EntityViewAttribute entityViewAttribute)
            {
                var view = mainView;
                if (view != null)
                {   // We found the view for this component: get properties that have a MinMaxValidationAttribute
                    //System.Attribute.GetCustomAttribute(prop, typeof(MinMaxValidationAttribute))
                    foreach (var prop in componentType.GetProperties())
                    {
                        if (System.Attribute.GetCustomAttribute(prop, typeof(MinMaxValidationAttribute)) is MinMaxValidationAttribute propertyAttr)
                        {
                            var viewProperty = view.Properties.SingleOrDefault(vp => vp.Name == prop.Name);
                            if (viewProperty != null)
                            {
                                var minMaxViewPolicy = viewProperty.GetPolicy<MinMaxValuePolicy>();
                                minMaxViewPolicy.MinAllow = propertyAttr.MinValue;
                                minMaxViewPolicy.MaxAllow = propertyAttr.MaxValue;
                            }
                        }
                    }
                }

            }

            return arg;
        }
    }
}
