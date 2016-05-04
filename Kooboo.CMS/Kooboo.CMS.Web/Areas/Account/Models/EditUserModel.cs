#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using Kooboo.CMS.Account.Models;
using Kooboo.CMS.Web.Areas.Account.Models.DataSources;
using Kooboo.Web.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kooboo.Collections;

namespace Kooboo.CMS.Web.Areas.Account.Models
{
    public class EditUserModel
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression(RegexPatterns.EmailAddress, ErrorMessage = "Invalid email address")]
        public string Email { get; set; }


        [Display(Name = "Is administrator")]
        public bool IsAdministrator { get; set; }
        [Display(Name = "Is locked out")]
        public bool IsLockedOut { get; set; }

        [UIHint("DropDownList")]
        [DataSource(typeof(CultureSelectListDataSource))]
        [Description("The culture represents the current culture used by the Resource Manager to look up culture-specific resources at run time.")]
        public string UICulture { get; set; }

        [DisplayName("Global roles")]
        [UIHint("Multiple_DropDownList")]
        [DataSource(typeof(RolesDatasource))]
        [Description("Users with global roles have access to all websites using the defined roles.")]
        public string[] GlobalRoles { get; set; }

        [UIHint("Dictionary")]
        public Dictionary<string, string> CustomFields { get; set; }

        public EditUserModel()
        {

        }
        public EditUserModel(User user)
        {
            UserName = user.UserName;
            Email = user.Email;
            IsAdministrator = user.IsAdministrator;
            IsLockedOut = user.IsLockedOut;
            UICulture = user.UICulture;
            GlobalRoles = string.IsNullOrEmpty(user.GlobalRoles) ? null : user.GlobalRoles.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            CustomFields = user.CustomFields;
        }
        public User ToUser(User userToUpdate)
        {
            userToUpdate.UserName = UserName;
            userToUpdate.Email = Email;
            userToUpdate.IsAdministrator = IsAdministrator;
            userToUpdate.IsLockedOut = IsLockedOut;
            userToUpdate.CustomFields = CustomFields;
            if (userToUpdate.IsLockedOut)
            {
                userToUpdate.UtcLastLockoutDate = DateTime.UtcNow;
            }
            userToUpdate.UICulture = UICulture;
            userToUpdate.GlobalRoles = GlobalRoles != null ? string.Join(",", GlobalRoles) : null;

            return userToUpdate;
        }
    }
}