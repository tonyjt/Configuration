﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Configuration
{
    public static class PathHelper
    {
        /// <summary>
        /// 应用程序路径
        /// </summary>
        static public string ApplicationPath
        {

            get
            {
                string applicationPath = "/";

                if (HttpContext.Current != null)
                    applicationPath = HttpContext.Current.Request.ApplicationPath;

                if (applicationPath == "/")
                {
                    return string.Empty;
                }
                else
                {
                    return applicationPath.ToLower();
                }
            }

        }

        public static string MapPath(string path)
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Server.MapPath(path);
            else
                return PhysicalPath(path.Replace("/", Path.DirectorySeparatorChar.ToString()).Replace("~", ""));
        }


        public static string PhysicalPath(string path)
        {
            return string.Concat(RootPath().TrimEnd(Path.DirectorySeparatorChar), Path.DirectorySeparatorChar.ToString(), path.TrimStart(Path.DirectorySeparatorChar));
        }

        public static string CalculateStorageLocation(string storageLocation)
        {
            return CalculateStorageLocation(storageLocation, false);
        }

        public static string CalculateStorageLocation(string storageLocation, bool endwithDirectorySeparator)
        {
            string calculatedStorageLocation = null;

            // 如果是本地路径
            if ((storageLocation.IndexOf(":\\") != -1) || (storageLocation.IndexOf("\\\\") != -1))
                calculatedStorageLocation = storageLocation;
            else
                calculatedStorageLocation = MapPath(storageLocation);

            if (endwithDirectorySeparator)
            {
                // 以斜杠结尾
                if (!calculatedStorageLocation.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    calculatedStorageLocation += Path.DirectorySeparatorChar;
            }
            return calculatedStorageLocation;
        }


        public static string CalculateLocation(string location)
        {
            if (location == null)
                return null;

            string calculatedLocation;

            // 如果已经是物理路径
            //
            if ((location.IndexOf(":\\") != -1) || (location.IndexOf("\\\\") != -1))
                calculatedLocation = location;
            else
                calculatedLocation = MapPath(location);

            if (!calculatedLocation.EndsWith(Path.DirectorySeparatorChar.ToString()))
                calculatedLocation += Path.DirectorySeparatorChar;


            return calculatedLocation;
        }

        private static string _rootPath = null;

        public static string RootPath()
        {
            if (_rootPath == null)
            {
                _rootPath = AppDomain.CurrentDomain.BaseDirectory;
                string dirSep = Path.DirectorySeparatorChar.ToString();

                _rootPath = _rootPath.Replace("/", dirSep);

            }
            return _rootPath;
        }

        public static string HostPath(Uri uri)
        {
            string str = (uri.Port == 80) ? string.Empty : (":" + uri.Port.ToString());
            return string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, str);
        }

        public static string FullPath(string local)
        {
            if (string.IsNullOrEmpty(local))
            {
                return local;
            }
            if (local.ToLower().StartsWith("http://") || local.ToLower().StartsWith("https://"))
            {
                return local;
            }
            if (HttpContext.Current == null)
            {
                return local;
            }
            return FullPath(HostPath(HttpContext.Current.Request.Url), local);
        }

        public static string FullPath(string hostPath, string local)
        {
            return (hostPath + local);
        }

    }
}
