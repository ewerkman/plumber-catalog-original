﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Plumber.Catalog.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class PropertyAttribute : System.Attribute
    {
        public string DisplayName { get; private set; }
        public bool IsReadOnly { get; private set; }
        public bool IsRequired { get; private set; }

        public PropertyAttribute(string displayName = "", bool isReadOnly = true, bool isRequired = false)
        {
            DisplayName = displayName;
            IsReadOnly = isReadOnly;
            IsRequired = isRequired;
        }
    }
}
