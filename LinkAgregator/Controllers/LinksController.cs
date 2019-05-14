using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LinkAgregator.Data;
using LinkAgregator.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

//TODO 4 = Komunikaty
//TODO 3 = Zabezpieczenie linków żeby skryptów się nie dało dodać


namespace LinkAgregator.Controllers
{
    public class LinksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;
        
        public LinksController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Links
        public async Task<IActionResult> Index()
        {
            ViewBag.userId = GetUserIdString();
            ViewBag.MessageTitle = TempData["MessageTitle"];
            ViewBag.MessageText =  TempData["MessageText"];
            return View(await _context.Links.ToListAsync());
        }

        // GET: MyLinks
        public async Task<IActionResult> UserLinks()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.userId = GetUserIdString();
                var userId = new Guid(_userManager.GetUserId(this.HttpContext.User));
                return View("Index",await _context.Links.Where(x => x.userId.Equals(userId)).ToListAsync());
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Links/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var link = await _context.Links
                .FirstOrDefaultAsync(m => m.Id == id);
            if (link == null)
            {
                return NotFound();
            }

            return View(link);
        }

        // GET: Links/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Links/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Url")] Link link)
        {
            if (ModelState.IsValid)
            {
                link.Id = Guid.NewGuid();
                link.userId = new Guid(_userManager.GetUserId(this.HttpContext.User));
                _context.Add(link);
                await _context.SaveChangesAsync();
                TempData["MessageTitle"] = "Success";
                TempData["MessageText"] = "Congratulation! Your link has been created.";
                return RedirectToAction(nameof(Index));
            }

            return View(link);
        }

        // GET: Links/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var link = await _context.Links.FindAsync(id);
            if (link == null)
            {
                return NotFound();
            }
            return View(link);
        }

        // POST: Links/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Url,Rate,userId")] Link link)
        {
            if (id != link.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(link);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LinkExists(link.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["MessageTitle"] = "Success";
                TempData["MessageText"] = "Congratulation! Your link has been updated.";
                return RedirectToAction(nameof(Index));
            }
            TempData["MessageTitle"] = "Failure";
            TempData["MessageText"] = "Your link has not been updated.";
           
            return View(link);
        }

        // GET: Links/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var link = await _context.Links
                .FirstOrDefaultAsync(m => m.Id == id);
            if (link == null)
            {
                return NotFound();
            }
         
            return View(link);
        }

        // POST: Links/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var link = await _context.Links.FindAsync(id);
            _context.Links.Remove(link);
            await _context.SaveChangesAsync();
            TempData["MessageTitle"] = "Success";
            TempData["MessageText"] = "Congratulation! Your link has been deleted.";
            return RedirectToAction(nameof(Index));
        }

        //GET: Links/AddVote/5
        public async Task<IActionResult> AddVote(Guid id)
        {
            var link = await _context.Links.FindAsync(id);
            link.Rate += 1;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LinkExists(Guid id)
        {
            return _context.Links.Any(e => e.Id == id);
        }

        private string GetUserIdString()
        {
            var userId = String.IsNullOrEmpty(_userManager.GetUserId(this.HttpContext.User)) ? Guid.Empty : new Guid(_userManager.GetUserId(this.HttpContext.User));
            return userId.ToString();
        }
    }
}
