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
    public class TransactionController : Controller
    {
        private PerspectiveEntities db = new PerspectiveEntities();

        // GET: Transaction
        public ActionResult Index(string sortOrder, string searchString)
        {
            //For showing 'no result' message for searching
            ViewBag.ShowMsg = "hidden";

            var transactions = from t in db.Transactions.Include(t => t.Trader)
                               select t;
            
            //SORTING BEGINS
            ViewBag.SortByDate = String.IsNullOrEmpty(sortOrder) ? "OldestFirst" : "";
            ViewBag.SortByName = sortOrder == "ByName" ? "ByNameDesc" : "ByName";
            switch (sortOrder)
            {
                case "ByNameDesc":
                    transactions = transactions.OrderByDescending(t => t.Trader.Name);
                    break;
                case "OldestFirst":
                    transactions = transactions.OrderBy(t => t.Date);
                    break;
                case "ByName":
                    transactions = transactions.OrderBy(t => t.Trader.Name);
                    break;
                //By default they are sorted by descending dates which shows the newest transaction first
                default:
                    transactions = transactions.OrderByDescending(t => t.Date);
                    break;
            }
            //SORTING ENDS

            //SEARCHING BEGINS
            if (!String.IsNullOrEmpty(searchString))
            {
                transactions = transactions.Where(t => t.Id.ToString() == searchString);
                //For showing 'no result' message for searching
                if (!transactions.Any())
                {
                    ViewBag.SearchResultNullMsg = "There is no record for transaction number: " + searchString;
                    ViewBag.ShowMsg = "";
                    ViewBag.ShowTbl = "hidden";
                }
            }
            //SEARCHING ENDS

            return View(transactions.ToList());
        }

        // GET: Transaction/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transaction/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.TraderId = new SelectList(db.Traders, "Id", "Name", transaction.TraderId);
            return View(transaction);
        }

        // POST: Transaction/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TraderId,Receivable,Payable,Date,Description")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TraderId = new SelectList(db.Traders, "Id", "Name", transaction.TraderId);
            return View(transaction);
        }

        // GET: Transaction/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //REPORT
        public ActionResult Report()
        {
            var transactions = db.Transactions.Include(t => t.Trader);
            return View(transactions.ToList());
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
