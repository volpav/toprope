using System;
using System.Linq;
using System.Collections.Generic;

namespace Toprope.Infrastructure.Mail
{
    /// <summary>
    /// Represents a base class for logging into the file.
    /// </summary>
    public abstract class FileLoggerBase : Logger
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected FileLoggerBase() { }

        /// <summary>
        /// Logs all entries.
        /// </summary>
        /// <param name="entries">Entries to log.</param>
        /// <param name="path">Physical path to the log file.</param>
        protected virtual void Log(IEnumerable<LogEntry> entries, string path)
        {
            System.IO.FileStream stream = null;

            if (!string.IsNullOrEmpty(path) && entries != null && entries.Any(e => e != null))
            {
                InvokeOperation(() =>
                {
                    string directory = System.IO.Path.GetDirectoryName(path);

                    if (!System.IO.Directory.Exists(directory))
                        System.IO.Directory.CreateDirectory(directory);

                    if (!System.IO.File.Exists(path))
                        stream = System.IO.File.Create(path);
                    else
                        stream = new System.IO.FileStream(path, System.IO.FileMode.Append);
                });

                if (stream != null)
                {
                    InvokeOperation(() =>
                    {
                        using (stream)
                        {
                            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(stream))
                            {
                                foreach (LogEntry l in entries.Where(e => e != null))
                                    l.WriteTo(writer);
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Invokes the given operation and ignores any unhandled exceptions.
        /// </summary>
        /// <param name="operation">Opration to invoke.</param>
        protected void InvokeOperation(System.Action operation)
        {
            if (operation != null)
            {
                try
                {
                    operation();
                }
                catch (Exception) { }
            }
        }
    }
}
