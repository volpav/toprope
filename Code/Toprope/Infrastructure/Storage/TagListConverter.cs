using System;
using System.Linq;
using System.Collections.Generic;

namespace Toprope.Infrastructure.Storage
{
    /// <summary>
    /// Represents a tag list converter.
    /// </summary>
    public class TagListConverter : ValueConverter
    {
        #region Properties

        /// <summary>
        /// Gets the pair of types between which the conversion is made.
        /// </summary>
        public override Tuple<Type, Type> Conversion
        {
            get { return new Tuple<Type, Type>(typeof(IEnumerable<string>), typeof(string)); }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public TagListConverter() { }

        /// <summary>
        /// Converts the given value from original representation to alternative one.
        /// </summary>
        /// <param name="value">Value in its original representation.</param>
        /// <returns>Value in its alternative representation.</returns>
        public override object Convert(object value)
        {
            object ret = null;

            if (value != null && value is IEnumerable<string>)
            {
                ret = string.Join(",", (value as IEnumerable<string>));
                if ((ret as string).Length == 0)
                    ret = null;
            }

            return ret;
        }

        /// <summary>
        /// Converts the given value from alternative representation to original one.
        /// </summary>
        /// <param name="value">Value in its alternative representation.</param>
        /// <returns>Value in its original representation.</returns>
        public override object ConvertBack(object value)
        {
            object ret = null;

            if (value != null && value is string)
                ret = new List<string>((value as string).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => v.Trim().Trim()));

            return ret;
        }
    }
}