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
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Plumber.Catalog.Pipelines.Blocks
{
    [PipelineDisplayName("PopulateComponentActionsBlock")]
    public class PopulateComponentActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ViewCommander viewCommander;
        private readonly IGetSellableItemComponentsPipeline getSellableItemComponentsPipeline;

        public PopulateComponentActionsBlock(ViewCommander viewCommander, IGetSellableItemComponentsPipeline getSellableItemComponentsPipeline)
        {
            this.viewCommander = viewCommander;
            this.getSellableItemComponentsPipeline = getSellableItemComponentsPipeline;
        }

        public async override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument cannot be null.");

            var request = this.viewCommander.CurrentEntityViewArgument(context.CommerceContext);

            if(!(request.Entity is SellableItem))
            {
                return arg;
            }

            var sellableItem = (SellableItem)request.Entity;

            if (sellableItem != null)
            {
                List<Type> applicableComponentTypes = await GetApplicableComponentTypes(context, sellableItem);

                foreach (var componentType in applicableComponentTypes)
                {
                    if (string.IsNullOrEmpty(arg?.Name) || !arg.Name.Equals(componentType.FullName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var actionPolicy = arg.GetPolicy<ActionsPolicy>();

                    actionPolicy.Actions.Add(new EntityActionView
                    {
                        Name = $"Edit-{componentType.FullName}",
                        DisplayName = $"Edit {componentType.FullName}",
                        Description = "Edits the sellable item notes",
                        IsEnabled = true,
                        EntityView = arg.Name,
                        Icon = "edit"
                    });

                    break;
                }
            }
            
            return arg;
        }

        private async Task<List<Type>> GetApplicableComponentTypes(CommercePipelineExecutionContext context, SellableItem sellableItem)
        {
            // Get the item definition
            var catalogs = sellableItem.GetComponent<CatalogsComponent>();

            // TODO: What happens if a sellableitem is part of multiple catalogs?
            var catalog = catalogs.GetComponent<CatalogComponent>();
            var itemDefinition = catalog.ItemDefinition;

            var sellableItemComponentsArgument = new SellableItemComponentsArgument(itemDefinition);
            sellableItemComponentsArgument = await viewCommander.Pipeline<IGetSellableItemComponentsPipeline>().Run(sellableItemComponentsArgument, context);

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
    }
}
