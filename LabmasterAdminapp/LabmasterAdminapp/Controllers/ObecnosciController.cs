using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LabmasterAdminapp.Models;

namespace LabmasterAdminapp.Controllers
{
    public class ObecnosciController : Controller
    {
        private PZEntities db = new PZEntities();

        // GET: Obecnosci
        public ActionResult Index(int? id)
        {
            return RedirectToAction("Zajecia", "Przedmioty", new { id = id });
        }

        // GET: Obecnosci/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Obecnosci obecnosci = db.Obecnosci.Find(id);
            if (obecnosci == null)
            {
                return HttpNotFound();
            }
            return View(obecnosci);
        }

        // GET: Obecnosci/Create
        public ActionResult Create()
        {
            ViewBag.id_studenta = new SelectList(db.Studenci, "indeks", "imie");
            ViewBag.id_zajec = new SelectList(db.Zajecia, "id", "id");
            return View();
        }

        // POST: Obecnosci/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_studenta,id_zajec,obecnosc,usprawiedliwienie")] Obecnosci obecnosci)
        {
            if (ModelState.IsValid)
            {
                db.Obecnosci.Add(obecnosci);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_studenta = new SelectList(db.Studenci, "indeks", "imie", obecnosci.id_studenta);
            ViewBag.id_zajec = new SelectList(db.Zajecia, "id", "id", obecnosci.id_zajec);
            return View(obecnosci);
        }

        // GET: Obecnosci/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Obecnosci obecnosci = db.Obecnosci.Find(id);
            if (obecnosci == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_studenta = new SelectList(db.Studenci, "indeks", "imie", obecnosci.id_studenta);
            ViewBag.id_zajec = new SelectList(db.Zajecia, "id", "id", obecnosci.id_zajec);
            return View(obecnosci);
        }

        // POST: Obecnosci/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_studenta,id_zajec,obecnosc,usprawiedliwienie")] Obecnosci obecnosci)
        {
            var id_zajec = db.Zajecia.Find(obecnosci.id_zajec).id;
            if (ModelState.IsValid)
            {
                db.Entry(obecnosci).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Obecnosci", "Zajecia", new { id = id_zajec });
            }
            ViewBag.id_studenta = new SelectList(db.Studenci, "indeks", "imie", obecnosci.id_studenta);
            ViewBag.id_zajec = new SelectList(db.Zajecia, "id", "id", obecnosci.id_zajec);
            return View(obecnosci);
        }

        // GET: Obecnosci/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Obecnosci obecnosci = db.Obecnosci.Find(id);
            if (obecnosci == null)
            {
                return HttpNotFound();
            }
            return View(obecnosci);
        }

        // POST: Obecnosci/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Obecnosci obecnosci = db.Obecnosci.Find(id);
            db.Obecnosci.Remove(obecnosci);
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
