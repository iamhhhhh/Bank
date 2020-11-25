using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bank.Models;

namespace Bank.Controllers
{
    public class TransfersController : Controller
    {
        private BankEntities db = new BankEntities();

        // GET: Transfers
        public ActionResult Index()
        {
            var transfers = db.Transfers.Include(t => t.Account).Include(t => t.Account1);
            return View(transfers.ToList());
        }

        // GET: Transfers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transfer transfer = db.Transfers.Find(id);
            if (transfer == null)
            {
                return HttpNotFound();
            }
            return View(transfer);
        }

        // GET: Transfers/Create
        public ActionResult Create()
        {
            ViewBag.TransferFrom = new SelectList(db.Accounts, "AccountNumber", "AccountName");
            ViewBag.TransferTo = new SelectList(db.Accounts, "AccountNumber", "AccountName");
            return View();
        }

        // POST: Transfers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransferID,TransferFrom,TranferMoney,TransferTo")] Transfer transfer)
        {
            if (ModelState.IsValid)
            {
                var from = db.Accounts.Where(x => x.AccountNumber == transfer.TransferFrom).FirstOrDefault();
                var To = db.Accounts.Where(x => x.AccountNumber == transfer.TransferTo).FirstOrDefault();
                transfer.Account = from;
                transfer.Account1 = To;
                if (CheckBalance(transfer))
                {
                    transfer = updatebalance(transfer);
                    db.Transfers.Add(transfer);
                    db.SaveChanges();
                }
                else
                {
                 
                }
                return RedirectToAction("Index");

            }

            ViewBag.TransferFrom = new SelectList(db.Accounts, "AccountNumber", "AccountName", transfer.TransferFrom);
            ViewBag.TransferTo = new SelectList(db.Accounts, "AccountNumber", "AccountName", transfer.TransferTo);
            return View(transfer);
        }
        public static Transfer updatebalance(Transfer transfer)
        {
            transfer.Account.Balance = transfer.Account.Balance - transfer.TranferMoney;
            transfer.Account1.Balance = transfer.Account1.Balance + transfer.TranferMoney;
            return transfer;
        }
        public static Boolean CheckBalance(Transfer transfer)
        {
            if (transfer.Account.Balance < transfer.TranferMoney || transfer.TranferMoney < 0)
            {
                return false;
            }
            else
            { 
                return true;
            }
        }
        // GET: Transfers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transfer transfer = db.Transfers.Find(id);
            if (transfer == null)
            {
                return HttpNotFound();
            }
            ViewBag.TransferFrom = new SelectList(db.Accounts, "AccountNumber", "AccountName", transfer.TransferFrom);
            ViewBag.TransferTo = new SelectList(db.Accounts, "AccountNumber", "AccountName", transfer.TransferTo);
            return View(transfer);
        }

        // POST: Transfers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TransferID,TransferFrom,TranferMoney,TransferTo")] Transfer transfer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transfer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TransferFrom = new SelectList(db.Accounts, "AccountNumber", "AccountName", transfer.TransferFrom);
            ViewBag.TransferTo = new SelectList(db.Accounts, "AccountNumber", "AccountName", transfer.TransferTo);
            return View(transfer);
        }

        // GET: Transfers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transfer transfer = db.Transfers.Find(id);
            if (transfer == null)
            {
                return HttpNotFound();
            }
            return View(transfer);
        }

        // POST: Transfers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transfer transfer = db.Transfers.Find(id);
            db.Transfers.Remove(transfer);
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
