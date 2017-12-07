using Kooboo.CMS.Content.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.CMS.Content.EventBus.Content
{
    public class TextFolderEventContext : IEventContext
    {
        public TextFolderEventContext(ContentAction contentAction, TextFolder folder)
        {
            ContentAction = contentAction;
            Folder = folder;
        }

        public object State => this.Folder;

        public ContentAction ContentAction { get; set; }

        public TextFolder Folder { get; private set; }
    }
}
