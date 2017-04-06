using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FPTake2.Models;

namespace FPTake2.Controllers
{
    [Authorize]
    public class UserRequestsController : Controller
    {
        private FinalProjectDBEntities1 db = new FinalProjectDBEntities1();

        // GET: UserRequests
        public ActionResult Index()
        {
            return View(db.UserRequests.ToList());
        }
        [Authorize(Roles ="Manager")]
        public ActionResult ManagerApproval()
        {
            

            var needActn = from u in db.UserRequests where (u.Status == "Pending") && (u.TotalCost > 20) select u;
            return View(needActn.ToList());
        }

        // GET: UserRequests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRequest userRequest = db.UserRequests.Find(id);
            if (userRequest == null)
            {
                return HttpNotFound();
            }
            return View(userRequest);
        }

        // GET: UserRequests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserRequestID,DateRequested,RequestorEmail,Status,RequestDescription,DateNeededBy,DeliveryType,TotalCost")] UserRequest userRequest)
        {
            if (ModelState.IsValid)
            {
                db.UserRequests.Add(userRequest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userRequest);
        }

        // GET: UserRequests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRequest userRequest = db.UserRequests.Find(id);
            if (userRequest == null)
            {
                return HttpNotFound();
            }
            return View(userRequest);
        }

        // POST: UserRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserRequestID,DateRequested,RequestorEmail,Status,RequestDescription,DateNeededBy,DeliveryType,TotalCost")] UserRequest userRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userRequest);
        }

        // GET: UserRequests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRequest userRequest = db.UserRequests.Find(id);
            if (userRequest == null)
            {
                return HttpNotFound();
            }
            return View(userRequest);
        }

        // POST: UserRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserRequest userRequest = db.UserRequests.Find(id);
            db.UserRequests.Remove(userRequest);
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
