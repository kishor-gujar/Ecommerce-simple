using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }


        // GET: Checkout/Create
        public async Task<IActionResult> Address()
        {
            var user = await _userManager.GetUserAsync(User);

            var address = await _context.Address.SingleOrDefaultAsync(e => e.UserId == user.Id);

            if (address!= null)
            {
                var id = address.Id;

                ViewData["Gender"] = new SelectList(Enum.GetNames(typeof(Gender)).ToList());
                ViewData["Country"] = new SelectList(Enum.GetNames(typeof(Country)).ToList());
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");

                return RedirectToAction("Edit",new {@id=id});
            }

            ViewData["Gender"] = new SelectList(Enum.GetNames(typeof(Gender)).ToList());
            ViewData["Country"] = new SelectList(Enum.GetNames(typeof(Country)).ToList());
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Checkout/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Address(
        [Bind(
            "Id,FirstName,LastName,MobileNo,Email,Company,Street,Number,Gender,Pincode,ShppingAddress,State,City,Country,UserId"
        )] Address address)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                address.UserId = user.Id;
                _context.Update(address);
                await _context.SaveChangesAsync();
                return RedirectToAction("PaymentType");
            }
            ViewData["Gender"] = new SelectList(Enum.GetNames(typeof(Gender)).ToList());
            ViewData["Country"] = new SelectList(Enum.GetNames(typeof(Country)).ToList());
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", address.UserId);
            return View("PaymentType");
        }

        // GET: Checkout/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Address.SingleOrDefaultAsync(m => m.Id == id);
            if (address == null)
            {
                return NotFound();
            }
            ViewData["Gender"] = new SelectList(Enum.GetNames(typeof(Gender)).ToList());
            ViewData["Country"] = new SelectList(Enum.GetNames(typeof(Country)).ToList());
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", address.UserId);
            return View(address);
        }

        // POST: Checkout/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
        [Bind(
            "Id,FirstName,LastName,MobileNo,Email,Company,Street,Number,Gender,Pincode,ShppingAddress,State,City,Country,UserId"
        )] Address address)
        {
            if (id != address.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    address.UserId = user.Id;
                    _context.Update(address);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("PaymentType");
            }
            ViewData["Gender"] = new SelectList(Enum.GetNames(typeof(Gender)).ToList());
            ViewData["Country"] = new SelectList(Enum.GetNames(typeof(Country)).ToList());
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", address.UserId);
            return View(address);
        }

        [HttpGet]
        public IActionResult PaymentType()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PaymentType([Bind("Id,Type,Allowed")] Payment payment)
        {
            if (!ModelState.IsValid) return View("Index");

            payment = new Payment()
            {
                Type = payment.Type
            };

            HttpContext.Session.SetString("PaymentType", $"{payment.Type}");

            return RedirectToAction("OrderOverview");
        }


        private bool AddressExists(int id)
        {
            return _context.Address.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> OrderOverview()
        {
            var userId = _userManager.GetUserId(User);

            var carts = from c in _context.Carts
                        where c.UserId == userId
                        select c;

            var applicationDbContext = carts.Include(c => c.Product);

            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> OrderOverview(Order order)
        {
            var user = await _userManager.GetUserAsync(User);

            ViewBag.PaymetnType = HttpContext.Session.GetString("PaymentType");

            var payment = new Payment()
            {
                Type = HttpContext.Session.GetString("PaymentType")
            };

            HttpContext.Session.Clear();

            order = new Order()
            {
                Payment = payment,
                Shipper = new Shipper(),
                Date = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                Number = RandomNumberGenerator.Create().GetHashCode(),
                UserId = user.Id,
                TimeStamp = DateTime.Now,
                Paid = false,
            };


            var carts = await (from c in _context.Carts
                               where c.UserId == user.Id
                               select c).ToListAsync();


            foreach (var cart in carts)
            {
                var orderDetail = new OrderDetail()
                {
                    ProductId = cart.ProductId,
                    Order = order,
                    BillDate = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    OrderNumber = RandomNumberGenerator.Create().GetHashCode(),
                    ShipDate = new DateTime().AddDays(7).ToString(CultureInfo.InvariantCulture),
                };
                _context.Add(orderDetail);
            }


            foreach (var cart in carts)
            {
                _context.Remove(cart);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "home");
        }
    }
}
