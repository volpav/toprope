using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Toprope.Models
{
    /// <summary>
    /// Represents a route grade system.
    /// </summary>
    public enum RouteGradeSystem
    {
        /// <summary>
        /// Use YDS (USA) grade system.
        /// </summary>
        YDS = 0,

        /// <summary>
        /// Use French grade system.
        /// </summary>
        French = 1,

        /// <summary>
        /// Use Hueco grade system (bouldering).
        /// </summary>
        Hueco = 2,

        /// <summary>
        /// Use Ewbank grade system.
        /// </summary>
        Ewbank = 4
    }

    /// <summary>
    /// Represents route difficulty level.
    /// </summary>
    public enum RouteDifficultyLevel
    {
        /// <summary>
        /// Very easy route.
        /// </summary>
        VeryEasy = 0,

        /// <summary>
        /// Easy route.
        /// </summary>
        Easy = 1,

        /// <summary>
        /// Moderate route.
        /// </summary>
        Moderate = 2,

        /// <summary>
        /// Above moderate route.
        /// </summary>
        AboveModerate = 3,

        /// <summary>
        /// Hard route.
        /// </summary>
        Hard = 4,

        /// <summary>
        /// Very hard route.
        /// </summary>
        VeryHard = 5
    }

    /// <summary>
    /// Represents a route grade.
    /// </summary>
    public class RouteGrade
    {
        #region Properties

        private double _value = 50;
        private static object _lock = new object();
        private static IDictionary<double, string> _rawToYDS = null;
        private static IDictionary<double, string> _rawToFrench = null;
        private static IDictionary<double, string> _rawToHueco = null;
        private static IDictionary<double, string> _rawToEwbank = null;
        private ClimbingTypes _parsedClimbing = ClimbingTypes.NotSpecified;

        /// <summary>
        /// Gets the maximum allowed value.
        /// </summary>
        public const double MinValue = 50;

        /// <summary>
        /// Gets the minimum allowed value.
        /// </summary>
        public const double MaxValue = 515.3;

        /// <summary>
        /// Gets the raw value to YDS (USA) grade conversion mappings.
        /// </summary>
        private static IDictionary<double, string> RawToYDS
        {
            get
            {
                if (_rawToYDS == null)
                {
                    lock (_lock)
                    {
                        if (_rawToYDS == null)
                        {
                            _rawToYDS = new Dictionary<double, string>()
                            {
                                { 50, "5.0" }, { 51, "5.1" }, { 52, "5.2" }, { 53, "5.3" },
                                { 54, "5.4" }, { 55, "5.5" }, { 56, "5.6" }, { 57, "5.7" },
                                { 58, "5.8" }, { 59, "5.9" }, { 510.1, "5.10a" }, { 510.2, "5.10b" },
                                { 510.3, "5.10c" }, { 510.4, "5.10d" }, { 511.1, "5.11a" }, { 511.2, "5.11b" },
                                { 511.3, "5.11c" }, { 511.4, "5.11d" }, { 512.1, "5.12a" }, { 512.2, "5.12b" },
                                { 512.3, "5.12c" }, { 512.4, "5.12d" }, { 513.1, "5.13a" }, { 513.2, "5.13b" },
                                { 513.3, "5.13c" }, { 513.4, "5.13d" }, { 514.1, "5.14a" }, { 514.2, "5.14b" },
                                { 514.3, "5.14c" }, { 514.4, "5.14d" }, { 515.1, "5.15a" }, { 515.2, "5.15b" },
                                { 515.3, "5.15c" }, { 515.4, "5.15d" }, { 516.1, "5.16a" }
                            };
                        }
                    }
                }

                return _rawToYDS;
            }
        }

        /// <summary>
        /// Gets the raw value to French grade conversion mappings.
        /// </summary>
        private static IDictionary<double, string> RawToFrench
        {
            get
            {
                if (_rawToFrench == null)
                {
                    lock (_lock)
                    {
                        if (_rawToFrench == null)
                        {
                            _rawToFrench = new Dictionary<double, string>()
                            {
                                { 50, "1" }, { 51, "1" }, { 52, "2" }, { 53, "3" },
                                { 54, "4a" }, { 55, "4b" }, { 56, "4c" }, { 57, "5a" },
                                { 58, "5b" }, { 59, "5c" }, { 510.1, "6a" }, { 510.2, "6a+" },
                                { 510.3, "6b" }, { 510.4, "6b+" }, { 511.1, "6c" }, { 511.2, "6c" },
                                { 511.3, "6c+" }, { 511.4, "7a" }, { 512.1, "7a+" }, { 512.2, "7b" },
                                { 512.3, "7b+" }, { 512.4, "7c" }, { 513.1, "7c+" }, { 513.2, "8a" },
                                { 513.3, "8a+" }, { 513.4, "8b" }, { 514.1, "8b+" }, { 514.2, "8c" },
                                { 514.3, "8c+" }, { 514.4, "9a" }, { 515.1, "9a+" }, { 515.2, "9b" },
                                { 515.3, "9b+" }, { 515.4, "9c" }, { 516.1, "9c+" }
                            };
                        }
                    }
                }

                return _rawToFrench;
            }
        }

        /// <summary>
        /// Gets the raw value to Hueco grade conversion mappings.
        /// </summary>
        private static IDictionary<double, string> RawToHueco
        {
            get
            {
                if (_rawToHueco == null)
                {
                    lock (_lock)
                    {
                        if (_rawToHueco == null)
                        {
                            _rawToHueco = new Dictionary<double, string>()
                            {
                                { 54, "V0" }, { 55, "V0+" }, { 57, "V1" }, { 58, "V2" }, 
                                { 510.1, "V3" }, { 510.2, "V3" }, { 510.3, "V4" }, { 510.4, "V4" }, 
                                { 511.1, "V5" }, { 511.3, "V5" }, { 511.4, "V6" }, { 512.1, "V7" }, 
                                { 512.2, "V8" }, { 512.3, "V8" }, { 512.4, "V9" }, { 513.1, "V10" }, 
                                { 513.2, "V11" }, { 513.3, "V12" }, { 513.4, "V13" }, { 514.1, "V14" }, 
                                { 514.2, "V15" }, { 514.3, "V16" }
                            };
                        }
                    }
                }

                return _rawToHueco;
            }
        }

        /// <summary>
        /// Gets the raw value to Ewbank grade conversion mappings.
        /// </summary>
        private static IDictionary<double, string> RawToEwbank
        {
            get
            {
                if (_rawToEwbank == null)
                {
                    lock (_lock)
                    {
                        if (_rawToEwbank == null)
                        {
                            _rawToEwbank = new Dictionary<double, string>()
                            {
                                { 52, "4" }, { 53, "5" }, { 54, "8" }, { 55, "13" }, { 56, "14" }, { 57, "15" },
                                { 58, "16" }, { 59, "17" }, { 510.1, "18" }, { 510.2, "19" },
                                { 510.3, "20" }, { 510.4, "21" }, { 511.1, "22" }, { 511.2, "23" },
                                { 511.3, "24" }, { 511.4, "24" }, { 512.1, "25" }, { 512.2, "26" },
                                { 512.3, "27" }, { 512.4, "28" }, { 513.1, "29" }, { 513.2, "29" },
                                { 513.3, "30" }, { 513.4, "31" }, { 514.1, "32" }, { 514.2, "33" },
                                { 514.3, "34" }, { 514.4, "35" }, { 515.1, "36" }, { 515.2, "37" },
                                { 515.3, "38" }, { 515.4, "39" }, { 516.1, "40" }
                            };
                        }
                    }
                }

                return _rawToEwbank;
            }
        }

        /// <summary>
        /// Gets or sets the raw value of the route grade.
        /// </summary>
        public double Value
        {
            get { return _value; }
            set { _value = RouteGrade.ValidateValue(value); }
        }

        /// <summary>
        /// Gets the route difficulty level.
        /// </summary>
        public RouteDifficultyLevel DifficultyLevel
        {
            get
            {
                RouteDifficultyLevel ret = RouteDifficultyLevel.VeryEasy;

                if (Value <= 57)
                    ret = RouteDifficultyLevel.VeryEasy;
                else if (Value < 511)
                    ret = RouteDifficultyLevel.Easy;
                else if (Value < 512.2)
                    ret = RouteDifficultyLevel.Moderate;
                else if (Value < 513.2)
                    ret = RouteDifficultyLevel.AboveModerate;
                else if (Value < 514.2)
                    ret = RouteDifficultyLevel.Hard;
                else
                    ret = RouteDifficultyLevel.VeryHard;

                return ret;
            }
        }

        /// <summary>
        /// Gets the climbing type determined when the route grade was parsed.
        /// </summary>
        public ClimbingTypes ParsedClimbing
        {
            get { return _parsedClimbing; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public RouteGrade() { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="value">The raw value of the route grade.</param>
        public RouteGrade(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="copyFrom">Object to copy state from.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="copyFrom">copyFrom</paramref> is null.</exception>
        public RouteGrade(RouteGrade copyFrom)
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
        public void CopyTo(RouteGrade target)
        {
            if (target == null)
                throw new System.ArgumentNullException("target");
            else
            {
                target.Value = this.Value;
                target._parsedClimbing = this._parsedClimbing;
            }
        }

        /// <summary>
        /// Returns the YDS (USA) grade system representation of the current route grade.
        /// </summary>
        /// <returns>The YDS (USA) grade system representation of the current route grade.</returns>
        public string ToYDS()
        {
            return RawToYDS.ContainsKey(Value) ? RawToYDS[Value] : "-";
        }

        /// <summary>
        /// Returns the French grade system representation of the current route grade.
        /// </summary>
        /// <returns>The French grade system representation of the current route grade.</returns>
        public string ToFrench()
        {
            return RawToFrench.ContainsKey(Value) ? RawToFrench[Value] : "-";
        }

        /// <summary>
        /// Returns the Hueco grade system representation of the current route grade.
        /// </summary>
        /// <returns>The Hueco grade system representation of the current route grade.</returns>
        public string ToHueco()
        {
            return RawToHueco.ContainsKey(Value) ? RawToHueco[Value] : "-";
        }

        /// <summary>
        /// Returns the Ewbank grade system representation of the current route grade.
        /// </summary>
        /// <returns>The Ewbank grade system representation of the current route grade.</returns>
        public string ToEwbank()
        {
            return RawToEwbank.ContainsKey(Value) ? RawToEwbank[Value] : "-";
        }

        /// <summary>
        /// Returns the specified grade system representation of the current route grade.
        /// </summary>
        /// <param name="gradeSystem">Grade system to apply.</param>
        /// <returns>The specified grade system representation of the current route grade.</returns>
        public string ToGrade(RouteGradeSystem gradeSystem)
        {
            return ToGrade(null, gradeSystem);
        }

        /// <summary>
        /// Returns the specified grade system representation of the current route grade.
        /// </summary>
        /// <param name="route">Owning route.</param>
        /// <param name="gradeSystem">Grade system to apply.</param>
        /// <returns>The specified grade system representation of the current route grade.</returns>
        public string ToGrade(Route route, RouteGradeSystem gradeSystem)
        {
            string ret = string.Empty;

            if (route != null)
            {
                if (route.Climbing.HasFlag(ClimbingTypes.Bouldering))
                    ret = ToHueco();
            }

            if (string.IsNullOrEmpty(ret))
            {
                switch (gradeSystem)
                {
                    case RouteGradeSystem.YDS:
                        ret = ToYDS();
                        break;
                    case RouteGradeSystem.French:
                        ret = ToFrench();
                        break;
                    case RouteGradeSystem.Hueco:
                        ret = ToHueco();
                        break;
                    case RouteGradeSystem.Ewbank:
                        ret = ToEwbank();
                        break;
                }
            }

            if (string.IsNullOrEmpty(ret))
                ret = "-";

            return ret;
        }

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        /// <returns>A string representation of the given object.</returns>
        public override string ToString()
        {
            return ToFrench();
        }

        #region Static methods

        /// <summary>
        /// Validates the given raw value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <returns>Validated value.</returns>
        internal static double ValidateValue(double value)
        {
            double ret = value;
            int fractionalPart = 0;

            if (ret < MinValue) ret = MinValue; // 5.0
            else if (ret > 59 && ret < 510) ret = 59; // 5.9
            else if (ret > MaxValue) ret = MaxValue;
            else
            {
                fractionalPart = (int)(ret - Math.Floor(ret));

                if (ret <= 59 && fractionalPart > 0) // E.g. 5.9c is not allowed, there are no letters
                    ret = Math.Floor(ret);
                else if (fractionalPart != 0 && fractionalPart > 4) // E.g. 5.12d is allowed, but 5.12e is not
                    ret = Math.Floor(ret) + .1;
            }

            return ret;
        }

        /// <summary>
        /// Parses  the route grade from the given YDS representation.
        /// </summary>
        /// <param name="grade">Route grade in YDS representation.</param>
        /// <returns>Route grade.</returns>
        public static RouteGrade FromYDS(string grade)
        {
            Match m = null;
            double value = 0;
            RouteGrade ret = null;
            
            if (!string.IsNullOrEmpty(grade))
            {
                grade = grade.Trim().ToLowerInvariant();

                if (grade.Length > 5)
                {
                    m = Regex.Match(grade, "5\\.[0-9]{1,2}(a|b|c|d)?", RegexOptions.IgnoreCase);
                    
                    if (m != null && m.Success)
                        grade = m.Value;
                }

                foreach (double key in RawToYDS.Keys.OrderBy(k => k))
                {
                    if (string.Compare(RawToYDS[key], grade, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        value = key;
                        break;
                    }
                }
            }

            if (value > 0)
                ret = new RouteGrade() { Value = value, _parsedClimbing = ClimbingTypes.Sport };

            return ret;
        }

        /// <summary>
        /// Parses  the route grade from the given French representation.
        /// </summary>
        /// <param name="grade">Route grade in French representation.</param>
        /// <returns>Route grade.</returns>
        public static RouteGrade FromFrench(string grade)
        {
            Match m = null;
            double value = 0;
            RouteGrade ret = null;

            if (!string.IsNullOrEmpty(grade))
            {
                grade = grade.Trim().ToLowerInvariant();

                if (grade.Length > 3)
                {
                    m = Regex.Match(grade, "[1-9](a|b|c)?(\\+)?", RegexOptions.IgnoreCase);
                    
                    if (m != null && m.Success)
                        grade = m.Value;
                }

                foreach (double key in RawToFrench.Keys.OrderBy(k => k))
                {
                    if (string.Compare(RawToFrench[key], grade, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        value = key;
                        break;
                    }
                }
            }

            if (value > 0)
                ret = new RouteGrade() { Value = value, _parsedClimbing = ClimbingTypes.Sport };

            return ret;
        }

        /// <summary>
        /// Parses the route grade from the given Hueco representation.
        /// </summary>
        /// <param name="grade">Route grade in Hueco representation.</param>
        /// <returns>Route grade.</returns>
        public static RouteGrade FromHueco(string grade)
        {
            Match m = null;
            double value = 0;
            RouteGrade ret = null;

            if (!string.IsNullOrEmpty(grade))
            {
                grade = grade.Trim().ToLowerInvariant();

                if (grade.Length > 3)
                {
                    m = Regex.Match(grade, "V[0-9]{1,2}\\+?", RegexOptions.IgnoreCase);

                    if (m != null && m.Success)
                        grade = m.Value;
                }

                foreach (double key in RawToHueco.Keys.OrderBy(k => k))
                {
                    if (string.Compare(RawToHueco[key], grade, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        value = key;
                        break;
                    }
                }
            }

            if (value > 0)
                ret = new RouteGrade() { Value = value, _parsedClimbing = ClimbingTypes.Bouldering };

            return ret;
        }

        /// <summary>
        /// Parses the route grade from the given Ewbank representation.
        /// </summary>
        /// <param name="grade">Route grade in Ewbank representation.</param>
        /// <returns>Route grade.</returns>
        public static RouteGrade FromEwbank(string grade)
        {
            Match m = null;
            double value = 0;
            RouteGrade ret = null;

            if (!string.IsNullOrEmpty(grade))
            {
                grade = grade.Trim().ToLowerInvariant();

                if (grade.Length > 3)
                {
                    m = Regex.Match(grade, "[0-9]+?", RegexOptions.IgnoreCase);

                    if (m != null && m.Success)
                        grade = m.Value;
                }

                foreach (double key in RawToEwbank.Keys.OrderBy(k => k))
                {
                    if (string.Compare(RawToEwbank[key], grade, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        value = key;
                        break;
                    }
                }
            }

            if (value > 0)
                ret = new RouteGrade() { Value = value, _parsedClimbing = ClimbingTypes.Sport };

            return ret;
        }

        /// <summary>
        /// Parses  the route grade from the specified representation.
        /// </summary>
        /// <param name="grade">Route grade in specified representation.</param>
        /// <param name="gradeSystem">Grade system the value is expressed in.</param>
        /// <returns>Route grade.</returns>
        public static RouteGrade FromGrade(string grade, RouteGradeSystem gradeSystem)
        {
            RouteGrade ret = null;

            switch (gradeSystem)
            {
                case RouteGradeSystem.YDS:
                    ret = RouteGrade.FromYDS(grade);
                    break;
                case RouteGradeSystem.French:
                    ret = RouteGrade.FromFrench(grade);
                    break;
                case RouteGradeSystem.Hueco:
                    ret = RouteGrade.FromHueco(grade);
                    break;
                case RouteGradeSystem.Ewbank:
                    ret = RouteGrade.FromEwbank(grade);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Parses route grade from its string representation.
        /// </summary>
        /// <param name="grade">Route grade.</param>
        /// <returns>Route grade object.</returns>
        public static RouteGrade Parse(string grade)
        {
            RouteGrade ret = FromYDS(grade);

            if (ret == null)
            {
                ret = FromFrench(grade);

                if (ret == null)
                {
                    ret = FromHueco(grade);

                    if (ret == null)
                        ret = FromEwbank(grade);
                }
            }

            return ret;
        }

        /// <summary>
        /// Implicitly converts the given grade to its raw representation.
        /// </summary>
        /// <param name="grade">Grade instance.</param>
        /// <returns>Grade's raw representation.</returns>
        public static implicit operator double(RouteGrade grade)
        {
            return grade != null ? grade.Value : 0;
        }

        /// <summary>
        /// Explicitly converts the given raw value to its RouteGrade representation.
        /// </summary>
        /// <param name="value">Raw value.</param>
        /// <returns>RouteGrade instance.</returns>
        public static explicit operator RouteGrade(double value)
        {
            return new RouteGrade() { Value = value };
        }

        #endregion
    }
}