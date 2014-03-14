using System.Collections.Generic;

namespace Toprope.Models
{
    /// <summary>
    /// Represents a year seasons.
    /// </summary>
    [System.Flags]
    public enum Seasons
    {
        /// <summary>
        /// No season specified.
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        /// Autumn.
        /// </summary>
        Autumn = 1,

        /// <summary>
        /// Winter.
        /// </summary>
        Winter = 2,

        /// <summary>
        /// Spring.
        /// </summary>
        Spring = 4,

        /// <summary>
        /// Summer.
        /// </summary>
        Summer = 8
    }

    /// <summary>
    /// Represents a climbing types.
    /// </summary>
    [System.Flags]
    public enum ClimbingTypes
    {
        /// <summary>
        /// No climbing type specified.
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        /// Sport climbing.
        /// </summary>
        Sport = 1,

        /// <summary>
        /// Trad climbing.
        /// </summary>
        Trad = 2,

        /// <summary>
        /// Bouldering.
        /// </summary>
        Bouldering = 4
    }

    /// <summary>
    /// Represents a climbing spot.
    /// </summary>
    public class Place : Infrastructure.Storage.DbRecord
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the place.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the place.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the geographical location of the place.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets the list of tags associated with this place.
        /// </summary>
        public IList<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the seasons where climbing at this place is possible.
        /// </summary>
        public Seasons Season { get; set; }

        /// <summary>
        /// Gets or sets the climbing types for this place.
        /// </summary>
        public ClimbingTypes Climbing { get; set; }

        /// <summary>
        /// Gets or sets the place origin.
        /// </summary>
        public string Origin { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Place()
        {
            Tags = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="copyFrom">Object to copy state from.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="copyFrom">copyFrom</paramref> is null.</exception>
        public Place(Place copyFrom)
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
        public void CopyTo(Place target)
        {
            if (target == null)
                throw new System.ArgumentNullException("target");
            else
            {
                target.Id = this.Id;
                target.Name = this.Name;
                target.Description = this.Description;

                if (this.Location != null)
                    target.Location = new Location(this.Location);
                else
                    target.Location = null;

                if (this.Tags != null)
                    target.Tags = new List<string>(this.Tags);
                else
                    target.Tags = null;

                target.Season = this.Season;
                target.Climbing = this.Climbing;
                target.Origin = this.Origin;
            }
        }

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        /// <returns>A string representation of the given object.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Returns the full place description.
        /// </summary>
        /// <returns>The full place description.</returns>
        public string GetFullDescription()
        {
            string ret = Description ?? string.Empty;

            // Hint: Use _{0}_ to make it italic
            if (!string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Origin))
                ret += string.Format("\n\n{0}", string.Format(Toprope.Resources.Frontend.ContentProvidedBy, Origin.TrimEnd('.')));

            return ret;
        }
    }
}