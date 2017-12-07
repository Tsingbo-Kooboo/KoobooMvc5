using Kooboo.CMS.Content.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.CMS.Content.EventBus.Content
{
    public static class TextFolderEvent
    {
        public static void Fire(ContentAction contentAction, TextFolder textFolder)
        {
            EventBus.Send(new TextFolderEventContext(contentAction, textFolder));
        }
    }
}
