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
using System.Web;
using System.ComponentModel.DataAnnotations;
using Kooboo.CMS.Account.Models;
using Kooboo.Web.Mvc;
using System.ComponentModel;
using System.Web.Mvc;
using Kooboo.CMS.Web.Areas.Account.Models.DataSources;
using Kooboo.Collections;

namespace Kooboo.CMS.Web.Areas.Account.Models
{

    public class CreateUserModel
    {
        [DisplayName("User name")]
        [Remote("IsNameAvailable", "Users")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(RegexPatterns.FileName, ErrorMessage = "A user name cannot contain a space or any of the following characters:\\/:*?<>|~")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression(RegexPatterns.EmailAddress, ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [UIHint("PasswordStrength")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }

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

        public CreateUserModel()
        {

        }

        public CreateUserModel(User user)
        {
            UserName = user.UserName;
            Email = user.Email;
            Password = user.Password;
            IsAdministrator = user.IsAdministrator;
            IsLockedOut = user.IsLockedOut;
            UICulture = user.UICulture;
            GlobalRoles = string.IsNullOrEmpty(user.GlobalRoles) ? null : user.GlobalRoles.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }
        public User ToUser()
        {
            User user = new User
            {
                UserName = UserName,
                Email = Email,
                IsAdministrator = IsAdministrator,
                IsLockedOut = IsLockedOut,
                Password = Password,
                UICulture = UICulture,
                CustomFields = CustomFields
            };
            if (user.IsLockedOut)
            {
                user.UtcLastLockoutDate = DateTime.UtcNow;
            }
            if (GlobalRoles != null)
            {
                user.GlobalRoles = string.Join(",", GlobalRoles);
            }
            return user;
        }
    }
}