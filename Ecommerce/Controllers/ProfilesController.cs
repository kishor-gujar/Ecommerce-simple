using System.IO;
using System.Threading.Tasks;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfilesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }


        // GET: Profiles/Details/5
        public async Task<IActionResult> Details()
        {
            var curruntuser = await _userManager.GetUserAsync(User);

            var profile = await _context.Profiles
                .SingleOrDefaultAsync(m => m.Id == curruntuser.ProfileId);

            if (profile == null)
                return NotFound();

            return View(profile);
        }

        // GET: Profiles/Edit/5
        public async Task<IActionResult> Edit()
        {
            var currentuser = await _userManager.GetUserAsync(User);

            var profile = await _context.Profiles.SingleOrDefaultAsync(m => m.Id == currentuser.ProfileId);

            if (profile == null)
                return NotFound();

            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("Id,DisplayName,Birthdate,Height,Description,PhoneNumber,Occupation,ProfilePicture,Smoking")]
        Profile profile, IFormFile ProfilePicture)
        {
            if (ModelState.IsValid)
            {
                var currntUser = await _userManager.GetUserAsync(User);

                if (ProfilePicture != null)
                {
                    var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "Profiles");

                    Directory.CreateDirectory(Path.Combine(uploadPath, $"{currntUser.Id}"));

                    var fileName = Path.GetFileName(ProfilePicture.FileName);

                    using (
                        var fileStream = new FileStream(Path.Combine(uploadPath, $"{currntUser.Id}", fileName),
                            FileMode.Create))
                    {
                        await ProfilePicture.CopyToAsync(fileStream);
                    }
                    profile.ProfilePicture = $"{currntUser.Id}/{fileName}";
                }
                _context.Update(profile);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details");
            }
            return View(profile);
        }
    }
}