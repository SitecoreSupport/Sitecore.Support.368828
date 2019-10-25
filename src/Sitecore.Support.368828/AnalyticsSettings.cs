using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Web.IPAddresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

namespace Sitecore.Support.Analytics.Configuration
{
    public static class AnalyticsSettings
    {
        public static class Robots
        {
            private static ExcludeList excludeList;

            private static object excludeListSync = new object();

            public static bool AutoDetect => Settings.GetBoolSetting("Analytics.AutoDetectBots", defaultValue: true);

            public static ExcludeList ExcludeList
            {
                get
                {
                    if (excludeList == null)
                    {
                        lock (excludeListSync)
                        {
                            if (excludeList == null)
                            {
                                excludeList = ExcludeList.GeExcludeList();
                            }
                        }
                    }
                    return excludeList;
                }
                internal set
                {
                    excludeList = value;
                }
            }

            public static bool IgnoreRobots => Settings.GetBoolSetting("Analytics.Robots.IgnoreRobots", defaultValue: true);

            public static int SessionTimeout => Settings.GetIntSetting("Analytics.Robots.SessionTimeout", 1);
        }
        public class ExcludeList
        {
            private HashSet<string> userAgents;

            private IPList ipaddresses;

            public ExcludeList(IPList ipaddresses, IEnumerable<string> userAgents)
                : this(ipaddresses, new HashSet<string>(userAgents))
            {
            }

            public ExcludeList(IPList ipaddresses, HashSet<string> userAgents)
            {
                this.ipaddresses = ipaddresses;
                this.userAgents = userAgents;
            }

            public static ExcludeList GeExcludeList()
            {
                IPList iPAddresses = GetIPAddresses();
                HashSet<string> hashSet = GetUserAgents();
                return new ExcludeList(iPAddresses, hashSet);
            }

            public bool ContainsIpAddress(string addressString)
            {
                Assert.ArgumentNotNull(addressString, "addressString");
                if (IPAddress.TryParse(addressString, out IPAddress address))
                {
                    return ipaddresses.Contains(address);
                }
                return false;
            }

            public bool ContainsUserAgent(string userAgent)
            {
                Assert.ArgumentNotNull(userAgent, "userAgent");

                #region Sitecore.Support.368828

                return userAgents.Contains(userAgent, System.StringComparer.OrdinalIgnoreCase);

                #endregion

            }

            private static IPList GetIPAddresses()
            {
                XmlNode configNode = Factory.GetConfigNode("analyticsExcludeRobots/excludedIPAddresses");
                if (configNode == null)
                {
                    return new IPList();
                }
                return IPHelper.GetIPList(configNode) ?? new IPList();
            }

            private static HashSet<string> GetUserAgents()
            {
                XmlNode configNode = Factory.GetConfigNode("analyticsExcludeRobots/excludedUserAgents");
                HashSet<string> hashSet = new HashSet<string>();
                if (configNode == null)
                {
                    return hashSet;
                }
                string[] array = configNode.InnerText.Split('\n');
                for (int i = 0; i < array.Length; i++)
                {
                    string text = array[i].Trim();
                    if (!(text == string.Empty) && !text.StartsWith("#") && !hashSet.Contains(text))
                    {
                        hashSet.Add(text);
                    }
                }
                return hashSet;
            }
        }
    }
}