using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Toprope.Infrastructure.Utilities
{
    /// <summary>
    /// Provides methods and properties for manupulating type metadata.
    /// </summary>
    public static class CodeMeta
    {
        #region Properties

        /// <summary>
        /// Gets or sets the inheritors.
        /// </summary>
        private static ConcurrentDictionary<Type, IEnumerable<Type>> Inheritors { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        private static ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> Properties { get; set; }

        #endregion

        /// <summary>
        /// Returns value indicating whether the given type is enumerable.
        /// </summary>
        /// <param name="type">Type to examine.</param>
        /// <returns>Value indicating whether the given type is enumerable.</returns>
        public static bool IsEnumerable(Type type)
        {
            bool ret = false;

            if (type != null && !type.Equals(typeof(string)))
            {
                ret = typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
                
                if (!ret)
                    ret = type.GetInterfaces().Any(i => i.IsGenericType && !i.IsGenericTypeDefinition && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)));
            }

            return ret;
        }

        /// <summary>
        /// Returns the enumerable item type of the given type.
        /// </summary>
        /// <param name="type">Enumerable type.</param>
        /// <returns>Item type.</returns>
        public static Type GetEnumerableType(Type type)
        {
            Type ret = null;
            Type enumerable = null;

            if (type != null && !type.Equals(typeof(string)))
            {
                enumerable = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && !i.IsGenericTypeDefinition && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)));
                
                if (enumerable != null)
                    ret = enumerable.GetGenericArguments().FirstOrDefault();
                else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
                    ret = typeof(object);
            }

            return ret;
        }

        /// <summary>
        /// Finds the given type by name.
        /// </summary>
        /// <param name="name">Type name.</param>
        /// <returns>Type.</returns>
        public static Type FindType(string name)
        {
            Type ret = null;

            if (!string.IsNullOrEmpty(name))
            {
                ret = Type.GetType(name, false, true);
                if (ret == null)
                {
                    if (!name.StartsWith("Toprope.", StringComparison.InvariantCultureIgnoreCase))
                        ret = Type.GetType(string.Format("Toprope.{0}", name), false, true);

                    if (ret == null)
                    {
                        if (name.IndexOf('.') < 0)
                            ret = Type.GetType(string.Format("Toprope.Models.{0}", name), false, true);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns a list of all public properties of a given type.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <returns>A list of all public properties of a given type.</returns>
        public static IEnumerable<PropertyInfo> PropertiesOf<T>()
        {
            return PropertiesOf(typeof(T));
        }

        /// <summary>
        /// Returns a list of all types that inherit from the given type.
        /// </summary>
        /// <param name="type">Base type.</param>
        /// <returns>A list of all types that inherit from the given type.</returns>
        public static IEnumerable<PropertyInfo> PropertiesOf(Type type)
        {
            List<PropertyInfo> props = null;
            IEnumerable<PropertyInfo> ret = null;

            if (type != null)
            {
                if (Properties == null)
                    Properties = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

                if (!Properties.ContainsKey(type))
                {
                    props = new List<PropertyInfo>(type.GetProperties());
                    Properties.AddOrUpdate(type, props, (k, e) => { return props; });
                }

                Properties.TryGetValue(type, out ret);
            }

            if (ret == null)
                ret = new PropertyInfo[] { }.AsEnumerable();

            return ret;
        }

        /// <summary>
        /// Returns a list of all types that inherit from the given type.
        /// </summary>
        /// <typeparam name="T">Base type.</typeparam>
        /// <returns>A list of all types that inherit from the given type.</returns>
        public static IEnumerable<Type> InheritorsOf<T>()
        {
            return InheritorsOf(typeof(T));
        }

        /// <summary>
        /// Returns a list of all types that inherit from the given type.
        /// </summary>
        /// <param name="type">Base type.</param>
        /// <returns>A list of all types that inherit from the given type.</returns>
        public static IEnumerable<Type> InheritorsOf(Type type)
        {
            List<Type> types = null;
            IEnumerable<Type> ret = null;

            if (type != null)
            {
                if (Inheritors == null)
                    Inheritors = new ConcurrentDictionary<Type, IEnumerable<Type>>();

                if (!Inheritors.ContainsKey(type))
                {
                    types = new List<Type>();

                    foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (Type t in asm.GetTypes())
                        {
                            if (t.BaseType != null && t.BaseType.Equals(type))
                                types.Add(t);
                        }
                    }

                    Inheritors.AddOrUpdate(type, types, (k, e) => { return types; });
                }

                Inheritors.TryGetValue(type, out ret);
            }

            if (ret == null)
                ret = new Type[] { }.AsEnumerable();

            return ret;
        }

        /// <summary>
        /// Returns value indicating whether the given type is assignable from another type.
        /// </summary>
        /// <param name="fromType">From type.</param>
        /// <param name="toType">To type.</param>
        /// <returns>Value indicating whether the given type is assignable from another type.</returns>
        public static bool IsAssignableFrom(Type fromType, Type toType)
        {
            bool ret = false;

            if (fromType != null && toType != null)
            {
                ret = fromType.Equals(toType) || fromType.IsAssignableFrom(toType);
                if (!ret)
                {
                    ret = fromType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .Where(m => string.Compare(m.Name, "op_Implicit", System.StringComparison.InvariantCultureIgnoreCase) == 0 &&
                            m.ReturnType.Equals(toType))
                        .Any();
                }
            }

            return ret;
        }
    }
}