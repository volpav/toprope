using System;
using System.Collections.Generic;
using System.Linq;

namespace Toprope.Infrastructure.Search.Transformation
{
    /// <summary>
    /// Represents a transformation manager.
    /// </summary>
    public class TransformationManager
    {
        #region Properties

        /// <summary>
        /// Gets or sets object materializers list.
        /// </summary>
        private HashSet<ObjectMaterializer> _materializers = new HashSet<ObjectMaterializer>();

        /// <summary>
        /// Gets object materializers.
        /// </summary>
        public IEnumerable<ObjectMaterializer> Materializers
        {
            get { return _materializers.AsEnumerable(); }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public TransformationManager() { }

        /// <summary>
        /// Registers multiple object materializers.
        /// </summary>
        /// <param name="materializers">Object materializers to register.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="materializers">materializers</paramref> is null.</exception>
        public void RegisterMultipleMaterializers(IEnumerable<ObjectMaterializer> materializers)
        {
            List<ObjectMaterializer> list = null;

            if (materializers == null)
                throw new ArgumentNullException("materializers");
            else
            {
                list = new List<ObjectMaterializer>(materializers.Where(m => m != null));

                for(int i = 0; i < list.Count; i++)
                    RegisterMaterializerInternal(list[i]);
            }
        }

        /// <summary>
        /// Registers a new object materializer.
        /// </summary>
        /// <param name="materializer">Object materializer to register.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="materializer">materializer</paramref> is null.</exception>
        public void RegisterMaterializer(ObjectMaterializer materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException("materializer");
            else
                RegisterMaterializerInternal(materializer);
        }

        /// <summary>
        /// Returns object materializer for a given object type.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <returns>Materializer for a given object type.</returns>
        public ObjectMaterializer GetMaterializerByType<T>()
        {
            return GetMaterializerByType(typeof(T));
        }

        /// <summary>
        /// Returns object materializer for a given object type.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <returns>Materializer for a given object type.</returns>
        public ObjectMaterializer GetMaterializerByType(Type type)
        {
            return type != null ? _materializers.FirstOrDefault(m => m.ResultType != null && m.ResultType.Equals(type)) : null;
        }

        /// <summary>
        /// Registers a new object materializer.
        /// </summary>
        /// <param name="materializer">Object materializer to register.</param>
        private void RegisterMaterializerInternal(ObjectMaterializer materializer)
        {
            if (materializer != null && !_materializers.Contains(materializer))
                _materializers.Add(materializer);
        }
    }
}