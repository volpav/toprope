using System.Collections.Generic;

namespace Toprope.Models
{
    /// <summary>
    /// Represents a route.
    /// </summary>
    public class Route : Infrastructure.Storage.DbRecord
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Id of the sector to which this route belongs.
        /// </summary>
        public System.Guid SectorId { get; set; }

        /// <summary>
        /// Gets or sets the name of the route.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this route.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the grade of this route (the level of difficulty).
        /// </summary>
        public RouteGrade Grade { get; set; }

        /// <summary>
        /// Gets or sets the climbing types for this route.
        /// </summary>
        public ClimbingTypes Climbing { get; set; }

        /// <summary>
        /// Gets or sets the 1-based order of the route in a given sector.
        /// </summary>
        public int Order { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Route() { Climbing = ClimbingTypes.Sport; }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="copyFrom">Object to copy state from.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="copyFrom">copyFrom</paramref> is null.</exception>
        public Route(Route copyFrom)
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
        public void CopyTo(Route target)
        {
            if (target == null)
                throw new System.ArgumentNullException("target");
            else
            {
                target.Id = this.Id;
                target.SectorId = this.SectorId;
                target.Name = this.Name;
                target.Description = this.Description;
                target.Climbing = this.Climbing;

                if (this.Grade != null)
                    target.Grade = new RouteGrade(this.Grade);
                else
                    target.Grade = null;

                target.Order = this.Order;
            }
        }

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        /// <returns>A string representation of the given object.</returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Grade != null ? Grade.ToString() : "-");
        }

        #region Static methods

        /// <summary>
        /// Categorizes the given routes.
        /// </summary>
        /// <param name="routes">Routes to categorize.</param>
        /// <returns>Categorized routes.</returns>
        public static IDictionary<RouteDifficultyLevel, IList<Route>> Categorize(IEnumerable<Route> routes)
        {
            Dictionary<RouteDifficultyLevel, IList<Route>> ret = new Dictionary<RouteDifficultyLevel, IList<Route>>();

            if (routes != null)
            {
                foreach (Route r in routes)
                {
                    if (r != null && r.Grade != null)
                    {
                        if (!ret.ContainsKey(r.Grade.DifficultyLevel))
                            ret.Add(r.Grade.DifficultyLevel, new List<Route>());

                        ret[r.Grade.DifficultyLevel].Add(r);
                    }
                }
            }

            return ret;
        }

        #endregion
    }
}