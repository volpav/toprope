using System;
using System.Linq;
using System.Collections.Generic;

namespace Toprope.Infrastructure.Mail
{
    /// <summary>
    /// Represents a log organize mode.
    /// </summary>
    public enum LogOrganizeMode
    {
        /// <summary>
        /// New log file is creatd daily.
        /// </summary>
        Daily = 0,

        /// <summary>
        /// New log file is created weekly.
        /// </summary>
        Weekly = 1,

        /// <summary>
        /// New log file is creatd monthly.
        /// </summary>
        Monthly = 2,

        /// <summary>
        /// New log file is created yearly.
        /// </summary>
        Yearly = 3
    }

    /// <summary>
    /// Represents a multi text file logger.
    /// </summary>
    public class MultiTextFileLogger : FileLoggerBase
    {
        #region Properties

        /// <summary>
        /// Gets the organize mode.
        /// </summary>
        public LogOrganizeMode Mode { get; private set; }

        /// <summary>
        /// Gets the physical path to the base directory for log files.
        /// </summary>
        public string BaseDirectory { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="baseDirectory">The physical path to the base directory for log files.</param>
        public MultiTextFileLogger(string baseDirectory) : this(baseDirectory, LogOrganizeMode.Daily) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="baseDirectory">The physical path to the base directory for log files.</param>
        /// <param name="mode">Organize mode.</param>
        public MultiTextFileLogger(string baseDirectory, LogOrganizeMode mode)
        {
            this.BaseDirectory = baseDirectory;
            this.Mode = mode;
        }

        /// <summary>
        /// Logs all entries.
        /// </summary>
        /// <param name="entries">Entries to log.</param>
        public override void Log(IEnumerable<LogEntry> entries)
        {
            string path = string.Empty;
            DateTime date = DateTime.Now;
            System.Globalization.CultureInfo culture = null;
            System.Globalization.DateTimeFormatInfo formatInfo = null;

            culture = new System.Globalization.CultureInfo("en-US");
            formatInfo = culture.DateTimeFormat;

            if (entries != null && entries.Any(e => e != null))
            {
                if (!string.IsNullOrEmpty(BaseDirectory))
                {
                    switch (Mode)
                    {
                        case LogOrganizeMode.Daily:
                            path = System.IO.Path.Combine(BaseDirectory, string.Format("{0}.txt", date.ToString("dd-MM-yyyy", culture)));
                            break;
                        case LogOrganizeMode.Weekly:
                            path = System.IO.Path.Combine(BaseDirectory, string.Format("Week {0}, {1}.txt", 
                                formatInfo.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday), date.ToString("yyyy", culture)));
                            break;
                        case LogOrganizeMode.Monthly:
                            path = System.IO.Path.Combine(BaseDirectory, string.Format("{0}, {1}.txt", date.ToString("MMMM", culture), date.ToString("yyyy", culture)));
                            break;
                        case LogOrganizeMode.Yearly:
                            path = System.IO.Path.Combine(BaseDirectory, string.Format("{0}.txt", date.ToString("yyyy", culture)));
                            break;
                    }

                    base.Log(entries, path);
                }
            }
        }
    }
}
