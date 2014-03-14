using System;
using Microsoft.SqlServer.Types;

namespace Toprope.Infrastructure.Storage
{
    /// <summary>
    /// Represents a value converter that allows converting from Location to SqlGeography and vice versa.
    /// </summary>
    public class LocationConverter : ValueConverter
    {
        #region Properties

        /// <summary>
        /// Gets the pair of types between which the conversion is made.
        /// </summary>
        public override Tuple<Type, Type> Conversion
        {
            get { return new Tuple<Type, Type>(typeof(Models.Location), typeof(SqlGeography)); }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public LocationConverter() { }

        /// <summary>
        /// Converts the given value from original representation to alternative one.
        /// </summary>
        /// <param name="value">Value in its original representation.</param>
        /// <returns>Value in its alternative representation.</returns>
        public override object Convert(object value)
        {
            object ret = null;
            Models.Location l = null;
            SqlGeographyBuilder builder = null;

            if (value != null && value is Models.Location)
            {
                l = value as Models.Location;
                builder = new SqlGeographyBuilder();

                builder.SetSrid(4326);

                builder.BeginGeography(OpenGisGeographyType.Point);

                builder.BeginFigure(l.Latitude, l.Longitude);
                builder.EndFigure();

                builder.EndGeography();

                ret = builder.ConstructedGeography;
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
            SqlGeography g = null;

            if (value != null && value is SqlGeography)
            {
                g = value as SqlGeography;
                ret = new Models.Location() { Latitude = g.Lat.Value, Longitude = g.Long.Value };
            }

            return ret;
        }
    }
}