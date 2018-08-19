using Sitecore.Commerce.Core;
using System;
using System.Collections.Generic;

namespace Plugin.Plumber.Catalog.Pipelines.Arguments
{
    public class SellableItemComponentsArgument : PipelineArgument
    {
        public string ItemDefinition { get; set; }
        public List<Type> SellableItemComponents { get; private set; }

        public SellableItemComponentsArgument(string itemDefition)
        {
            ItemDefinition = itemDefition;
            SellableItemComponents = new List<Type>();
        }
    }
}
