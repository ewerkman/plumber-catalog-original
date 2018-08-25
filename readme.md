[![Build status](https://ci.appveyor.com/api/projects/status/s5nq8u1fo4klxt0x?svg=true)](https://ci.appveyor.com/project/ewerkman/plumber-catalog)

# Plumber Catalog for Sitecore Commerce

> Plugin for Sitecore Commerce that allows you to add attributes to your catalog components for easy addition to the Sitecore Commerce Business tools.

## What is this?

You can extend a Sitecore Commerce Catalog in two ways:

* Use the Entity Composer to create templates that add new properties to your catalog;
* Create your own components that add properties as POCO classes and write pipeline blocks that add functionality to the Business tools to edit these properties;

The first way is easy to do but has the disadvantage that there is no typesafeway to work with these properties in code. 

The second way gives you type safety but it makes you write a lot of plumbing to support viewing and editing these properties in the business tools.

Plumber Catalog is a plugin for Sitecore Commerce that gives you the ability to add attributes to your catalog component classes that contain meta information about the component.  
It also adds pipeline blocks to the IGetEntityViewPipeline (which is responsible for providing the views for the Business Tools) that take the meta information of your component and adds the appropriate views and actions to the business tools. 

This means you just add attributes to your catalog component class and Plumber Catalog will take care of the rest.