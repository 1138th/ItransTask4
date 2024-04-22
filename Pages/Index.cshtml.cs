using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication4.Data;

namespace WebApplication4.Pages
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty] public InputModel Input { get; set; }
        public User[] Users { get; set; }

        public class InputModel
        {
            public List<bool> Checkboxes { get; set; }
            public string ActionType { get; set; }
        }

        public void OnGet()
        {
            Users = _userManager.Users.ToArray();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Users = _userManager.Users.ToArray();
            for (var i = 0; i < Users.Length; i++)
            {
                if (!Input.Checkboxes[i]) continue;
                var user = await _userManager.FindByEmailAsync(Users[i].Email);
                await MakeAction(user);
                _logger.LogInformation("Status for {0} was set to: {1}.", user.Email, Input.ActionType);
            }
            return RedirectToPage("/Index");
        }

        private async Task MakeAction(User user)
        {
            switch (Input.ActionType)
            {
                case "Delete":
                    await SignOutIfSignedIn(user);
                    await _userManager.DeleteAsync(user);
                    break;
                case "Block":
                    user.Status = false;
                    await SignOutIfSignedIn(user);
                    await _userManager.UpdateAsync(user);
                    break;
                default:
                    user.Status = true;
                    await _userManager.UpdateAsync(user);
                    break;
            }
        }

        private async Task SignOutIfSignedIn(User user)
        {
            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
            }
        }
    }
}