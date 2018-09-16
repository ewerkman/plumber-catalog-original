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
    /// <summary>
    ///     Helper class 
    /// </summary>
    public class CatalogSchemaCommander : CommerceCommander
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public CatalogSchemaCommander(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        ///     Returns all registered component types  
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<List<Type>> GetAllComponentTypes(CommerceContext context)
        {
            var sellableItemComponentsArgument = new SellableItemComponentsArgument();
            sellableItemComponentsArgument = await this.Pipeline<IGetSellableItemComponentsPipeline>().Run(sellableItemComponentsArgument, context.GetPipelineContext());

            return sellableItemComponentsArgument.SellableItemComponents;
        }

        /// <summary>
        ///     Retrieves all component types applicable for the sellable item
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sellableItem">Sellable item for which to get the applicable components</param>
        /// <returns></returns>
        public async Task<List<Type>> GetApplicableComponentTypes(CommerceContext context, SellableItem sellableItem)
        {
            // Get the item definition
            var catalogs = sellableItem.GetComponent<CatalogsComponent>();

            // TODO: What happens if a sellableitem is part of multiple catalogs?
            var catalog = catalogs.GetComponent<CatalogComponent>();
            var itemDefinition = catalog.ItemDefinition;

            var sellableItemComponentsArgument = new SellableItemComponentsArgument();
            sellableItemComponentsArgument = await this.Pipeline<IGetSellableItemComponentsPipeline>().Run(sellableItemComponentsArgument, context.GetPipelineContext());

            var applicableComponentTypes = new List<Type>();
            foreach (var component in sellableItemComponentsArgument.SellableItemComponents)
            {
                System.Attribute[] attrs = System.Attribute.GetCustomAttributes(component);

                if (attrs.Any(attr => attr is AllSellableItemsAttribute))
                {
                    applicableComponentTypes.Add(component);
                }
                else if (attrs.Any(attr => attr is ItemDefinitionAttribute && ((ItemDefinitionAttribute)attr).ItemDefinition == itemDefinition))
                {
                    applicableComponentTypes.Add(component);
                }

            }

            return applicableComponentTypes;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="sellableItem"></param>
        /// <param name="editedComponentType"></param>
        /// <returns></returns>
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
