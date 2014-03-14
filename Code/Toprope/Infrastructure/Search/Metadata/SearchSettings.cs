using System;
using System.Collections.Generic;

namespace Toprope.Infrastructure.Search.Metadata
{
    /// <summary>
    /// Represents search settings.
    /// </summary>
    public class SearchSettings
    {
        #region Properties

        /// <summary>
        /// Gets the default searchable property types.
        /// </summary>
        public static readonly IEnumerable<Type> DefaultSearchableProperties = new Type[] { 
                typeof(double), 
                typeof(int), 
                typeof(string), 
                typeof(Guid),
                typeof(bool),
                typeof(IEnumerable<string>),
                typeof(Enum)
            };

        /// <summary>
        /// Gets or sets the searchable entity types.
        /// </summary>
        public HashSet<Type> Entities { get; set; }

        /// <summary>
        /// Gets or sets the searchable property types.
        /// </summary>
        public HashSet<Type> SearchableProperties { get; set; }

        /// <summary>
        /// Gets or sets object materializers.
        /// </summary>
        public HashSet<Transformation.ObjectMaterializer> Materializers { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SearchSettings() : this(null) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="entities">Searchable entity types.</param>
        public SearchSettings(IEnumerable<Type> entities)
        {
            Entities = new HashSet<Type>(entities != null ? entities : new Type[] { });

            SearchableProperties = new HashSet<Type>(DefaultSearchableProperties);

            Materializers = new HashSet<Transformation.ObjectMaterializer>();
        }
    }
}