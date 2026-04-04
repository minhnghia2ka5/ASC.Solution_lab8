using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace ASC.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ResetPasswordModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "Email không được để trống.")]
            [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "Mật khẩu không được để trống.")]
            [StringLength(100, ErrorMessage = "Mật khẩu phải từ {2} đến {1} ký tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string? Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
            public string? ConfirmPassword { get; set; }

            public string? Code { get; set; }
        }

        public IActionResult OnGet(string? code = null)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Bạn phải cung cấp mã để đặt lại mật khẩu.");
            }

            Input.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(Input.Email))
            {
                ModelState.AddModelError(string.Empty, "Email không hợp lệ.");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Địa chỉ email không tồn tại.");
                return Page();
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code ?? string.Empty, Input.Password ?? string.Empty);
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}