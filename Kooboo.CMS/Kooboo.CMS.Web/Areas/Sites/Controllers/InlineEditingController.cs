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
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Models.Binder;
using Kooboo.CMS.Sites;
using Kooboo.CMS.Sites.Models;
using Kooboo.CMS.Sites.Persistence;
using Kooboo.CMS.Sites.Services;
using Kooboo.CMS.Web.Authorizations;
using Kooboo.CMS.Web.Models;
using Kooboo.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kooboo.Web.Script.Serialization;
namespace Kooboo.CMS.Web.Areas.Sites.Controllers
{
    [Authorization(AreaName = "Sites", Group = "Page", Name = "Publish", Order = 1)]
    public class InlineEditingController : Kooboo.CMS.Sites.AreaControllerBase
    {
        IPageProvider PageProvider { get; set; }
        ITextContentBinder Binder { get; set; }
        public InlineEditingController(IPageProvider pageProvider, ITextContentBinder binder)
        {
            this.PageProvider = pageProvider;
            Binder = binder;
        }
        #region Content
        [HttpPost]
        public virtual ActionResult CopyContent(string schema, string uuid)
        {
            var data = new JsonResultData(ModelState);
            data.RunWithTry((resultData) =>
            {
                var content = Kooboo.CMS.Content.Services.ServiceFactory.TextContentManager.Copy(new Schema(Repository.Current, schema), uuid);
                resultData.Model = new
                {
                    uuid = content.UUID,
                    schema = content.SchemaName,
                    published = string.Empty,
                    editUrl = Url.Action("InlineEdit", new
                    {
                        controller = "TextContent",
                        Area = "Contents",
                        RepositoryName = content.Repository,
                        SiteName = Site.FullName,
                        FolderName = content.FolderName,
                        UUID = content.UUID
                    }),
                    summary = HttpUtility.HtmlAttributeEncode(content.GetSummary())
                };
            });
            return Json(data);
        }
        [HttpPost]
        public virtual ActionResult DeleteContent(string schema, string uuid)
        {
            var data = new JsonResultData(ModelState);
            data.RunWithTry((resultData) =>
            {
                Kooboo.CMS.Content.Services.ServiceFactory.TextContentManager.Delete(Repository.Current, new Schema(Repository.Current, schema), uuid);
            });
            return Json(data);
        }
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult UpdateContent(string schema, string uuid, string fieldName, string value)
        {
            var data = new JsonResultData(ModelState);
            data.RunWithTry((resultData) =>
            {
                var schemaObject = new Schema(Repository.Current, schema).AsActual();

                var fieldValue = Binder.ConvertToColumnType(schemaObject, fieldName, value);

                Kooboo.CMS.Content.Services.ServiceFactory.TextContentManager.Update(Repository.Current, schemaObject, uuid, fieldName, fieldValue, User.Identity.Name);
            });
            return Json(data);
        }
        #endregion

