using Plugin.Plumber.Catalog.Attributes;
using Plugin.Plumber.Catalog.Pipelines;
using Plugin.Plumber.Catalog.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Plumber.Catalog.Commanders
{
    public class CatalogSchemaCommander : CommerceCommander
    {
        public CatalogSchemaCommander(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<List<Type>> GetAllComponentTypes(CommercePipelineExecutionContext context, SellableItem sellableItem)
        {
            // Get the item definition
            var catalogs = sellableItem.GetComponent<CatalogsComponent>();

            // TODO: What happens if a sellableitem is part of multiple catalogs?
            var catalog = catalogs.GetComponent<CatalogComponent>();
            var itemDefinition = catalog.ItemDefinition;

            var sellableItemComponentsArgument = new SellableItemComponentsArgument(itemDefinition);
            sellableItemComponentsArgument = await this.Pipeline<IGetSellableItemComponentsPipeline>().Run(sellableItemComponentsArgument, context);

            return sellableItemComponentsArgument.SellableItemComponents;
        }

        public async Task<List<Type>> GetApplicableComponentTypes(CommercePipelineExecutionContext context, SellableItem sellableItem)
        {
            // Get the item definition
            var catalogs = sellableItem.GetComponent<CatalogsComponent>();

            // TODO: What happens if a sellableitem is part of multiple catalogs?
            var catalog = catalogs.GetComponent<CatalogComponent>();
            var itemDefinition = catalog.ItemDefinition;

            var sellableItemComponentsArgument = new SellableItemComponentsArgument(itemDefinition);
            sellableItemComponentsArgument = await this.Pipeline<IGetSellableItemComponentsPipeline>().Run(sellableItemComponentsArgument, context);

            var applicableComponentTypes = new List<Type>();
            foreach (var component in sellableItemComponentsArgument.SellableItemComponents)
            {
                System.Attribute[] attrs = System.Attribute.GetCustomAttributes(component);
                if (attrs.SingleOrDefault(attr => attr is ItemDefinitionAttribute && ((ItemDefinitionAttribute)attr).ItemDefinition == itemDefinition) is ItemDefinitionAttribute itemDefinitionAttribute)
                {
                    applicableComponentTypes.Add(component);
                }
            }

            return applicableComponentTypes;
        }

        public Component GetEditedComponent(SellableItem sellableItem, Type editedComponentType)
        {
            Sitecore.Commerce.Core.Component component = sellableItem.Components.SingleOrDefault(comp => comp.GetType() == editedComponentType);
            if (component == null)
            {
                component = (Component)Activator.CreateInstance(editedComponentType);
                sellableItem.Components.Add(component);
            }

            return component;
        }
    }
}
