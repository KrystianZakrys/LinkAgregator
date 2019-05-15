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

//TODO 3 = Zabezpieczenie linków 
//TODO 4 = Komunikaty

namespace LinkAgregator.Controllers
{
    public class LinksController : Controller
    {
        #region constants
        private const string MESSAGE_TITLE_SUCCESS = "Success";
        private const string MESSAGE_TITLE_FAILURE = "Failure";
        private const string MESSAGE_TEXT_VALIDATION_INVALID_LINK_FORMAT = "Invalid link format.";
        private const string MESSAGE_TEXT_CREATE_LINK_SUCCESS = "Congratulation! Your link has been created.";
        private const string MESSAGE_TEXT_UPDATE_LINK_SUCCESS = "Congratulation! Your link has been updated.";
        private const string MESSAGE_TEXT_UPDATE_LINK_FAILURE = "Your link has not been updated.";
        private const string MESSAGE_TEXT_DELETE_LINK_SUCCESS = "Congratulation! Your link has been deleted.";
        #endregion

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
            return View(await _context.Links.OrderByDescending(x => x.Rate).Where(x => DateTime.Now.Subtract(x.Date).Days <= 5).ToListAsync());
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
            ViewBag.MessageTitle = TempData["MessageTitle"];
            ViewBag.MessageText = TempData["MessageText"];
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
                if (!IsValidUri(link.Url))
                {
                    TempData["MessageTitle"] = MESSAGE_TITLE_FAILURE;
                    TempData["MessageText"] = MESSAGE_TEXT_VALIDATION_INVALID_LINK_FORMAT;
                    return RedirectToAction(nameof(Create));
                }
                link.Id = Guid.NewGuid();
                link.userId = new Guid(_userManager.GetUserId(this.HttpContext.User));
                link.Date = DateTime.Now;
                _context.Add(link);
                await _context.SaveChangesAsync();
                TempData["MessageTitle"] = MESSAGE_TITLE_SUCCESS;
                TempData["MessageText"] = MESSAGE_TEXT_CREATE_LINK_SUCCESS;
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Url,Rate,userId,Date")] Link link)
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
                TempData["MessageTitle"] = MESSAGE_TITLE_SUCCESS;
                TempData["MessageText"] = MESSAGE_TEXT_UPDATE_LINK_SUCCESS;
                return RedirectToAction(nameof(Index));
            }
            TempData["MessageTitle"] = MESSAGE_TITLE_FAILURE;
            TempData["MessageText"] = MESSAGE_TEXT_UPDATE_LINK_FAILURE;
           
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
            TempData["MessageTitle"] = MESSAGE_TITLE_SUCCESS;
            TempData["MessageText"] = MESSAGE_TEXT_DELETE_LINK_SUCCESS;
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
        /// <summary>
        /// Gets logged user Id
        /// </summary>
        /// <returns>logged user Id</returns>
        private string GetUserIdString()
        {
            var userId = String.IsNullOrEmpty(_userManager.GetUserId(this.HttpContext.User)) ? Guid.Empty : new Guid(_userManager.GetUserId(this.HttpContext.User));
            return userId.ToString();
        }
        /// <summary>
        /// Validates url
        /// </summary>
        /// <param name="uri">url to add</param>
        /// <returns>true if url is valid</returns>
        public bool IsValidUri(string uri)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(uri, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }
    }
}
