using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var carts = from c in _context.Carts
                where c.UserId == userId
                select c;

            var applicationDbContext = carts.Include(c => c.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var cart = await _context.Carts.Include(c => c.Product).SingleOrDefaultAsync(m => m.ProductId == id);

            if (cart == null)
                return NotFound();

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            return RedirectToAction("Index");
        }

        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,UserId,ProductId,Quantity")] Cart cart)
        {
            var usre = await _userManager.GetUserAsync(User);

            if (ProductCartExist(cart.ProductId, usre.Id))
            {
                TempData["cartExist"] = "This product alreay in cart";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid) return View(cart);

            cart.UserId = usre.Id;

            _context.Add(cart);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var cart = await _context.Carts.SingleOrDefaultAsync(m => m.ProductId == id);
            if (cart == null)
                return NotFound();
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", cart.ProductId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ProductId,Quantity")] Cart cart)
        {
            if (id != cart.Product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction("Index");
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var cart = await _context.Carts
                .Include(c => c.Product)
                .SingleOrDefaultAsync(m => m.Product.Id == id);
            if (cart == null)
                return NotFound();

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.SingleOrDefaultAsync(m => m.ProductId == id);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool UserExists(string userid)
        {
            return _context.Carts.Any(u => u.UserId == userid);
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.ProductId == id);
        }

        private bool ProductCartExist(int cartid, string usrid)
        {
            if (UserExists(usrid) && CartExists(cartid))
                return true;

            return false;
        }
    }
}