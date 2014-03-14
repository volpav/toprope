namespace Toprope.Infrastructure
{
    /// <summary>
    /// Represents a current application. This class cannot be inherited.
    /// </summary>
    public sealed class Application
    {
        #region Properties

        private static Application _current = new Application();
        private bool _initialized = false;
        
        /// <summary>
        /// Gets the current instance of the application.
        /// </summary>
        public static Application Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Gets the query cache.
        /// </summary>
        public Utilities.RecencyDictionary<string, object> QueryCache { get; private set; }

        /// <summary>
        /// Gets the current user session.
        /// </summary>
        public ApplicationSession Session
        {
            get { return ApplicationSession.Current; }
        }
        
        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        private Application() { }

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public void Initialize()
        {
            if (!_initialized)
            {
                Search.Specialized.DbSearchEngine.Initialize();
                QueryCache = new Utilities.RecencyDictionary<string, object>(100);

                _initialized = true;
            }
        }
    }
}