using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Toprope.Infrastructure.Storage
{
    /// <summary>
    /// Represents a repository settings.
    /// </summary>
    public class RepositorySettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the list of value converters.
        /// </summary>
        public IList<ValueConverter> Converters { get; set; }

        /// <summary>
        /// Gets or sets the property mappings.
        /// </summary>
        private IDictionary<Type, IDictionary<string, PropertyInfo>> PropertyMappings { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public RepositorySettings()
        {
            Converters = new List<ValueConverter>();

            Converters.Add(new LocationConverter());
            Converters.Add(new TagListConverter());
            Converters.Add(new RouteGradeConverter());

            PropertyMappings = new Dictionary<Type, IDictionary<string, PropertyInfo>>();
        }

        /// <summary>
        /// Returns the list of all matching value converters for the given types.
        /// </summary>
        /// <param name="fromType">Type to convert from.</param>
        /// <param name="toType">Type to convert to.</param>
        /// <returns>The list of all matching value converters for the given types.</returns>
        public IEnumerable<ValueConverter> GetMatchingConverters(Type fromType, Type toType)
        {
            Tuple<Type, Type> conversion = null;
            bool fromTypeMatches = false, toTypeMatches = false;
            List<ValueConverter> ret = new List<ValueConverter>();
            
            if ((fromType != null || toType != null) && Converters != null)
            {
                foreach (ValueConverter c in Converters)
                {
                    conversion = c.Conversion;

                    toTypeMatches = true;
                    fromTypeMatches = true;

                    if (fromType != null)
                        fromTypeMatches = conversion.Item1.Equals(fromType) || conversion.Item1.IsAssignableFrom(fromType);

                    if (toType != null)
                        toTypeMatches = conversion.Item2.Equals(toType) || conversion.Item2.IsAssignableFrom(toType);

                    if (fromTypeMatches && toTypeMatches)
                        ret.Add(c);
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns property mappings for a given type.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <returns>Property mappings for a given object type.</returns>
        public IDictionary<string, PropertyInfo> GetPropertyMappings<T>()
        {
            return GetPropertyMappings(typeof(T));
        }

        /// <summary>
        /// Returns property mappings for a given type.
        /// </summary>
        /// <param name="t">Object type.</param>
        /// <returns>Property mappings for a given object type.</returns>
        public IDictionary<string, PropertyInfo> GetPropertyMappings(Type t)
        {
            IDictionary<string, PropertyInfo> ret = null;

            if (t != null)
            {
                if (!PropertyMappings.ContainsKey(t))
                    PropertyMappings.Add(t, new Dictionary<string, PropertyInfo>(t.GetProperties().ToDictionary(p => p.Name)));

                if (PropertyMappings.ContainsKey(t))
                    ret = PropertyMappings[t];
            }

            return ret;
        }
    }
}