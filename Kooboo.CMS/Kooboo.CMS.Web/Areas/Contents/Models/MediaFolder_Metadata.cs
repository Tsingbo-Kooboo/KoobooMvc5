using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Web.Areas.Contents.Models.DataSources;
using Kooboo.ComponentModel;
using Kooboo.Web.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kooboo.CMS.Web.Areas.Contents.Models
{
    [MetadataFor(typeof(MediaFolder))]
    public class MediaFolder_Metadata
    {
        [DisplayName("Display name")]
        public string DisplayName { get; set; }

        [DisplayName("Allow extensions")]
        [Description("extensions that allowed to this folder,eg: .jpg,.png,.gif")]
        [UIHint("Multiple_DropDownList")]
        [DataSource(typeof(FileExtensionsDataSource))]
        public string AllowedExtensions { get; set; }
    }
}