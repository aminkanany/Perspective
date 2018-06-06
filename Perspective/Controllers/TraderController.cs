using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Perspective.Models;

namespace Perspective.Controllers
{
    public class TraderController : Controller
    {
        private PerspectiveEntities db = new PerspectiveEntities();

        // GET: Trader
        public ActionResult Index(string sortOrder, string searchString)
        {
            //For showing 'no result' message for searching
            ViewBag.ShowMsg = "hidden";

            var traders = from t in db.Traders
                          select t;

            //SORTING BEGINS
            ViewBag.SortByName = String.IsNullOrEmpty(sortOrder) ? "ByNameDesc" : "";
            ViewBag.SortByCompany = sortOrder == "ByCompany" ? "ByCompanyDesc" : "ByCompany";
            switch (sortOrder)
            {
                case "ByNameDesc":
                    traders = traders.OrderByDescending(t => t.Name);
                    break;
                case "ByCompany":
                    traders = traders.OrderBy(t => t.Company);
                    break;
                case "ByCompanyDesc":
                    traders = traders.OrderByDescending(t => t.Company);
                    break;
                //By default they are sorted by name
                default:
                    traders = traders.OrderBy(t => t.Name);
                    break;
            }
            //SORTING ENDS

            //SEARCHING BEGINS
            if (!String.IsNullOrEmpty(searchString))
            {
                traders = traders.Where(t => t.Name.Contains(searchString) || t.Company.Contains(searchString));
                //For showing 'no result' message for searching
                if (!traders.Any())
                {
                    ViewBag.SearchResultNullMsg = "There is no record for trader: " + searchString;
                    ViewBag.ShowMsg = "";
                    ViewBag.ShowTbl = "hidden";
                }
            }
            //SEARCHING ENDS

            return View(traders.ToList());
        }

        // GET: Trader/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error");
            }
            Trader trader = db.Traders.Find(id);
            if (trader == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            return View(trader);
        }

        // GET: Trader/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Trader/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Company,Phone,Address")] Trader trader)
        {
            if (ModelState.IsValid)
            {
                db.Traders.Add(trader);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trader);
        }

        // GET: Trader/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error");

            }
            Trader trader = db.Traders.Find(id);
            if (trader == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            return View(trader);
        }

        // POST: Trader/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Company,Phone,Address")] Trader trader)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trader).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trader);
        }

        // GET: Trader/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error");
            }
            Trader trader = db.Traders.Find(id);
            if (trader == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            return View(trader);
        }

        // POST: Trader/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int numAssociatedTransactions = 0;
            TempData["DeletionError"] = false;

            try
            {
                //Get trader record and find out if there are associated transactions
                Trader trader = db.Traders.Find(id);
                numAssociatedTransactions = trader.Transactions.Count();
                //Only allow delete if there are no transactions associated with transaction
                if (numAssociatedTransactions == 0)
                {
                    db.Traders.Remove(trader);
                    db.SaveChanges();
                }
                else
                {
                    TempData["MyErrorMessage"] = "You cannot delete this trader while there are transactions linked to them.";
                    TempData["DeletionError"] = true;
                    return RedirectToAction("Delete", new { id = id });
                }

            }
            catch (Exception)
            {

                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
