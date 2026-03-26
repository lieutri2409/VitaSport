// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shopping.Models;

namespace Shopping.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        // hiển thị role
        public string UserRole { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
            [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải là 10 chữ số")]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = user.PhoneNumberCustom
            };

            // load role
            var roles = await _userManager.GetRolesAsync(user);
            UserRole = roles.FirstOrDefault() ?? "Customer";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                Username = await _userManager.GetUserNameAsync(user);

                var roles = await _userManager.GetRolesAsync(user);
                UserRole = roles.FirstOrDefault() ?? "Customer";

                return Page();
            }

            var phoneExist = _userManager.Users
                .Any(u => u.PhoneNumberCustom == Input.PhoneNumber && u.Id != user.Id);

            if (phoneExist)
            {
                ModelState.AddModelError(string.Empty, "Số điện thoại đã được sử dụng.");

                Username = await _userManager.GetUserNameAsync(user);

                var roles = await _userManager.GetRolesAsync(user);
                UserRole = roles.FirstOrDefault() ?? "Customer";

                return Page();
            }

            if (Input.PhoneNumber != user.PhoneNumberCustom)
            {
                user.PhoneNumberCustom = Input.PhoneNumber;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Cập nhật thông tin thành công";

            return RedirectToPage();
        }
    }
}