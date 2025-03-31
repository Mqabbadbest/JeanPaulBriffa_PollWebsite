// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Presentation.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessageConfirmEmail { get; set; }
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            var rawQuery = HttpContext.Request.QueryString.Value;
            var userFromQuery = rawQuery.Split("&").FirstOrDefault(q => q.Contains("userId"));
            var codeFromQuery = rawQuery.Split("&").FirstOrDefault(q => q.Contains("code"));
            if (userFromQuery != null)
            {
                userId = userFromQuery.Split("=")[1];
            }
            if (codeFromQuery != null)
            {
                code = codeFromQuery.Split("=")[1];
            }


            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            StatusMessageConfirmEmail = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            return Redirect("~/");
        }
    }
}
