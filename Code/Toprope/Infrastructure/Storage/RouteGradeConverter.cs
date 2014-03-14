using System;

namespace Toprope.Infrastructure.Storage
{
    /// <summary>
    /// Represents a route grade converter.
    /// </summary>
    public class RouteGradeConverter : ValueConverter
    {
        #region Properties

        /// <summary>
        /// Gets the pair of types between which the conversion is made.
        /// </summary>
        public override Tuple<Type, Type> Conversion
        {
            get { return new Tuple<Type, Type>(typeof(Models.RouteGrade), typeof(double)); }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public RouteGradeConverter() { }

        /// <summary>
        /// Converts the given value from original representation to alternative one.
        /// </summary>
        /// <param name="value">Value in its original representation.</param>
        /// <returns>Value in its alternative representation.</returns>
        public override object Convert(object value)
        {
            object ret = null;

            if (value != null && value is Models.RouteGrade)
                ret = (value as Models.RouteGrade).Value;

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

            if (value != null && value is double)
                ret = new Models.RouteGrade() { Value = (double)value };

            return ret;
        }
    }
}