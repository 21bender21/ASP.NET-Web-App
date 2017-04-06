using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FPTake2.Models;
using Newtonsoft.Json;

namespace FPTake2.Controllers
{
    [Authorize]
    public class PurchaseRequestsController : Controller
    {
        private FinalProjectDBEntities1 db = new FinalProjectDBEntities1    ();

        // GET: PurchaseRequests
        public ActionResult Index()
        {
            var purchaseRequests = db.PurchaseRequests.Include(p => p.Product).Include(p => p.UserRequest);
            return View(purchaseRequests.ToList());
        }

        // GET: PurchaseRequests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequest purchaseRequest = db.PurchaseRequests.Find(id);
            if (purchaseRequest == null)
            {
                return HttpNotFound();
            }
            return View(purchaseRequest);
        }

        // GET: PurchaseRequests/Create
        public ActionResult Create()
        {
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.UserRequestID = new SelectList(db.UserRequests, "UserRequestID", "RequestorEmail");
            ViewBag.Products = db.Products.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PRViewModel requestItem)
        {
            
            if (ModelState.IsValid)
            {
                if(requestItem.RI.UserRequest.TotalCost <= 20)
                {
                    requestItem.RI.UserRequest.Status = "Approved";
                }
                else
                {
                    requestItem.RI.UserRequest.Status = "Pending";
                }
                requestItem.RI.UserRequest.RequestorEmail = User.Identity.Name != string.Empty ? User.Identity.Name : "No Email";
                
                requestItem.RI.UserRequest.DateRequested = DateTime.Now;
                var newPR = db.UserRequests.Add(requestItem.RI.UserRequest);
                db.SaveChanges();

                var cps = requestItem.Products;
                var prodItems = JsonConvert.DeserializeObject<List<ProductsJson>>(cps);
                int prodid = 0;
                int qty = 0;
                foreach (var prod in prodItems)
                {
                    prodid = int.Parse(prod.ProductID);
                        qty = int.Parse(prod.QtyNeeded);
                    db.PurchaseRequests.Add(new PurchaseRequest { UserRequestID = newPR.UserRequestID, ProductID = prodid, QtyNeeded = qty });
                    var qtyfix = from pd in db.Products where pd.ProductID == prodid select pd;
                    qtyfix.First().QtyInStock = qtyfix.First().QtyInStock - qty;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", requestItem.RI.ProductID);
            ViewBag.PurchaseRequestId = new SelectList(db.PurchaseRequests, "RequestId", "Status", requestItem.RI.UserRequestID);
            return View(requestItem);
        }

        // GET: RequestItems/Edit/5
       

        // POST: PurchaseRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "PurchaseRequestID,UserRequestID,ProductID,QtyNeeded")] PurchaseRequest purchaseRequest)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.PurchaseRequests.Add(purchaseRequest);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", purchaseRequest.ProductID);
        //    ViewBag.UserRequestID = new SelectList(db.UserRequests, "UserRequestID", "RequestorEmail", purchaseRequest.UserRequestID);
        //    return View(purchaseRequest);
        //}

        // GET: PurchaseRequests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequest purchaseRequest = db.PurchaseRequests.Find(id);
            if (purchaseRequest == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", purchaseRequest.ProductID);
            ViewBag.UserRequestID = new SelectList(db.UserRequests, "UserRequestID", "RequestorEmail", purchaseRequest.UserRequestID);
            return View(purchaseRequest);
        }

        // POST: PurchaseRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseRequestID,UserRequestID,ProductID,QtyNeeded")] PurchaseRequest purchaseRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchaseRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", purchaseRequest.ProductID);
            ViewBag.UserRequestID = new SelectList(db.UserRequests, "UserRequestID", "RequestorEmail", purchaseRequest.UserRequestID);
            return View(purchaseRequest);
        }

        // GET: PurchaseRequests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequest purchaseRequest = db.PurchaseRequests.Find(id);
            if (purchaseRequest == null)
            {
                return HttpNotFound();
            }
            return View(purchaseRequest);
        }

        // POST: PurchaseRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PurchaseRequest purchaseRequest = db.PurchaseRequests.Find(id);
            db.PurchaseRequests.Remove(purchaseRequest);
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
