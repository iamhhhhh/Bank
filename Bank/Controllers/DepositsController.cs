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
    public class DepositsController : Controller
    {
        private BankEntities db = new BankEntities();

        // GET: Deposits
        public ActionResult Index()
        {
            var deposits = db.Deposits.Include(d => d.Account);
            return View(deposits.ToList());
        }

        // GET: Deposits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposit deposit = db.Deposits.Find(id);
            if (deposit == null)
            {
                return HttpNotFound();
            }
            return View(deposit);
        }

        // GET: Deposits/Create
        public ActionResult Create()
        {
            ViewBag.Depositnumber = new SelectList(db.Accounts, "AccountNumber", "AccountName");
            return View();
        }

        // POST: Deposits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepositID,Depositnumber,DepositAmount")] Deposit deposit)
        {
            if (ModelState.IsValid)
            {
                var account = db.Accounts.Where(x => x.AccountNumber == deposit.Depositnumber).FirstOrDefault();
                deposit.Account = account;
                deposit = Checkbalanceandgetdeposit(deposit);


                db.Deposits.Add(deposit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Depositnumber = new SelectList(db.Accounts, "AccountNumber", "AccountName", deposit.Depositnumber);
            return View(deposit);
        }

        public static Deposit Checkbalanceandgetdeposit(Deposit deposit)
        {
            deposit.DepositAmount = deposit.DepositAmount - (deposit.DepositAmount.GetValueOrDefault() * 1 / 1000);
            deposit.Account.Balance = deposit.Account.Balance + deposit.DepositAmount;

            return deposit;
        }
        // GET: Deposits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposit deposit = db.Deposits.Find(id);
            if (deposit == null)
            {
                return HttpNotFound();
            }
            ViewBag.Depositnumber = new SelectList(db.Accounts, "AccountNumber", "AccountName", deposit.Depositnumber);
            return View(deposit);
        }

        // POST: Deposits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepositID,Depositnumber,DepositAmount")] Deposit deposit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deposit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Depositnumber = new SelectList(db.Accounts, "AccountNumber", "AccountName", deposit.Depositnumber);
            return View(deposit);
        }

        // GET: Deposits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposit deposit = db.Deposits.Find(id);
            if (deposit == null)
            {
                return HttpNotFound();
            }
            return View(deposit);
        }

        // POST: Deposits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Deposit deposit = db.Deposits.Find(id);
            db.Deposits.Remove(deposit);
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
