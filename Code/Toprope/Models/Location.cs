namespace Toprope.Models
{
    /// <summary>
    /// Represents a geographical point on the map.
    /// </summary>
    public class Location
    {
        #region Properties

        /// <summary>
        /// Gets or sets the latitude component.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude component.
        /// </summary>
        public double Longitude { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Location() : this(0, 0) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="latitude">Latitude.</param>
        /// <param name="longitude">Longitude.</param>
        public Location(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="copyFrom">Object to copy state from.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="copyFrom">copyFrom</paramref> is null.</exception>
        public Location(Location copyFrom)
        {
            if (copyFrom == null)
                throw new System.ArgumentNullException("copyFrom");
            else
                copyFrom.CopyTo(this);
        }

        /// <summary>
        /// Copies the state of the current object into the given one.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="target">target</paramref> is null.</exception>
        public void CopyTo(Location target)
        {
            if (target == null)
                throw new System.ArgumentNullException("target");
            else
            {
                target.Latitude = this.Latitude;
                target.Longitude = this.Longitude;
            }
        }

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        /// <returns>A string representation of the given object.</returns>
        public override string ToString()
        {
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");

            return string.Format("{0},{1}", Latitude.ToString(culture), Longitude.ToString(culture));
        }

        #region Static methods

        /// <summary>
        /// Parses the location from the given input string.
        /// </summary>
        /// <param name="location">Input string representing a location.</param>
        /// <returns>Parsed location.</returns>
        public static Location Parse(string location)
        {
            Location ret = null;
            string[] components = null;

            if (!string.IsNullOrEmpty(location))
            {
                components = location.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (components != null && components.Length == 2)
                {
                    ret = new Location();

                    ret.Latitude = Infrastructure.Utilities.Input.GetDouble(components[0]);
                    ret.Longitude = Infrastructure.Utilities.Input.GetDouble(components[1]);

                    if (ret.Latitude == 0 && ret.Longitude == 0)
                        ret = null;
                }
            }

            return ret;
        }

        #endregion
    }
}