using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WebBlog
{
    public class AppVersionInfo
    {
        private const string _buildFileName = ".buildinfo.json";
        private readonly string _buildFilePath;
        private string _buildNumber = string.Empty;
        private string _buildId = string.Empty;
        private string _gitHash = string.Empty;
        private string _gitShortHash = string.Empty;

        /// <summary>
        /// AppVersionInfo
        /// </summary>
        /// <param name="hostEnvironment"></param>
        public AppVersionInfo(IHostEnvironment hostEnvironment)
        {
            _buildFilePath = Path.Combine(hostEnvironment.ContentRootPath, _buildFileName);
        }

        /// <summary>
        /// BuildNumber
        /// </summary>
        public string BuildNumber
        {
            get
            {
                if (string.IsNullOrEmpty(_buildNumber))
                {
                    if (File.Exists(_buildFilePath))
                    {
                        var fileContents = File.ReadLines(_buildFilePath).ToList();

                        if (fileContents.Count > 0)
                        {
                            _buildNumber = fileContents[0];
                        }
                        if (fileContents.Count > 1)
                        {
                            _buildId = fileContents[1];
                        }
                    }

                    if (string.IsNullOrEmpty(_buildNumber))
                    {
                        _buildNumber = DateTime.UtcNow.ToString("yyyyMMdd") + ".0";
                    }

                    if (string.IsNullOrEmpty(_buildId))
                    {
                        _buildId = "123456";
                    }
                }

                return _buildNumber;
            }
        }

        /// <summary>
        /// BuildId
        /// </summary>
        public string BuildId
        {
            get
            {
                if (string.IsNullOrEmpty(_buildId))
                {
                    var _ = BuildNumber;
                }

                return _buildId;
            }
        }

        /// <summary>
        /// GitHash
        /// </summary>
        public string GitHash
        {
            get
            {
                if (string.IsNullOrEmpty(_gitHash))
                {
                    var version = "1.0.0+LOCALBUILD";
                    var appAssembly = typeof(AppVersionInfo).Assembly;
                    var infoVerAttr = (AssemblyInformationalVersionAttribute)appAssembly
                        .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute)).FirstOrDefault();

                    if (infoVerAttr != null && infoVerAttr.InformationalVersion.Length > 6)
                    {
                        version = infoVerAttr.InformationalVersion;
                    }
                    _gitHash = version[(version.IndexOf('+') + 1)..];
                }

                return _gitHash;
            }
        }

        /// <summary>
        /// ShortGitHash
        /// </summary>
        public string ShortGitHash
        {
            get
            {
                if (string.IsNullOrEmpty(_gitShortHash))
                {
                    _gitShortHash = GitHash.Substring(GitHash.Length - 6, 6);
                }
                return _gitShortHash;
            }
        }
    }
}
