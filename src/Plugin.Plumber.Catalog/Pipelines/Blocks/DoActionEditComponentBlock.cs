using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Plugin.Plumber.Catalog.Attributes;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Plumber.Catalog.Commanders;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Plugin.Plumber.Catalog.Pipelines.Blocks
{
    /// <summary>
    ///     Applies the entered data to the component that is being edited.
    /// </summary>
    [PipelineDisplayName(Constants.Pipelines.Blocks.DoActionEditComponentBlock)]
    public class DoActionEditComponentBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CatalogSchemaCommander catalogSchemaCommander;

        public DoActionEditComponentBlock(CatalogSchemaCommander catalogSchemaCommander)
        {
            this.catalogSchemaCommander = catalogSchemaCommander;
        }

        public async override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
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

            KnownResultCodes errorCodes = context.CommerceContext.GetPolicy<KnownResultCodes>();
            if (context.CommerceContext.AnyMessage(msg => msg.Code == errorCodes.ValidationError))
            {   // We found an error
                return entityView;
            }

            var applicableComponentTypes = await this.catalogSchemaCommander.GetApplicableComponentTypes(context.CommerceContext, sellableItem);
            var editedComponentType = applicableComponentTypes.SingleOrDefault(comp => entityView.Action == $"Edit-{comp.FullName}");

            if (editedComponentType != null)
            {
                // Get the component from the sellable item or its variation
                var editedComponent = catalogSchemaCommander.GetEditedComponent(sellableItem, editedComponentType);

                SetPropertyValuesOnEditedComponent(entityView.Properties, editedComponentType, editedComponent, context.CommerceContext);

                // Persist changes
                await this.catalogSchemaCommander.Pipeline<IPersistEntityPipeline>().Run(new PersistEntityArgument(sellableItem), context);
            }

            return entityView;
        }

        private void SetPropertyValuesOnEditedComponent(List<ViewProperty> properties,
            Type editedComponentType,
            Sitecore.Commerce.Core.Component editedComponent,
            CommerceContext context)
        {
            // Map entity view properties to component
            var props = editedComponentType.GetProperties();

            foreach (var prop in props)
            {
                System.Attribute[] propAttributes = System.Attribute.GetCustomAttributes(prop);

                if (propAttributes.SingleOrDefault(attr => attr is PropertyAttribute) is PropertyAttribute propAttr)
                {
                    var fieldValue = properties.FirstOrDefault(x => x.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase))?.Value;

                    TypeConverter converter = TypeDescriptor.GetConverter(prop.PropertyType);
                    if (converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(prop.PropertyType))
                    {
                        try
                        {
                            object propValue = converter.ConvertFromString(fieldValue);
                            prop.SetValue(editedComponent, propValue);
                        }
                        catch (Exception)
                        {
                            context.Logger.LogError($"Could not convert property '{prop.Name}' with value '{fieldValue}' to type '{prop.PropertyType}'");
                        }
                    }
                }
            }
        }
    }
}
