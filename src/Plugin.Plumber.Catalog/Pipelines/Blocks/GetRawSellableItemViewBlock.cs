using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Plumber.Catalog.Pipelines.Blocks
{
    public class GetRawSellableItemViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public async override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            EntityViewArgument entityViewArgument = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault<EntityViewArgument>();
            if (!(entityViewArgument?.Entity is SellableItem))
                return entityView;

            EntityView rawView = entityView.ChildViews.FirstOrDefault<Model>((Func<Model, bool>)(p => p.Name.Equals("Raw", StringComparison.OrdinalIgnoreCase))) as EntityView;

            return await Task.FromResult(entityView);
        }
    }
}
