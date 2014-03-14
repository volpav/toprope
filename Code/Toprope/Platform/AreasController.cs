using System;
using Toprope.Infrastructure.Storage;
using Toprope.Models;

namespace Toprope.Platform
{
    /// <summary>
    /// Represents an areas controller.
    /// </summary>
    public class AreasController
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public AreasController() { }

        /// <summary>
        /// Returns the area by its Id.
        /// </summary>
        /// <param name="areaId">Area Id.</param>
        /// <returns>A area by its Id.</returns>
        public Area GetArea(string areaId)
        {
            Area ret = null;
            Guid id = Infrastructure.Utilities.Input.GetGuid(areaId);

            if (id != Guid.Empty)
            {
                using (Repository repo = new Repository())
                    ret = repo.Select<Area>(id);
            }

            return ret;
        }
    }
}