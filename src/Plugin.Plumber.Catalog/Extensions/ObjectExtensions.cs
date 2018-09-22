using Newtonsoft.Json;
using System;

namespace Plugin.Plumber.Catalog.Extensions
{
    public static class ObjectExtensions
    {    
        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var serializeSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace,
                TypeNameHandling = TypeNameHandling.Auto };

            var serializedObject = JsonConvert.SerializeObject(source, serializeSettings);
            var deserializedObject = JsonConvert.DeserializeObject(serializedObject, deserializeSettings);

            return (T)deserializedObject;
        }
    }
}
