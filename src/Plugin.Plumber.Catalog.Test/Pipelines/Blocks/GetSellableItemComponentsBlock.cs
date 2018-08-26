using Sitecore.Commerce.Core;
using Plugin.Plumber.Catalog.Pipelines.Arguments;
using Sitecore.Framework.Pipelines;
using System.Threading.Tasks;
using Plugin.Plumber.Catalog.Sample.Components;

namespace Plugin.Plumber.Catalog.Sample.Pipelines.Blocks
{
    public class GetSellableItemComponentsBlock : PipelineBlock<SellableItemComponentsArgument, SellableItemComponentsArgument, CommercePipelineExecutionContext>
    {
        public async override Task<SellableItemComponentsArgument> Run(SellableItemComponentsArgument arg, CommercePipelineExecutionContext context)
        {
            arg.Add<NotesComponent>();
            arg.Add<SampleComponent>();
            arg.Add<WarrantyComponent>();

            return await Task.FromResult<SellableItemComponentsArgument>(arg);
        }
    }
}
