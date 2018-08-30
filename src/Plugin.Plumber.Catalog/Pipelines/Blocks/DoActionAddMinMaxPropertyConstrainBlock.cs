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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Plumber.Catalog.Pipelines.Blocks
{
    [PipelineDisplayName("DoActionAddMinMaxPropertyConstrainBlock")]
    public class DoActionAddMinMaxPropertyConstraintBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CatalogSchemaCommander catalogSchemaCommander;

        public DoActionAddMinMaxPropertyConstraintBlock(CatalogSchemaCommander catalogSchemaCommander)
        {
            this.catalogSchemaCommander = catalogSchemaCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null.");

            // Only proceed if the right action was invoked
            if (string.IsNullOrEmpty(entityView.Action) || !entityView.Action.StartsWith("Edit-", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            // Get the sellable item from the context
            var sellableItem = context.CommerceContext.GetObject<SellableItem>(x => x.Id.Equals(entityView.EntityId));
            if (sellableItem == null)
            {
                return entityView;
            }

            var applicableComponentTypes = await this.catalogSchemaCommander.GetApplicableComponentTypes(context, sellableItem);
            var editedComponentType = applicableComponentTypes.SingleOrDefault(comp => entityView.Action == $"Edit-{comp.FullName}");

            if (editedComponentType != null)
            {
                // Get the component from the sellable item or its variation
                var editedComponent = catalogSchemaCommander.GetEditedComponent(sellableItem, editedComponentType);

                SetPropertyValuesOnEditedComponent(entityView.Properties, 
                    editedComponentType, editedComponent, context);
            }

            return entityView;
            /*

            if (true)
            {
 
            }

            return arg;
            */
        }

        private async void SetPropertyValuesOnEditedComponent(List<ViewProperty> properties,
                        Type editedComponentType,
                        Sitecore.Commerce.Core.Component editedComponent,
                        CommercePipelineExecutionContext context)
        {
            // Map entity view properties to component
            var props = editedComponentType.GetProperties();

            foreach (var prop in props)
            {
                Attribute[] propAttributes = Attribute.GetCustomAttributes(prop);

                if (propAttributes.SingleOrDefault(attr => attr is MinMaxValidationAttribute) is MinMaxValidationAttribute minMaxAttr)
                {
                    var fieldValueAsString = properties.FirstOrDefault(x => x.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase))?.Value;

                    try
                    {
                        decimal fieldValue;
                        if (decimal.TryParse(fieldValueAsString, out fieldValue))
                        {
                            if (fieldValue < minMaxAttr.MinValue || fieldValue > minMaxAttr.MaxValue)
                            {
                                KnownResultCodes errorCodes = context.CommerceContext.GetPolicy<KnownResultCodes>();
                                var str = await context.CommerceContext.AddMessage(errorCodes.ValidationError, "InvalidOrMissingPropertyValue", new object[1]
                                      {
                                        (object) "Blah"
                                      }, string.Format("Invalid or missing value for property '{0}'.", (object)"Blah"));

                            }
                        }
                    }
                    catch (Exception)
                    {
                       // logger.LogWarning($"Could not convert property '{prop.Name}' with value '{fieldValue}' to type {prop.PropertyType}");
                    }
                }
            }
        }

    }
}
