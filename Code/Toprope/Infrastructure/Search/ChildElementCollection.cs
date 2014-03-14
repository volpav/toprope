using System;
using System.Collections.Specialized;
using System.Linq;

namespace Toprope.Infrastructure.Search
{
    /// <summary>
    /// Represents a collection of child elements.
    /// </summary>
    /// <typeparam name="TElement">Element type.</typeparam>
    /// <typeparam name="TParent">Parent type.</typeparam>
    public class ChildElementCollection<TElement, TParent> : System.Collections.ObjectModel.ObservableCollection<TElement> where TElement : IChildElement<TParent>
    {
        #region Properties

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        protected TParent Parent { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// <param name="parent">Parent element.</param>
        /// </summary>
        /// <exception cref="System.ArgumentNullException"><paramref name="parent">parent</paramref> is null.</exception>
        public ChildElementCollection(TParent parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            else
            {
                this.Parent = parent;
                this.CollectionChanged += Collection_Changed;
            }
        }

        /// <summary>
        /// Handles collection "Changed" event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void Collection_Changed(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action != NotifyCollectionChangedAction.Reset)
            {
                if (args.Action == NotifyCollectionChangedAction.Replace || args.Action == NotifyCollectionChangedAction.Remove)
                {
                    if (args.OldItems != null)
                    {
                        foreach (IChildElement<TParent> item in args.OldItems.OfType<IChildElement<TParent>>())
                        {
                            if (item != null)
                                item.Parent = default(TParent);
                        }
                    }
                }

                if (args.NewItems != null)
                {
                    foreach (IChildElement<TParent> item in args.NewItems.OfType<IChildElement<TParent>>())
                    {
                        if (item != null)
                            item.Parent = this.Parent;
                    }
                }
            }
        }
    }
}