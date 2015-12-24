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
using Kooboo.Globalization;

namespace Kooboo.CMS.Form.Html.Controls
{
    public class Tinymce : ControlBase
    {
        private new const string EditorTemplate = @"<tr>
            <th>
            <label for=""{0}"">{1}</label>
            <div class=""tip""><input type=""checkbox"" checked=""checked"" id=""{0}{5}"" class=""{5}""/><label for=""{0}{5}"">{4}</label></div>
            </th>
            <td>
            {2}            
            @Html.ValidationMessageForColumn(((ISchema)ViewData[""Schema""])[""{0}""], null)
            {3}
            </td>  
            </tr>";

        public override string Name
        {
            get { return "Tinymce"; }
        }

        protected override string RenderInput(IColumn column)
        {
            return string.Format(@"<textarea name=""{0}"" id=""{0}"" class=""{0} tinymce"" media_library_url=""@Url.Action(""Selection"",""MediaContent"",ViewContext.RequestContext.AllRouteValues()))"" media_library_title =""@(""Selected Files"".Localize())"" rows=""10"" cols=""100"">@( Model.{0} ?? """")</textarea>", column.Name);
        }

        public override string Render(ISchema schema, IColumn column)
        {
            string html = string.Format(EditorTemplate, 
                column.Name,
                (string.IsNullOrEmpty(column.Label) ? column.Name : column.Label).RazorHtmlEncode(), 
                RenderInput(column),
                FormHelper.Tooltip(column.Tooltip),
                "Load site css".Localize(),
                FormHelper.CSSLoadTogglerNameSuffix
                );

            return html;
        }

     
    }
}
