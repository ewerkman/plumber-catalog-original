using Microsoft.Extensions.Logging;
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
    /// <summary>
    ///     Validates the data entered against the validation attributes. 
    /// </summary>
    [PipelineDisplayName(Constants.Pipelines.Blocks.DoActionAddValidationConstraintBlock)]
    public class DoActionAddValidationConstraintBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CatalogSchemaCommander catalogSchemaCommander;

        public DoActionAddValidationConstraintBlock(CatalogSchemaCommander catalogSchemaCommander)
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

            // Check if we have a sellable item on the context
            var sellableItem = context.CommerceContext.GetObject<SellableItem>(x => x.Id.Equals(entityView.EntityId));
            if (sellableItem == null)
            {
                return entityView;
            }

            var applicableComponentTypes = await this.catalogSchemaCommander.GetApplicableComponentTypes(context.CommerceContext, sellableItem);
            var editedComponentType = applicableComponentTypes.SingleOrDefault(comp => entityView.Action == $"Edit-{comp.FullName}");

            if (editedComponentType != null)
            {
                // Get the component from the sellable item or its variation
                var editedComponent = catalogSchemaCommander.GetEditedComponent(sellableItem, editedComponentType);

                var error = await ValidateConstraints(entityView.Properties,
                    editedComponentType, editedComponent, context);
            }

            return entityView;
        }

        private async Task<bool> ValidateConstraints(List<ViewProperty> properties,
                        Type editedComponentType,
                        Sitecore.Commerce.Core.Component editedComponent,
                        CommercePipelineExecutionContext context)
        {
            var error = false;
            var props = editedComponentType.GetProperties();

            foreach (var prop in props)
            {
                Attribute[] propAttributes = Attribute.GetCustomAttributes(prop);

                var propertyAttribute = propAttributes.SingleOrDefault(attr => attr is PropertyAttribute) as PropertyAttribute;

                if (propAttributes.SingleOrDefault(attr => attr is ValidationAttribute) is ValidationAttribute validationAttribute)
                {
                    var fieldValueAsString = properties.FirstOrDefault(x => x.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase))?.Value;

                    try
                    {
                        var valid = await validationAttribute.Validate(fieldValueAsString, prop, propertyAttribute, context.CommerceContext);

                        if (!valid)
                        {
                            error = true;
                        }

                    }
                    catch (Exception)
                    {
                        context.Logger.LogError($"Could not validate property '{prop.Name}' with value '{fieldValueAsString}' using validator '{validationAttribute.GetType().FullName}'");
                    }
                }
            }
            return error;
        }

    }
}
