using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class WeddingController : Controller
    {
        public WeddingController(WeddingPlannerContext context) : base() { _db = context; }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            if (!_IsLoggedIn) { return RedirectToAction("Index", "Home"); }

            List<Wedding> AllWeddings = _db.Weddings
                .Include(wedding => wedding.Rsvps)
                .Include(wedding => wedding.WeddingCreator)
                .ToList();

            return View("Dashboard", AllWeddings);
        }

        [HttpGet("weddings/{weddingId}")]
        public IActionResult Details(int weddingId)
        {
            if (!_IsLoggedIn) { return RedirectToAction("Index", "Home"); }

            Wedding dbWedding = _db.Weddings
                .Include(wedding => wedding.Rsvps)
                    .ThenInclude(rsvp => rsvp.UserRsvp)
                .FirstOrDefault(wedding => wedding.WeddingId == weddingId);
            
            if (dbWedding == null) { return RedirectToAction("Dashboard"); }

            return View("Details", dbWedding);
        }

        [HttpGet("weddings/new")]
        public IActionResult New()
        {
            if (!_IsLoggedIn) { return RedirectToAction("Index", "Home"); }
            return View("New");
        }

        [HttpPost("weddings/create")]
        public IActionResult Create(Wedding newWedding)
        {
            if (!_IsLoggedIn) { return RedirectToAction("Index", "Home"); }
            if (!ModelState.IsValid) { return View("New"); }

            User dbUser = _db.Users
                .Include(user => user.Weddings)
                .FirstOrDefault(user => user.UserId == _UserId);

            newWedding.UserId = (int)_UserId;
            newWedding.WeddingCreator = dbUser;
            dbUser.Weddings.Add(newWedding);

            _db.Weddings.Add(newWedding);
            _db.SaveChanges();

            return RedirectToAction("Details", new { weddingId = newWedding.WeddingId });
        }

        // Do I need to remove the RSPVs for this wedding?
        [HttpPost("weddings/{weddingId}/delete")]
        public IActionResult Delete(int weddingId)
        {
            if (_IsLoggedIn)
            {
                Wedding dbWedding = _db.Weddings
                    .Include(wedding => wedding.Rsvps)
                    .FirstOrDefault(wedding => wedding.WeddingId == weddingId);
                User dbUser = _db.Users
                    .FirstOrDefault(user => user.UserId == _UserId);

                if (_UserId == dbWedding.UserId && dbWedding != null)
                {
                    dbUser.Weddings.RemoveAll(wedding => wedding.WeddingId == weddingId);
                    _db.Weddings.Remove(dbWedding);
                    _db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("weddings/{weddingId}/rsvp")]
        public IActionResult Rsvp(int weddingId)
        {
            if (!_IsLoggedIn) { return RedirectToAction("Index", "Home"); }

            Wedding dbWedding = _db.Weddings
                .Include(wedding => wedding.Rsvps)
                .FirstOrDefault(wedding => wedding.WeddingId == weddingId);
            User dbUser = _db.Users
                .Include(user => user.Rsvps)
                .FirstOrDefault(user => user.UserId == _UserId);

            bool isLoggedInUsersWedding = dbWedding.UserId == _UserId;

            if (!isLoggedInUsersWedding)
            {
                Rsvp dbRsvp = _db.Rsvps
                    .FirstOrDefault(rsvp => rsvp.WeddingId == weddingId && rsvp.UserId == _UserId);
                if (dbRsvp == null)
                {
                    dbRsvp = new Rsvp();
                    dbRsvp.WeddingId = weddingId;
                    dbRsvp.WeddingRsvp = dbWedding;
                    dbRsvp.UserId = (int)_UserId;
                    dbRsvp.UserRsvp = _db.Users.FirstOrDefault(user => user.UserId == _UserId);

                    dbWedding.Rsvps.Add(dbRsvp);
                    dbUser.Rsvps.Add(dbRsvp);
                    _db.Rsvps.Add(dbRsvp);
                }
                else
                {
                    dbWedding.Rsvps.Remove(dbRsvp);
                    dbUser.Rsvps.Remove(dbRsvp);
                    _db.Rsvps.Remove(dbRsvp);
                }

                _db.SaveChanges();
            }

            return RedirectToAction("Dashboard");
        }


        private WeddingPlannerContext _db;
        private int? _UserId { get => HttpContext.Session.GetInt32("UserId"); }
        private bool _IsLoggedIn
        {
            get => _UserId != null; 
        }
    }
}