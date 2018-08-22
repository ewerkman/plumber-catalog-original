using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Plugin.Plumber.Catalog.Attributes;
using Plugin.Plumber.Catalog.Pipelines.Arguments;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Plumber.Catalog.Commanders;

namespace Plugin.Plumber.Catalog.Pipelines.Blocks
{
    [PipelineDisplayName("DoActionEditComponentBlock")]
    public class DoActionEditComponentBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CatalogSchemaCommander catalogSchemaCommander;

        public DoActionEditComponentBlock(CatalogSchemaCommander catalogSchemaCommander)
        {
            this.catalogSchemaCommander = catalogSchemaCommander;
        }

        public async override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument cannot be null.");

            // Only proceed if the right action was invoked
            if (string.IsNullOrEmpty(arg.Action) || !arg.Action.StartsWith("Edit-", StringComparison.OrdinalIgnoreCase))
            {
                return arg;
            }

            // Get the sellable item from the context
            var sellableItem = context.CommerceContext.GetObject<SellableItem>(x => x.Id.Equals(arg.EntityId));
            if (sellableItem == null)
            {
                return arg;
            }

            var applicableComponentTypes = await this.catalogSchemaCommander.GetApplicableComponentTypes(context, sellableItem);
            var editedComponentType = applicableComponentTypes.SingleOrDefault(comp => arg.Action == $"Edit-{comp.FullName}");

            if (editedComponentType != null)
            {
                // Get the component from the sellable item or its variation
                Component component = (Component) sellableItem.Components.SingleOrDefault(comp => comp.GetType() == editedComponentType);
                if (component == null)
                {
                    component = (Component)Activator.CreateInstance(editedComponentType);
                    sellableItem.Components.Add(component);
                }

                // Map entity view properties to component
                var props = editedComponentType.GetProperties();

                foreach (var prop in props)
                {
                    System.Attribute[] propAttributes = System.Attribute.GetCustomAttributes(prop);

                    if (propAttributes.SingleOrDefault(attr => attr is PropertyAttribute) is PropertyAttribute propAttr)
                    {
                        prop.SetValue(component, arg.Properties.FirstOrDefault(x => x.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase))?.Value);
                    }
                }
                //component.WarrantyInformation = arg.Properties.FirstOrDefault(x => x.Name.Equals(nameof(NotesComponent.WarrantyInformation), StringComparison.OrdinalIgnoreCase))?.Value;
                //component.InternalNotes = arg.Properties.FirstOrDefault(x => x.Name.Equals(nameof(NotesComponent.InternalNotes), StringComparison.OrdinalIgnoreCase))?.Value;

                // Persist changes
                await this.catalogSchemaCommander.Pipeline<IPersistEntityPipeline>().Run(new PersistEntityArgument(sellableItem), context);
            }

            return arg;
        }
    }
}
