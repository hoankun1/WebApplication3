// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication3.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;


    public IndexModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            Username = await _userManager.GetUserNameAsync(user);
            Email = await _userManager.GetEmailAsync(user);

            var phoneNumber =
                await _userManager.GetPhoneNumberAsync(user);

            Roles =
                (await _userManager.GetRolesAsync(user)).ToList();

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound(
                    $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound(
                    $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber =
                await _userManager.GetPhoneNumberAsync(user);

            if (Input.PhoneNumber != phoneNumber)
            {
                var result =
                    await _userManager.SetPhoneNumberAsync(
                        user,
                        Input.PhoneNumber);

                if (!result.Succeeded)
                {
                    StatusMessage =
                        "Lỗi khi cập nhật số điện thoại.";

                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage =
                "Cập nhật hồ sơ thành công.";

            return RedirectToPage();
        }
    }


}
