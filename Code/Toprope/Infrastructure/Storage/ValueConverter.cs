using System;

namespace Toprope.Infrastructure.Storage
{
    /// <summary>
    /// Represents a value converter.
    /// </summary>
    public abstract class ValueConverter
    {
        #region Properties

        /// <summary>
        /// Gets the pair of types between which the conversion is made.
        /// </summary>
        public abstract Tuple<Type, Type> Conversion { get; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected ValueConverter() { }

        /// <summary>
        /// Converts the given value from original representation to alternative one.
        /// </summary>
        /// <param name="value">Value in its original representation.</param>
        /// <returns>Value in its alternative representation.</returns>
        public abstract object Convert(object value);

        /// <summary>
        /// Converts the given value from alternative representation to original one.
        /// </summary>
        /// <param name="value">Value in its alternative representation.</param>
        /// <returns>Value in its original representation.</returns>
        public abstract object ConvertBack(object value);
    }
}