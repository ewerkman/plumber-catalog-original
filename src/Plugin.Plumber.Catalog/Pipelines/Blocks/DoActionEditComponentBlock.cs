﻿using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Plugin.Plumber.Catalog.Attributes;
using Plugin.Plumber.Catalog.Pipelines.Arguments;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Plumber.Catalog.Commanders;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Plugin.Plumber.Catalog.Pipelines.Blocks
{
    [PipelineDisplayName("DoActionEditComponentBlock")]
    public class DoActionEditComponentBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CatalogSchemaCommander catalogSchemaCommander;

        public DoActionEditComponentBlock(CatalogSchemaCommander catalogSchemaCommander)
        {
            this.catalogSchemaCommander = catalogSchemaCommander;
        }

        public async override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null.");

            // Only proceed if the right action was invoked
            if (string.IsNullOrEmpty(entityView.Action) || !entityView.Action.StartsWith("Edit-", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            // Get the sellable item from the context
            var sellableItem = context.CommerceContext.GetObject<SellableItem>(x => x.Id.Equals(entityView.EntityId));
            if (sellableItem == null)
            {
                return entityView;
            }

            var applicableComponentTypes = await this.catalogSchemaCommander.GetApplicableComponentTypes(context, sellableItem);
            var editedComponentType = applicableComponentTypes.SingleOrDefault(comp => entityView.Action == $"Edit-{comp.FullName}");

            if (editedComponentType != null)
            {
                // Get the component from the sellable item or its variation
                var component = GetComponent(sellableItem, editedComponentType);

                SetPropertyValuesOnComponent(editedComponentType, component, entityView, context);

                // Persist changes
                await this.catalogSchemaCommander.Pipeline<IPersistEntityPipeline>().Run(new PersistEntityArgument(sellableItem), context);
            }

            return entityView;
        }

        private Sitecore.Commerce.Core.Component GetComponent(SellableItem sellableItem, Type editedComponentType)
        {
            Sitecore.Commerce.Core.Component component = sellableItem.Components.SingleOrDefault(comp => comp.GetType() == editedComponentType);
            if (component == null)
            {
                component = (Sitecore.Commerce.Core.Component)Activator.CreateInstance(editedComponentType);
                sellableItem.Components.Add(component);
            }

            return component;
        }

        private void SetPropertyValuesOnComponent(Type editedComponentType,
            Sitecore.Commerce.Core.Component editedComponent,
            EntityView entityView,
            CommercePipelineExecutionContext context)
        {
            // Map entity view properties to component
            var props = editedComponentType.GetProperties();

            foreach (var prop in props)
            {
                System.Attribute[] propAttributes = System.Attribute.GetCustomAttributes(prop);

                if (propAttributes.SingleOrDefault(attr => attr is PropertyAttribute) is PropertyAttribute propAttr)
                {
                    var fieldValue = entityView.Properties.FirstOrDefault(x => x.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase))?.Value;

                    TypeConverter converter = TypeDescriptor.GetConverter(prop.PropertyType);
                    if (converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(prop.PropertyType))
                    {
                        try
                        {
                            object propValue = converter.ConvertFromString(fieldValue);
                            prop.SetValue(component, propValue);
                        }
                        catch (Exception)
                        {
                            context.Logger.LogWarning($"Could not convert property '{prop.Name}' with value '{fieldValue}' to type {prop.PropertyType}");
                        }
                    }
                }
            }
        }
    }
}
