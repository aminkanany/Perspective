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
    public class ReceivableController : Controller
    {
        private PerspectiveEntities db = new PerspectiveEntities();

        // GET: Receivable
        public ActionResult Index(string sortOrder)
        {
            ViewBag.SortByDate = String.IsNullOrEmpty(sortOrder) ? "OldestFirst" : "";
            ViewBag.SortByName = sortOrder == "ByName" ? "ByNameDesc" : "ByName";
            var transactions = from r in db.Transactions.Include(t => t.Trader)
                               select r;
            transactions = transactions.Where(x => x.Payable == 0);

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

            return View(transactions.ToList());
        }

        // GET: Receivable/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error");
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            return View(transaction);
        }

        // GET: Receivable/Create
        public ActionResult Create()
        {
            ViewBag.TraderId = new SelectList(db.Traders, "Id", "Name");
            return View();
        }

        // POST: Receivable/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TraderId,Receivable,Payable,Date,Description")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TraderId = new SelectList(db.Traders, "Id", "Name", transaction.TraderId);
            return View(transaction);
        }

        // GET: Receivable/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error");
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            ViewBag.TraderId = new SelectList(db.Traders, "Id", "Name", transaction.TraderId);
            return View(transaction);
        }

        // POST: Receivable/Edit/5
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

        // GET: Receivable/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error");
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return RedirectToAction("BadRequest", "Error");
            }
            return View(transaction);
        }

        // POST: Receivable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
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
