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
    public class ZajeciaController : Controller
    {
        private PZEntities db = new PZEntities();

        // GET: Zajecia
        public ActionResult Index(int? id)
        {
            return RedirectToAction("Zajecia", "Przedmioty", new { id = id });
        }

        // GET: Zajecia/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zajecia zajecia = db.Zajecia.Find(id);
            if (zajecia == null)
            {
                return HttpNotFound();
            }
            return View(zajecia);
        }

        // GET: Zajecia/Create
        public ActionResult Create(int? id)
        {
            ViewBag.id_grupy = new SelectList(db.Grupy, "id", "nazwa");
            ICollection<Przedmioty> przedmioty = new List<Przedmioty>();
            przedmioty.Add(db.Przedmioty.Where(s => s.id == id).ToList().ElementAt(0));
            ViewBag.id_przedmiotu = new SelectList(przedmioty, "id", "nazwa");
            ViewBag.id_sali = new SelectList(db.Sale, "id", "nazwa");
            return View();
        }

        // POST: Zajecia/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_grupy,rozpoczecie,zakonczenie,id_przedmiotu,id_sali")] Zajecia zajecia)
        {
            int id_przedmiotu = zajecia.id_przedmiotu.Value;
            if (ModelState.IsValid)
            {
                int id_grupy = zajecia.id_grupy.Value;
                db.Zajecia.Add(zajecia);
                db.SaveChanges();
                foreach (var sg in db.StudenciGrupy.Where(s => s.id_grupy == id_grupy).ToList())
                {
                    Obecnosci obecnosci = new Obecnosci();
                    obecnosci.id_studenta = sg.id_studenta;
                    obecnosci.id_zajec = zajecia.id;
                    obecnosci.obecnosc = false;
                    obecnosci.usprawiedliwienie = false;
                    db.Obecnosci.Add(obecnosci);
                    db.SaveChanges();
                }
                return RedirectToAction("Zajecia", "Przedmioty", new { id = id_przedmiotu });
            }

            ViewBag.id_grupy = new SelectList(db.Grupy, "id", "nazwa", zajecia.id_grupy);
            ViewBag.id_przedmiotu = new SelectList(db.Przedmioty, "id", "nazwa", zajecia.id_przedmiotu);
            ViewBag.id_sali = new SelectList(db.Sale, "id", "nazwa", zajecia.id_sali);
            return RedirectToAction("Zajecia", "Przedmioty", new { id = id_przedmiotu });
        }

        // GET: Zajecia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zajecia zajecia = db.Zajecia.Find(id);
            if (zajecia == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_grupy = new SelectList(db.Grupy, "id", "nazwa", zajecia.id_grupy);
            ViewBag.id_przedmiotu = new SelectList(db.Przedmioty, "id", "nazwa", zajecia.id_przedmiotu);
            ViewBag.id_sali = new SelectList(db.Sale, "id", "nazwa", zajecia.id_sali);
            return View(zajecia);
        }

        // POST: Zajecia/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_grupy,rozpoczecie,zakonczenie,id_przedmiotu,id_sali")] Zajecia zajecia)
        {
            int id_przedmiotu = zajecia.id_przedmiotu.Value;
            if (ModelState.IsValid)
            {
                db.Entry(zajecia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Zajecia", "Przedmioty", new { id = id_przedmiotu });
            }
            ViewBag.id_grupy = new SelectList(db.Grupy, "id", "nazwa", zajecia.id_grupy);
            ViewBag.id_przedmiotu = new SelectList(db.Przedmioty, "id", "nazwa", zajecia.id_przedmiotu);
            ViewBag.id_sali = new SelectList(db.Sale, "id", "nazwa", zajecia.id_sali);
            return RedirectToAction("Zajecia", "Przedmioty", new { id = id_przedmiotu });
        }

        // GET: Zajecia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zajecia zajecia = db.Zajecia.Find(id);
            if (zajecia == null)
            {
                return HttpNotFound();
            }
            return View(zajecia);
        }

        // POST: Zajecia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Zajecia zajecia = db.Zajecia.Find(id);
            int id_przedmiotu = zajecia.id_przedmiotu.Value;
            int id_zajec = zajecia.id;
            foreach(var ob in db.Obecnosci.Where(s => s.id_zajec == id_zajec))
            {
                db.Obecnosci.Remove(ob);
            }
            db.Zajecia.Remove(zajecia);
            db.SaveChanges();
            return RedirectToAction("Zajecia", "Przedmioty", new { id = id_przedmiotu });
        }
        public ActionResult Back(int? id)
        {
            return RedirectToAction("Zajecia", "Przedmioty", new { id = id });
        }

        public ActionResult Obecnosci(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ICollection<Obecnosci> obecnosci = new List<Obecnosci>();
            List<Obecnosci> list = db.Obecnosci.Where(s => s.id_zajec == id).ToList();
            foreach (var z in list)
            {
                obecnosci.Add(z);
            }
            return View(obecnosci);
        }

        public ActionResult ObecnosciEdit(int? id)
        {
            return RedirectToAction("Edit", "Obecnosci", new { id = id });
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
