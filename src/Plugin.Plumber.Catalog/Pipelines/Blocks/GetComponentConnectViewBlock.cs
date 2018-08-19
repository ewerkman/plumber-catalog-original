﻿using Plugin.Plumber.Catalog.Attributes;
using Plugin.Plumber.Catalog.Pipelines.Arguments;
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
    [PipelineDisplayName("GetComponentConnectViewBlock")]
    public class GetComponentConnectViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ViewCommander viewCommander;
        private readonly IGetSellableItemComponentsPipeline getSellableItemComponentsPipeline;

        public GetComponentConnectViewBlock(ViewCommander viewCommander, IGetSellableItemComponentsPipeline getSellableItemComponentsPipeline)
        {
            this.viewCommander = viewCommander;
            this.getSellableItemComponentsPipeline = getSellableItemComponentsPipeline;
        }

        public async override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
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
            var isConnectView = arg.Name.Equals(catalogViewsPolicy.ConnectSellableItem, StringComparison.OrdinalIgnoreCase);            

            // Make sure that we target the correct views
            if (!isConnectView)
            {
                return arg;
            }

            List<Type> componentTypes = await GetComponentTypes(context, sellableItem);

            var targetView = arg;

            foreach (var componentType in componentTypes)
            {
                System.Attribute[] attrs = System.Attribute.GetCustomAttributes(componentType);

                var component = sellableItem.Components.SingleOrDefault(comp => comp.GetType() == componentType);

                if (attrs.SingleOrDefault(attr => attr is EntityViewAttribute) is EntityViewAttribute entityViewAttribute)
                {
                    // Check if the edit action was requested
                    // Create a new view and add it to the current entity view.
                    var view = new EntityView
                    {
                        Name = componentType.FullName.Replace(".", "_"),
                        DisplayName = entityViewAttribute?.ViewName ?? componentType.Name,
                        EntityId = arg.EntityId,
                        EntityVersion = arg.EntityVersion
                    };

                    arg.ChildViews.Add(view);

                    targetView = view;

                    var props = componentType.GetProperties();

                    foreach (var prop in props)
                    {
                        System.Attribute[] propAttributes = System.Attribute.GetCustomAttributes(prop);

                        if (propAttributes.SingleOrDefault(attr => attr is PropertyAttribute) is PropertyAttribute propAttr)
                        {
                            targetView.Properties.Add(new ViewProperty
                            {
                                Name = prop.Name,
                                DisplayName = propAttr.DisplayName,
                                RawValue = component != null ? prop.GetValue(component) : "",
                                IsReadOnly = propAttr.IsReadOnly,
                                IsRequired = propAttr.IsRequired
                            });
                        }
                    }
                }

            }

            return arg;
        }

        private async Task<List<Type>> GetComponentTypes(CommercePipelineExecutionContext context, SellableItem sellableItem)
        {
            // Get the item definition
            var catalogs = sellableItem.GetComponent<CatalogsComponent>();

            // TODO: What happens if a sellableitem is part of multiple catalogs?
            var catalog = catalogs.GetComponent<CatalogComponent>();
            var itemDefinition = catalog.ItemDefinition;

            var sellableItemComponentsArgument = new SellableItemComponentsArgument(itemDefinition);
            sellableItemComponentsArgument = await viewCommander.Pipeline<IGetSellableItemComponentsPipeline>().Run(sellableItemComponentsArgument, context);

            return sellableItemComponentsArgument.SellableItemComponents;
        }
    }
}