#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using Kooboo.CMS.Common;
using Kooboo.CMS.Common.Persistence.Non_Relational;
using Kooboo.CMS.Content.EventBus.Content;
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Services;
using Kooboo.CMS.Sites;
using Kooboo.CMS.Web.Models;
using Kooboo.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Kooboo.CMS.Web.Areas.Contents.Controllers
{
    public class MediaFolderController : ManagerControllerBase
    {
        #region .ctor
        MediaContentManager ContentManager
        {
            get;
            set;
        }

        MediaFolderManager FolderManager
        {
            get;
            set;
        }
        public MediaFolderController(MediaContentManager mediaContentManager, MediaFolderManager mediaFolderManager)
        {
            this.ContentManager = mediaContentManager;
            this.FolderManager = mediaFolderManager;
        } 
        #endregion
        
        #region Index
        public virtual ActionResult Index(string fullName, string search)
        {
            var folders = FolderManager.All(Repository, search, fullName);

            return View(folders);
        } 
        #endregion

        [HttpPost]
        public virtual ActionResult Create(MediaFolder model, string folderName)
        {
            var data = new JsonResultData(ModelState);
            if (ModelState.IsValid)
            {
                data.RunWithTry((resultData) =>
                {
                    MediaFolder parent = null;
                    if (!string.IsNullOrEmpty(folderName))
                    {
                        parent = FolderHelper.Parse<MediaFolder>(Repository, folderName).AsActual();
                    }
                    model.Parent = parent;

                    FolderManager.Add(Repository, model);
                });
            }

            return Json(data);
        }

        #region Edit
        [Kooboo.CMS.Web.Authorizations.Authorization(AreaName = "Contents", Group = "", Name = "Folder", Order = 99)]
        public virtual ActionResult Edit(string UUID)
        {
            var model = FolderManager.Get(Repository, UUID).AsActual();
            return View(model);
        }
        [Kooboo.CMS.Web.Authorizations.Authorization(AreaName = "Contents", Group = "", Name = "Folder", Order = 99)]
        [HttpPost]
        public virtual ActionResult Edit(MediaFolder model, string UUID, string successController, string @return)
        {
            var data = new JsonResultData(ModelState);
            if (ModelState.IsValid)
            {
                data.RunWithTry((resultData) =>
                {
                    UUID = string.IsNullOrEmpty(UUID) ? model.FullName : UUID;
                    var old = FolderManager.Get(Repository, UUID);
                    model.Parent = old.Parent;
                    //TextFolderEvent.Fire(ContentAction.PreUpdate, model);
                    FolderManager.Update(Repository, model, old);
                    //TextFolderEvent.Fire(ContentAction.Update, model);
                    resultData.RedirectUrl = @return;
                });
            }
            return Json(data);
        }
        #endregion

        public virtual ActionResult Delete(string selectedFolders)
        {
            var data = new JsonResultData(ModelState);
            data.RunWithTry((resultData) =>
            {
                var folderArr = selectedFolders.Split(',');
                foreach (var f in folderArr)
                {
                    if (string.IsNullOrEmpty(f)) continue;
                    FolderManager.Remove(Repository, new MediaFolder(Repository, f));
                }
            });
            return Json(data);
        }

    }
}
