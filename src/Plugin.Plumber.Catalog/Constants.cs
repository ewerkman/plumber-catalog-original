﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Plumber.Catalog
{
    public static class Constants
    {
        /// <summary>
        ///     The names of the Plumber.Catalog pipelines.
        /// </summary>
        public static class Pipelines
        {
            public const string GetSellableItemComponentsPipeline = "Plumber.Catalog.IGetSellableItemComponentsPipeline";

            /// <summary>
            ///     The names of the Plumber.Catalog blocks.
            /// </summary>
            public static class Blocks
            {
                public const string DoActionAddValidationConstraintBlock = "Plumber.Catalog.DoActionAddValidationConstraintBlock";
                public const string DoActionEditComponentBlock = "Plumber.Catalog.DoActionEditComponentBlock";
                public const string GetAddMinMaxPropertyConstraintViewBlock = "Plumber.Catalog.GetAddMinMaxPropertyConstraintViewBlock";
                public const string GetComponentConnectViewBlock = "Plumber.Catalog.GetComponentConnectViewBlock";
                public const string GetComponentViewBlock = "Plumber.Catalog.GetComponentViewBlock";
                public const string GetRawSellableItemViewBlock = "Plumber.Catalog.GetRawSellableItemViewBlock";
                public const string PopulateComponentActionsBlock = "Plumber.Catalog.PopulateComponentActionsBlock";
                public const string ConfigureServiceApiBlock = "Plumber.Catalog.ConfigureServiceApiBlock";
            }

        }
    }
}
