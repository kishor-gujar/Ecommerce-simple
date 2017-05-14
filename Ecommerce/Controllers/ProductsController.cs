using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
	[Authorize]
	public class ProductsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IHostingEnvironment _hostingEnvironment;
		private readonly UserManager<ApplicationUser> _userManager;

		public ProductsController(ApplicationDbContext context, 
			UserManager<ApplicationUser> userManager,
			IHostingEnvironment hostingEnvironment)
		{
			_hostingEnvironment = hostingEnvironment;
			_userManager = userManager;
			_context = context;
		}

		// GET: Products
		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			var applicationDbContext = _context.Products.Include(p => p.ApplicationUser).Include(p => p.Category);
			return View(await applicationDbContext.ToListAsync());
		}

		// GET: Products/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
				return NotFound();

			var product = await _context.Products
				.Include(p => p.ApplicationUser)
				.Include(p => p.Category)
				.SingleOrDefaultAsync(m => m.Id == id);
			if (product == null)
				return NotFound();

			return View(product);
		}

		// GET: Products/Create
		public IActionResult Create()
		{
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
			ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
			return View();
		}

		// POST: Products/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(
		[Bind(
			"Id,Name,Description,QuantityPerUnit,UnitPrice,AvailabelSize,Size,Color,Dicount,UnitWeight,UnitsInStock,DiscountAvailabel,Picture,Note,UserId,CategoryId"
		)] Product product, IFormFile Picture)
		{
			var currntUser = await _userManager.GetUserAsync(User);

			if (Picture != null)
			{
				var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "products");

				Directory.CreateDirectory(Path.Combine(uploadPath, $"{currntUser.Id}{product.Name}"));

				var fileName = Path.GetFileName(Picture.FileName);

				using (
					var fileStream = new FileStream(
						Path.Combine(uploadPath, $"{currntUser.Id}{product.Name}", fileName), FileMode.Create))
				{
					await Picture.CopyToAsync(fileStream);
				}
				product.Picture = $"{currntUser.Id}{product.Name}/{fileName}";
				product.UserId = currntUser.Id;
			}

			if (ModelState.IsValid)
			{
				_context.Add(product);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", product.UserId);
			ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
			return View(product);
		}


		// GET: Products/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
				return NotFound();

			var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);
			if (product == null)
				return NotFound();
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", product.UserId);
			ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
			return View(product);
		}

		// POST: Products/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id,
		[Bind(
			"Id,Name,Description,QuantityPerUnit,UnitPrice,AvailabelSize,Size,Color,Dicount,UnitWeight,UnitsInStock,DiscountAvailabel,Picture,Note,UserId,CategoryId"
		)] Product product)
		{
			if (id != product.Id)
				return NotFound();

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(product);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProductExists(product.Id))
						return NotFound();
					throw;
				}
				return RedirectToAction("Index");
			}
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", product.UserId);
			ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
			return View(product);
		}

		// GET: Products/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
				return NotFound();

			var product = await _context.Products
				.Include(p => p.ApplicationUser)
				.Include(p => p.Category)
				.SingleOrDefaultAsync(m => m.Id == id);
			if (product == null)
				return NotFound();

			return View(product);
		}

		// POST: Products/Delete/5
		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);
			_context.Products.Remove(product);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		private bool ProductExists(int id)
		{
			return _context.Products.Any(e => e.Id == id);
		}
	}
}