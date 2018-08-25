[![Build status](https://ci.appveyor.com/api/projects/status/s5nq8u1fo4klxt0x?svg=true)](https://ci.appveyor.com/project/ewerkman/plumber-catalog)

# Plumber Catalog for Sitecore Commerce

> Plugin for Sitecore Commerce that allows you to add attributes to your catalog components for easy addition to the Sitecore Commerce Business tools.

## What is this?

You can extend a Sitecore Commerce Catalog in two ways:

* Use the Entity Composer to create templates that add new properties to your catalog;
* Create your own components that add properties as POCO classes and write pipeline blocks that add functionality to the Business tools to edit these properties;

The first way is easy to do but has the disadvantage that there is no (easy) typesafe way to work with these properties in code. 

The second way gives you type safety but it makes you write a lot of plumbing to support viewing and editing these properties in the business tools.

**Plumber Catalog** is a plugin for Sitecore Commerce that gives you the ability to add attributes to your catalog component classes that contain meta information about the component.  
It adds pipeline blocks to the IGetEntityViewPipeline (which is responsible for providing the views for the Business Tools) that take the meta information of your component and adds the appropriate views and actions to the business tools. 

This means you just add attributes to your catalog component class and Plumber Catalog will take care of the rest.

## How to use it? 

Add a dependency on Plumber.Catalog to the plugin that contains your catalog components. 

## Getting started

You want to extend a sellable item with information on the warranty. You want to add two fields: 

* An integer value indicating the number of months warranty you get with the product;
* A piece of text (string) giving more information on the warranty.

This is what your component would look like:


```c#
using Sitecore.Commerce.Core;

namespace Plugin.Plumber.Catalog.Sample.Components
{
	public class WarrantyComponent : Component
	{
		public int WarrantyLengthInMonths { get; set; }

		public string WarrantyInformation { get; set; }
	}
}
```

Now, if you want users to be able to edit the warranty information in the Merchandising Manager you would normally have to:

* Add a block to the `IGetEntityViewPipeline` to create an entity view for the Merchandising Manager
* Add a block to the `IPopulateEntityViewActionsPipeline` to add an action to the entity view so the user can edit the data.
* Add a block to the `IDoActionPipeline` to save the data the user edited.
* Add another block to the `IGetEntityViewPipeline` to handle updating the Sitecore template for a sellable item.

Instead, with __Plumber Catalog__ you do the following:

1. Add a dependency on the Plumber.Catalog Nuget package.
2. Change the `WarrantyComponent` class so it looks like this:


```c#
using Sitecore.Commerce.Core;
using Plugin.Plumber.Catalog.Attributes;

namespace Plugin.Plumber.Catalog.Sample.Components
{
	[EntityView("Warranty Information")]
	public class WarrantyComponent : Component
    {
        [Property("Warranty length (months)"]
        public int WarrantyLengthInMonths { get; set; }

        [Property("Additional warranty information")]
        public string WarrantyInformation { get; set; }
    }
}
```

3. Plumber.Catalog needs to know that the `WarrantyComponent` is a component that can be added to a `SellableItem`. You create a pipeline block to the `IGetSellab

## Available Attributes

### EntityViewAttribute

Add the `EntityVuewAttribute` to a class to indicate the class should be added as an entity view in the BizFx tools.

|Parameter|Description|
|---------|-----------|
|`ViewName`|Name of the view to show in the Merchandising Manager|


### ItemDefinitionAttribute

Add the `ItemDefinitionAttribute` to a component class to specify the item definition name this component should be added to.

|Parameter|Description|
|---------|-----------|
|`ItemDefinitnion`|Name of the item definition for which to add this component to a sellable item|


### PropertyAttribute

Add a `PropertyAttribute` to each property of the class you want 

|Parameter|Description|
|---------|-----------|
|`DisplayName`|Name of the property to show in the Merchandising Manager|
|`isReadOnly`|Set to `true` to indicate this property cannot be edited in the Merchandising Manager|
|`isRequired`|Set to `true` if this property is required.|


