using Sitecore.Commerce.Core;
using Plugin.Plumber.Catalog.Pipelines.Arguments;
using Sitecore.Framework.Pipelines;
using System.Threading.Tasks;
using Plugin.Plumber.Catalog.Test.Components;

namespace Plugin.Plumber.Catalog.Test.Pipelines.Blocks
{
    public class GetSellableItemComponentsBlock : PipelineBlock<SellableItemComponentsArgument, SellableItemComponentsArgument, CommercePipelineExecutionContext>
    {
        public async override Task<SellableItemComponentsArgument> Run(SellableItemComponentsArgument arg, CommercePipelineExecutionContext context)
        {
            arg.SellableItemComponents.Add(typeof(NotesComponent));
            arg.SellableItemComponents.Add(typeof(SampleComponent));

            return await Task.FromResult<SellableItemComponentsArgument>(arg);
        }
    }
}
