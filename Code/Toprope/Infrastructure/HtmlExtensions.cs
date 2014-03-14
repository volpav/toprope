using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Toprope.Infrastructure
{
    /// <summary>
    /// Represents an HTML control size.
    /// </summary>
    public enum HtmlControlSize
    {
        /// <summary>
        /// Auto size.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Normal size.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Small size.
        /// </summary>
        Small = 2,

        /// <summary>
        /// Large size.
        /// </summary>
        Large = 3,

        /// <summary>
        /// Tiny size.
        /// </summary>
        Tiny = 4
    }

    /// <summary>
    /// Provides various HTML extensions.
    /// </summary>
    public static class HtmlExtensions
    {
        /// <summary>
        /// Renders difficulty chart.
        /// </summary>
        /// <param name="helper">HTML helper.</param>
        /// <param name="difficulty">Difficulty distribution.</param>
        /// <returns>Difficulty chart.</returns>
        public static IHtmlString RenderDifficultyChart(this HtmlHelper helper, IDictionary<Models.RouteDifficultyLevel, IList<Models.Route>> difficulty)
        {
            return RenderDifficultyChart(helper, difficulty, HtmlControlSize.Auto);
        }

        /// <summary>
        /// Renders difficulty chart.
        /// </summary>
        /// <param name="helper">HTML helper.</param>
        /// <param name="difficulty">Difficulty distribution.</param>
        /// <param name="size">Control size.</param>
        /// <returns>Difficulty chart.</returns>
        public static IHtmlString RenderDifficultyChart(this HtmlHelper helper, IDictionary<Models.RouteDifficultyLevel, IList<Models.Route>> difficulty, HtmlControlSize size)
        {
            string difficultyClass = string.Empty;
            int maxHeight = 38, categoryHeight = 0, max = 0, percentage = 0;
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            Models.RouteDifficultyLevel level = Models.RouteDifficultyLevel.VeryEasy;
            Dictionary<Models.RouteDifficultyLevel, int> heights = new Dictionary<Models.RouteDifficultyLevel, int>();

            if (size == HtmlControlSize.Small)
                maxHeight = 19;
            else if (size == HtmlControlSize.Tiny)
                maxHeight = 16;

            if (difficulty != null)
            {
                output.AppendFormat("<div class=\"route-difficulty-chart clear{0}\">", size != HtmlControlSize.Auto ? string.Format(" size-{0}", Enum.GetName(typeof(HtmlControlSize), size).ToLowerInvariant()) : string.Empty);
                output.Append("<div class=\"route-difficulty-baseline\"></div>");

                output.AppendFormat("<label class=\"route-difficulty-start\">{0}</label>", Toprope.Resources.Frontend.Easy);
                output.AppendFormat("<label class=\"route-difficulty-middle\">{0}</label>", Toprope.Resources.Frontend.Moderate);
                output.AppendFormat("<label class=\"route-difficulty-end\">{0}</label>", Toprope.Resources.Frontend.Hard);

                foreach (Models.RouteDifficultyLevel l in difficulty.Keys)
                {
                    heights.Add(l, difficulty[l].Count);
                    max += difficulty[l].Count;
                }

                if (max > 0)
                {
                    foreach (Models.RouteDifficultyLevel l in heights.Keys.ToArray())
                    {
                        percentage = (int)Math.Floor((double)(100 * heights[l] / max));
                        heights[l] = (int)Math.Floor((double)(maxHeight * percentage / 100));
                    }
                }

                foreach (int i in Enum.GetValues(typeof(Models.RouteDifficultyLevel)).Cast<int>().OrderBy(v => v).ToArray())
                {
                    if (Enum.TryParse<Models.RouteDifficultyLevel>(i.ToString(), out level))
                    {
                        categoryHeight = 0;

                        if (heights.ContainsKey(level))
                            categoryHeight = heights[level];
                        
                        switch (level)
                        {
                            case Models.RouteDifficultyLevel.VeryEasy:
                                difficultyClass = "route-difficulty-ve";
                                break;
                            case Models.RouteDifficultyLevel.Easy:
                                difficultyClass =  "route-difficulty-e";
                                break;
                            case Models.RouteDifficultyLevel.Moderate:
                                difficultyClass = "route-difficulty-m";
                                break;
                            case Models.RouteDifficultyLevel.AboveModerate:
                                difficultyClass = "route-difficulty-am";
                                break;
                            case Models.RouteDifficultyLevel.Hard:
                                difficultyClass = "route-difficulty-h";
                                break;
                            case Models.RouteDifficultyLevel.VeryHard:
                                difficultyClass = "route-difficulty-vh";
                                break;
                        }

                        if (categoryHeight == 0 && difficulty.ContainsKey(level) && difficulty[level].Count > 0)
                            categoryHeight = 1;

                        output.AppendFormat("<div class=\"route-difficulty {0}\" style=\"height: {1}px\"><span>{2}</span></div>",
                            difficultyClass, categoryHeight, difficulty.ContainsKey(level) ? difficulty[level].Count.ToString() : string.Empty);
                    }
                    else
                        break;
                }

                output.Append("</div>");
            }

            return new MvcHtmlString(output.ToString());
        }

        /// <summary>
        /// Renders route grade.
        /// </summary>
        /// <param name="helper">HTML helper.</param>
        /// <param name="route">Route.</param>
        /// <param name="grade">Route grade.</param>
        /// <returns>Rendered route grade.</returns>
        public static IHtmlString RenderRouteGrade(this HtmlHelper helper, Models.Route route, Models.RouteGrade grade)
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();

            if (grade != null)
            {
                output.Append("<span class=\"route-grade ");
                switch (grade.DifficultyLevel)
                {
                    case Models.RouteDifficultyLevel.VeryEasy:
                        output.Append("route-grade-ve");
                        break;
                    case Models.RouteDifficultyLevel.Easy:
                        output.Append("route-grade-e");
                        break;
                    case Models.RouteDifficultyLevel.Moderate:
                        output.Append("route-grade-m");
                        break;
                    case Models.RouteDifficultyLevel.AboveModerate:
                        output.Append("route-grade-am");
                        break;
                    case Models.RouteDifficultyLevel.Hard:
                        output.Append("route-grade-h");
                        break;
                    case Models.RouteDifficultyLevel.VeryHard:
                        output.Append("route-grade-vh");
                        break;
                }

                output.AppendFormat("\">{0}</span>", grade.ToGrade(route, Application.Current.Session.RegionalSettings.Grades));
            }

            return new MvcHtmlString(output.ToString());
        }
    }
}