        #region Html Position
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult UpdateHtml(string positionId, string pageName, string value, bool? _draft_)
        {
            var data = new JsonResultData(ModelState);
            data.RunWithTry((resultData) =>
                {
                    Page page = (PageHelper.Parse(Site.Current, pageName)).AsActual();

                    if (page != null)
                    {
                        bool isDraft = _draft_.HasValue && _draft_.Value == true;
                        if (isDraft)
                        {
                            page = PageProvider.GetDraft(page);
                        }
                        var position = page.PagePositions.Where(it => it.PagePositionId.EqualsOrNullEmpty(positionId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (position != null && position is HtmlPosition)
                        {
                            ((HtmlPosition)position).Html = value;
                        }
                        if (isDraft)
                        {
                            PageProvider.SaveAsDraft(page);
                        }
                        else
                        {
                            CMS.Sites.Services.ServiceFactory.PageManager.Update(Site.Current, page, page);
                        }
                    }
                });
            return Json(data);
        }
        #endregion

        #region Label
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult UpdateLable(string uuid, string key, string category, string value)
        {
            var data = new JsonResultData(ModelState);
            data.RunWithTry((resultData) =>
                {
                    var label = ServiceFactory.LabelManager.Get(Site.Current, category, key);
                    if (label != null)
                    {
                        label.Value = value;
                        label.UtcLastestModificationDate = DateTime.UtcNow;
                        label.LastestEditor = User.Identity.Name;
                        ServiceFactory.LabelManager.Update(Site.Current, label, label);
                    }

                });
            return Json(data);
        }
        #endregion

        #region Html Block
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult UpdateHtmlBlock(string blockName, string value)
        {
            var data = new JsonResultData(ModelState);
            data.RunWithTry((resultData) =>
            {
                var manager = ServiceFactory.GetService<HtmlBlockManager>();
                var old = manager.Get(this.Site, blockName);
                var @new = new HtmlBlock(this.Site, blockName) { Body = value, UserName = User.Identity.Name };
                manager.Update(this.Site, @new, old);
            });
            return Json(data);
        }
        #endregion

        #region Variables
        [RequiredLogOn(Exclusive = true, Order = 0)]
        public virtual ActionResult Variables(string pageName, string _draft_)
        {
            var vars = (new
            {
                #region urls
                CopyContent = Url.Action("CopyContent", new { controller = "InlineEditing", repositoryName = Repository.Current.Name, siteName = Site.Current.FullName, area = "Sites" }),
                UpdateContent = Url.Action("UpdateContent", new { controller = "InlineEditing", repositoryName = Repository.Current.Name, siteName = Site.Current.FullName, area = "Sites" }),
                DeleteContent = Url.Action("DeleteContent", new { controller = "InlineEditing", repositoryName = Repository.Current.Name, siteName = Site.Current.FullName, area = "Sites" }),
                UpdateHtml = Url.Action("UpdateHtml", new { controller = "InlineEditing", repositoryName = Repository.Current.Name, siteName = Site.Current.FullName, area = "Sites", pageName = pageName, _draft_ = _draft_ }),
                UpdateHtmlBlock = Url.Action("UpdateHtmlBlock", new { controller = "InlineEditing", repositoryName = Repository.Current.Name, siteName = Site.Current.FullName, area = "Sites" }),
                UpdateLable = Url.Action("UpdateLable", new { controller = "InlineEditing", repositoryName = Repository.Current.Name, siteName = Site.Current.FullName, area = "Sites" }),
                MediaLibrary = Url.Action("Selection", new { controller = "MediaContent", repositoryName = Repository.Current.Name, siteName = Site.Current.FullName, area = "Contents", listType = "grid" }),
                ApplicationPath = Url.Content("~"),
                #endregion

                #region anchorBar
                blockMenuAnchorBar_js = new
                {
                    editBtnTitle = "Edit".Localize("InlineEditor"),
                    copyBtnTitle = "Copy".Localize("InlineEditor"),
                    deleteBtnTitle = "Delete".Localize("InlineEditor"),
                    publishBtnTitle = "Publish".Localize("InlineEditor"),
                    unpublishedTip = "This item has not been published yet.<br/>Click to publish this item.".Localize("InlineEditor")
                },

                fieldMenuAnchorBar_js = new
                {
                    editBtnTitle = "Edit".Localize("InlineEditor")
                },

                htmlMenuAnchorBar_js = new
                {
                    editBtnTitle = "Edit".Localize("InlineEditor")
                },

                inlineEditorAnchorBar_js = new
                {
                    saveBtnTitle = "Save".Localize("InlineEditor"),
                    cancelBtnTitle = "Cancel".Localize("InlineEditor"),
                    fontFamilyTitle = "Font family".Localize("InlineEditor"),
                    fontSizeTitle = "Font size".Localize("InlineEditor"),
                    fontColorTitle = "Font color".Localize("InlineEditor"),
                    backColorTitle = "Background color".Localize("InlineEditor")
                },
                #endregion

                #region colorPicker
                colorPicker_js = new
                {
                    caption = "Color picker".Localize("InlineEditor"),
                    description = "Select a color or insert a hex code value.".Localize("InlineEditor"),
                    originalColor = "Original:".Localize("InlineEditor"),
                    newColor = "New:".Localize("InlineEditor"),
                    hexValue = "Hex value:".Localize("InlineEditor"),
                    useNewColorBtn = "OK".Localize("InlineEditor"),
                    cancelBtn = "Cancel".Localize("InlineEditor")
                },
                #endregion

                #region editor
                sniffer_js = new
                {
                    widthFormatError = "Invalid input width".Localize("InlineEditor"),
                    heightFormatError = "Invalid input height".Localize("InlineEditor"),
                    imgSizeConfirm = "The image size is too big for this layout.\nAre you sure you want to use this size?".Localize("InlineEditor"),
                    deleteImgConfirm = "Are you sure you want to delete this image?".Localize("InlineEditor"),
                    unlinkConfirm = "Are you sure you want to delete the link?".Localize("InlineEditor")
                },

                toolbarButton_js = new
                {
                    bold = "Bold".Localize("InlineEditor"),
                    italic = "Italic".Localize("InlineEditor"),
                    underline = "Underline".Localize("InlineEditor"),
                    alignLeft = "Align left".Localize("InlineEditor"),
                    alignCenter = "Align center".Localize("InlineEditor"),
                    alignRight = "Align right".Localize("InlineEditor"),
                    alignJustify = "Align justify".Localize("InlineEditor"),
                    numberList = "Number list".Localize("InlineEditor"),
                    bulletList = "Bullet list".Localize("InlineEditor"),
                    indent = "Increase indent".Localize("InlineEditor"),
                    outdent = "Decrease indent".Localize("InlineEditor"),
                    insertImage = "Insert image".Localize("InlineEditor"),
                    insertLink = "Insert link".Localize("InlineEditor"),
                    editSource = "Edit source".Localize("InlineEditor"),
                    redo = "Redo".Localize("InlineEditor"),
                    undo = "Undo".Localize("InlineEditor"),
                    unformat = "Remove format".Localize("InlineEditor"),
                    horizontalRuler = "Insert horizontal ruler".Localize("InlineEditor"),
                    pastePlainText = "Paste with plain text".Localize("InlineEditor")
                },
                #endregion

                #region inline
                block_js = new
                {
                    confirmDel = "Are you sure you want to delete this item?".Localize("InlineEditor"),
                    networkError = "Network error, the action has been cancelled.".Localize("InlineEditor"),
                    copying = "Copying...".Localize("InlineEditor"),
                    deleting = "Deleting...".Localize("InlineEditor"),
                    publishing = "Publishing...".Localize("InlineEditor"),
                    copySuccess = "Copy successfully.".Localize("InlineEditor"),
                    deleteSuccess = "Delete successfully.".Localize("InlineEditor"),
                    publishSuccess = "Publish successfully.".Localize("InlineEditor"),
                    copyFailure = "The attempt to copy has failed.".Localize("InlineEditor"),
                    deleteFailure = "The attempt to delete has failed.".Localize("InlineEditor"),
                    publishFailure = "The attempt to publish has failed.".Localize("InlineEditor")
                },

                field_js = new
                {
                    networkError = "Network error, the action has been cancelled.".Localize("InlineEditor"),
                    saving = "Saving...".Localize("InlineEditor"),
                    saveSuccess = "Save successfully.".Localize("InlineEditor"),
                    saveFailure = "The attempt to save has failed.".Localize("InlineEditor")
                },
                blockAnchor_js = new
                {
                    editBtnTitle = "edit".Localize("InlineEditor"),
                    copyBtnTitle = "copy".Localize("InlineEditor"),
                    deleteBtnTitle = "delete".Localize("InlineEditor"),
                    publishBtnTitle = "publish".Localize("InlineEditor"),
                    unpublishedTip = "This item has not been published yet.<br/>Click to publish this item.".Localize("InlineEditor")
                },
                editorAnchor_js = new
                {
                    saveBtnTitle = "save".Localize("InlineEditor"),
                    cancelBtnTitle = "cancel".Localize("InlineEditor"),
                    fontFamilyTitle = "font family".Localize("InlineEditor"),
                    fontSizeTitle = "font size".Localize("InlineEditor"),
                    headingTitle = "heading".Localize("InlineEditor"),
                    fontColorTitle = "font color".Localize("InlineEditor"),
                    backColorTitle = "background color".Localize("InlineEditor")
                },
                fieldAnchor_js = new
                {
                    editBtnTitle = "edit".Localize("InlineEditor")
                },
                htmlAnchor_js = new
                {
                    editBtnTitle = "edit".Localize("InlineEditor")
                },
                #endregion

                #region panel

                imagePanel_js = new
                {
                    title = "Image options".Localize("InlineEditor"),
                    imgLibTitle = "Image library".Localize("InlineEditor"),
                    attrURL = "URL".Localize("InlineEditor"),
                    attrALT = "ALT".Localize("InlineEditor"),
                    attrWidth = "Width".Localize("InlineEditor"),
                    attrHeight = "Height".Localize("InlineEditor"),
                    btnOk = "Save".Localize("InlineEditor"),
                    btnCancel = "Cancel".Localize("InlineEditor"),
                    btnRemove = "Remove".Localize("InlineEditor"),
                    btnLibrary = "Library".Localize("InlineEditor"),
                    btnView = "View".Localize("InlineEditor"),
                    attrLinkHref = "LINK".Localize("InlineEditor"),
                    emptyUrlMsg = "Please input the url.".Localize("InlineEditor")
                },

                linkPanel_js = new
                {
                    headTitle = "Link panel".Localize("InlineEditor"),
                    btnOk = "OK".Localize("InlineEditor"),
                    btnCancel = "Cancel".Localize("InlineEditor"),
                    btnUnlink = "Unlink".Localize("InlineEditor"),
                    lblText = "Text:".Localize("InlineEditor"),
                    lblUrl = "Url:".Localize("InlineEditor"),
                    lblTitle = "Title:".Localize("InlineEditor"),
                    lblLinkType = "Link type:".Localize("InlineEditor"),
                    lblNewWin = "Open in a new window".Localize("InlineEditor"),
                    urlEmptyMsg = "Please input the url.".Localize("InlineEditor")
                },

                textPanel_js = new
                {
                    headTitle = "Text panel".Localize("InlineEditor"),
                    btnPreview = "Preview".Localize("InlineEditor"),
                    btnReset = "Reset".Localize("InlineEditor"),
                    btnOk = "OK".Localize("InlineEditor"),
                    btnCancel = "Cancel".Localize("InlineEditor")
                },
                #endregion

                #region positionAnchor
                positionAnchor_js = new
                {
                    addViewTitle = "Add a view".Localize("InlineEditor"),
                    addModuleTitle = "Add a module".Localize("InlineEditor"),
                    addFolderTitle = "Add a data folder".Localize("InlineEditor"),
                    addHtmlTitle = "Add HTML".Localize("InlineEditor"),
                    addHtmlBlockTitle = "Add a HTML block".Localize("InlineEditor")
                },
                #endregion

                #region design
                design_js = new
                {
                    removeConfirm = "Are you sure you want to remove?".Localize("InlineEditor"),
                    // add
                    addViewTitle = "Add a view".Localize("InlineEditor"),
                    addHtmlTitle = "Add HTML".Localize("InlineEditor"),
                    addModuleTitle = "Add a module".Localize("InlineEditor"),
                    addFolderTitle = "Add a data folder".Localize("InlineEditor"),
                    addHtmlBlockTitle = "Add a HTML block".Localize("InlineEditor"),
                    // edit
                    editViewTitle = "Edit view".Localize("InlineEditor"),
                    editHtmlTitle = "Edit HTML".Localize("InlineEditor"),
                    editModuleTitle = "Edit module".Localize("InlineEditor"),
                    editFolderTitle = "Edit data folder".Localize("InlineEditor"),
                    editHtmlBlockTitle = "Edit HTML block".Localize("InlineEditor"),
                    // btn
                    editBtnTitle = "Edit".Localize("InlineEditor"),
                    removeBtnTitle = "Remove".Localize("InlineEditor"),
                    expandBtnTitle = "Expand".Localize("InlineEditor"),
                    closeBtnTitle = "Close".Localize("InlineEditor"),
                    moveBtnTitle = "Move".Localize("InlineEditor")
                }
                #endregion
            }).ToJSON();

            // ret
            return JavaScript(string.Format("var __inlineEditVars = {0};", vars));
        }
        #endregion
    }
}
