#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Kooboo.Web.Url;

namespace Kooboo.CMS.Content.Models.Paths
{
    public class TextContentPath : IPath
    {
        public TextContentPath(TextContent content)
        {
            var repository = new Repository(content.Repository);
            var repositoryPath = new RepositoryPath(repository);
            var textContent = content;
            var name = string.IsNullOrEmpty(textContent.FolderName) ? textContent.SchemaName : textContent.FolderName;
            this.PhysicalPath = Path.Combine(repositoryPath.PhysicalPath, ContentAttachementFolder, name, content.UUID);
            this.VirtualPath = UrlUtility.RawCombine(repositoryPath.VirtualPath, ContentAttachementFolder, name, content.UUID);
            var sysFolderName = Path.GetDirectoryName(this.PhysicalPath);
            if (!Directory.Exists(sysFolderName))
            {
                Directory.CreateDirectory(sysFolderName);
                var sysFolder = new DirectoryInfo(sysFolderName);
                sysFolder.Attributes = sysFolder.Attributes | FileAttributes.Hidden;
            }

        }

        public static string ContentAttachementFolder = "Attachments";

        #region IPath Members

        public string PhysicalPath
        {
            get;
            private set;
        }

        public string VirtualPath
        {
            get;
            private set;
        }

        public string SettingFile
        {
            get { throw new NotImplementedException(); }
        }

        public bool Exists()
        {
            return Directory.Exists(this.PhysicalPath);
        }

        public void Rename(string newName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
