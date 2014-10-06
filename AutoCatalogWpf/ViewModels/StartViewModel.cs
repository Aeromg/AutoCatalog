using System;
using AutoCatalogWpf.Utils;

namespace AutoCatalogWpf.ViewModels
{
    public class StartViewModel : ViewModel
    {
        public string ReleaseInfo
        {
            get { return String.Format("build:{0}", VersionInfo.GetVersionByLinkerTimestamp()); }
        }
    }
}