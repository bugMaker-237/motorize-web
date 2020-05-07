using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Motorize
{
  public static class CssClassInlineBuilder
  {
    public static string CssClass(object obj)
    {
      var boolPropNames = obj.GetType()
        // Enumarate all properties of the argument object.
        .GetProperties()
        // Filter the properties only it's type is "bool".
        .Where(p => p.PropertyType == typeof(bool))
        // Filter the properties only that value is "true".
        .Where(p => (bool)p.GetMethod.Invoke(obj, null))
        // Collect names of properties, and convert it to lower case.
        .Select(p => p.Name.ToLower());

      // Concatenate names of filtered properties with space separator.
      return string.Join(' ', boolPropNames);
    }
  }
}
