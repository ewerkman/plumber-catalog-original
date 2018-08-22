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
using Plugin.Plumber.Catalog.Commanders;

namespace Plugin.Plumber.Catalog.Pipelines.Blocks
{
    [PipelineDisplayName("PopulateComponentActionsBlock")]
    public class PopulateComponentActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ViewCommander viewCommander;
        private readonly CatalogSchemaCommander catalogSchemaCommander;        

        public PopulateComponentActionsBlock(ViewCommander viewCommander, CatalogSchemaCommander catalogSchemaCommander)
        {
            this.viewCommander = viewCommander;
            this.catalogSchemaCommander = catalogSchemaCommander;
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
                List<Type> applicableComponentTypes = await this.catalogSchemaCommander.GetApplicableComponentTypes(context, sellableItem);

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
    }
}
