namespace Toprope.Models.Search
{
    /// <summary>
    /// Represents sector search result.
    /// </summary>
    public class SectorSearchResult : SearchResultBase
    {
        #region Properties

        /// <summary>
        /// Gets the data item.
        /// </summary>
        public Models.Sector DataItem { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="dataItem">Data item.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="dataItem">dataItem</paramref> is null.</exception>
        public SectorSearchResult(Models.Sector dataItem)
        {
            if (dataItem == null)
                throw new System.ArgumentNullException("dataItem");
            else
            {
                DataItem = dataItem;

                Name = dataItem.Name;
                Excerpt = Infrastructure.Utilities.RichText.Truncate(dataItem.Description).ToString();
                Url = url => { return url.Action("Details", "Sectors", new { id = dataItem.Id.ToString() }); };

                if (string.IsNullOrEmpty(Name))
                    Name = string.Format("({0})", Toprope.Resources.Frontend.NoName);
            }
        }
    }
}