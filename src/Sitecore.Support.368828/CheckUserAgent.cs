using Sitecore.Analytics.Configuration;
using Sitecore.Analytics.Pipelines.ExcludeRobots;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Sitecore.Support.Analytics.Pipelines.ExcludeRobots
{
    public class CheckUserAgent : Sitecore.Analytics.Pipelines.ExcludeRobots.CheckUserAgent
    {
        public override void Process(ExcludeRobotsArgs args)
        {
            var argsType = args.GetType();
            PropertyInfo propInfo = argsType.GetProperty("HttpContext", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            HttpContextWrapper HttpContext = propInfo.GetValue(args, null) as System.Web.HttpContextWrapper;
            
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(HttpContext, "HttpContext");
            Assert.IsNotNull(HttpContext.Request, "Request");
            if (HttpContext.Request.UserAgent != null && Sitecore.Support.Analytics.Configuration.AnalyticsSettings.Robots.ExcludeList.ContainsUserAgent(HttpContext.Request.UserAgent))
            {
               args.IsInExcludeList = true;
            }
        }
    }
}