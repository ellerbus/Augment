using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Augment.Caching
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <![CDATA[
    /// <httpHandlers>
    ///     <add verb="GET" path="{filename cache.manifest}" type="Augment.Caching.CacheManifestHttpHandler" />
    /// </httpHandlers>
    /// ]]>
    /// </remarks>
    public abstract class CacheManifestHttpHandler : IHttpHandler
    {
        #region Members

        private static readonly string NL = Environment.NewLine;

        #endregion

        #region Methods

        /// <summary>
        /// Those assets that should be cached by the browser via the manifest
        /// files (use URL pathing for exmaple ~/file.js or /file.css)
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<string> GetCacheSectionFiles();

        /// <summary>
        /// Those assets that should be referenced ONLINE at all times
        /// (use URL pathing for exmaple ~/file.html or /file.html)
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<string> GetNetworkSectionFiles();

        private void WriteCacheSection(HttpResponse res)
        {
            res.Write("CACHE:" + NL);

            WriteFiles(res, GetCacheSectionFiles());

            res.Write(NL);
        }

        private void WriteNetworkSection(HttpResponse res)
        {
            res.Write("NETWORK:" + NL);

            IList<string> files = GetNetworkSectionFiles().ToList();

            if (files.Count == 0)
            {
                res.Write("*" + NL);
            }
            else
            {
                WriteFiles(res, files);
            }

            res.Write(NL);
        }

        private void WriteFiles(HttpResponse res, IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                if (file.StartsWith("~"))
                {
                    res.Write(VirtualPathUtility.ToAbsolute(file) + NL);
                }
                else
                {
                    res.Write(file + NL);
                }
            }
        }

        /// <summary>
        /// Scan a folder and gather files based on search pattern
        /// </summary>
        /// <param name="mapPath">(for example &quot;~/Assets&quot;)</param>
        /// <param name="searchPattern">File Search Pattern, no a regex pattern (for example &quot;*.js;*.css&quot;)</param>
        /// <returns></returns>
        protected IEnumerable<string> GetUrlPathToFiles(string mapPath, string searchPattern = "*.js;*.css")
        {
            if (!mapPath.EndsWith("/"))
            {
                mapPath += "/";
            }

            DirectoryInfo root = new DirectoryInfo(HostingEnvironment.MapPath("~/"));

            string[] searchPatterns = searchPattern.Split(';')
                .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim())
                .ToArray();

            string replaceRoot = root.FullName;

            DirectoryInfo app = new DirectoryInfo(HostingEnvironment.MapPath(mapPath));

            foreach (string file in GetFiles(app, searchPatterns))
            {
                string path = file.Replace(replaceRoot, "~/").Replace('\\', '/');

                yield return path;
            }
        }

        private IEnumerable<string> GetFiles(DirectoryInfo dir, string[] fileSearchPatterns)
        {
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                foreach (string file in GetFiles(d, fileSearchPatterns))
                {
                    yield return file;
                }
            }

            foreach (string pattern in fileSearchPatterns)
            {
                foreach (FileInfo f in dir.GetFiles(pattern))
                {
                    yield return f.FullName;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The version for this manifest (typically Assembly.Version)
        /// </summary>
        public abstract string Version { get; }

        #endregion

        #region IHttpHandler Members

        /// <summary>
        /// 
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            HttpResponse res = context.Response;

            HttpCachePolicy cp = res.Cache;

            cp.SetCacheability(HttpCacheability.NoCache);
            cp.SetNoStore();
            cp.SetExpires(DateTime.MinValue);

            res.ContentType = "text/cache-manifest";

            res.Write("CACHE MANIFEST" + NL);

            res.Write("# version " + Version + NL + NL);

            WriteCacheSection(res);

            WriteNetworkSection(res);
        }

        #endregion
    }
}
