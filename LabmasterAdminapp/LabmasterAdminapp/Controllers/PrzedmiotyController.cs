using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using LabmasterAdminapp.Models;

namespace LabmasterAdminapp.Controllers
{
    public class PrzedmiotyController : Controller
    {
        private PZEntities db = new PZEntities();

        // GET: Przedmioty
        public ICollection<PrzedmiotObecnosci> getObecnosci(int id)
        {
            ICollection<PrzedmiotObecnosci> przedmiotobecnosci = new List<PrzedmiotObecnosci>();
            ICollection<Zajecia> zajecia = db.Zajecia.Where(s => s.id_przedmiotu == id).ToList();
            var grupy = zajecia.Select(s => s.Grupy).ToList();
            ICollection<StudenciGrupy> studencigrupy = new List<StudenciGrupy>();

            foreach (Grupy g in grupy)
            {
                foreach (StudenciGrupy sg in db.StudenciGrupy.Where(s => s.id_grupy == g.id))
                {
                    studencigrupy.Add(sg);
                }
            }

            ICollection<Studenci> studenci = studencigrupy.Select(s => s.Studenci).Distinct().ToList();
            ICollection<Obecnosci> obecnosci = new List<Obecnosci>();
            foreach (Zajecia z in zajecia)
            {
                ICollection<Obecnosci> ob = db.Obecnosci.Where(s => s.id_zajec == z.id).ToList();
                foreach (Obecnosci o in ob)
                {
                    obecnosci.Add(o);
                }
            }
            foreach (Studenci s in studenci)
            {
                PrzedmiotObecnosci przedmiotobecnosc = new PrzedmiotObecnosci();
                przedmiotobecnosc.student_id = s.indeks;
                przedmiotobecnosc.student_imie = s.imie;
                przedmiotobecnosc.student_nazwisko = s.nazwisko;
                przedmiotobecnosc.przedmiot_id = id;
                przedmiotobecnosc.przedmiot_nazwa = db.Przedmioty.Find(id).nazwa;
                przedmiotobecnosc.grupa = db.Przedmioty.Find(id).Zajecia.Where(x => x.Grupy.StudenciGrupy.Where(y => y.id_studenta == s.indeks).ToList().Count > 0).ToList().ElementAt(0).Grupy.nazwa;
                var ob = obecnosci.Where(x => x.id_studenta == s.indeks).Where(y => y.Zajecia.zakonczenie < DateTime.Now);
                przedmiotobecnosc.liczba_zajec = ob.ToList().Count;
                przedmiotobecnosc.udzial = ob.Where(x => x.obecnosc == true).ToList().Count + ob.Where(x => x.usprawiedliwienie == true).ToList().Count;
                przedmiotobecnosci.Add(przedmiotobecnosc);
            }
            return przedmiotobecnosci;
        }
        public ActionResult Index()
        {
            var user = User.Identity.Name;
            int id = db.Prowadzacy.Where(s => s.adname == user).ToList().ElementAt(0).id;
            var przedmioty = db.Przedmioty.Where(s => s.id_prowadzacego == id);
            przedmioty = przedmioty.Include(p => p.Prowadzacy);
            return View(przedmioty.ToList());
        }

        public ActionResult Obecnosci(int? id)
        {
            return View(getObecnosci(id ?? default(int)));
        }

        public ActionResult Export(int? id)
        {
            var gv = new GridView();
            gv.DataSource = this.getObecnosci(id ?? default(int));
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename="+db.Przedmioty.Find(id).nazwa+".xls");
            Response.ContentType = "application/ms-excel";
            Response.ContentEncoding = System.Text.Encoding.Unicode;
            Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("Obecnosci", new { id = id });
        }

        // GET: Przedmioty/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Przedmioty przedmioty = db.Przedmioty.Find(id);
            if (przedmioty == null)
            {
                return HttpNotFound();
            }
            return View(przedmioty);
        }

        // GET: Przedmioty/Create
        public ActionResult Create()
        {
            var user = User.Identity.Name;
            ICollection<Prowadzacy> prowadzacy = new List<Prowadzacy>();
            prowadzacy.Add(db.Prowadzacy.Where(s => s.adname == user).ToList().ElementAt(0));
            ViewBag.id_prowadzacego = new SelectList(prowadzacy, "id", "imie");
            return View();
        }

        public ActionResult Zajecia(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ICollection<Zajecia> zajecia = new List<Zajecia>();
            List<Zajecia> list = db.Zajecia.Where(s => s.id_przedmiotu == id).ToList();
            foreach(var z in list) {
                zajecia.Add(z);
            }
            return View(zajecia);
        }

        // POST: Przedmioty/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_prowadzacego,nazwa")] Przedmioty przedmioty)
        {
            if (ModelState.IsValid)
            {
                db.Przedmioty.Add(przedmioty);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_prowadzacego = new SelectList(db.Prowadzacy, "id", "imie", przedmioty.id_prowadzacego);
            return View(przedmioty);
        }

        // GET: Przedmioty/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Przedmioty przedmioty = db.Przedmioty.Find(id);
            if (przedmioty == null)
            {
                return HttpNotFound();
            }
            var user = User.Identity.Name;
            ICollection<Prowadzacy> prowadzacy = new List<Prowadzacy>();
            prowadzacy.Add(db.Prowadzacy.Where(s => s.adname == user).ToList().ElementAt(0));
            ViewBag.id_prowadzacego = new SelectList(prowadzacy, "id", "imie", przedmioty.id_prowadzacego);
            return View(przedmioty);
        }

        // POST: Przedmioty/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_prowadzacego,nazwa")] Przedmioty przedmioty)
        {
            if (ModelState.IsValid)
            {
                db.Entry(przedmioty).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_prowadzacego = new SelectList(db.Prowadzacy, "id", "imie", przedmioty.id_prowadzacego);
            return View(przedmioty);
        }

        // GET: Przedmioty/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Przedmioty przedmioty = db.Przedmioty.Find(id);
            if (przedmioty == null)
            {
                return HttpNotFound();
            }
            return View(przedmioty);
        }

        // POST: Przedmioty/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Przedmioty przedmioty = db.Przedmioty.Find(id);
            db.Przedmioty.Remove(przedmioty);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ZajeciaCreate(int? id)
        {
            return RedirectToAction("Create", "Zajecia", new { id = id });
        }
        public ActionResult ZajeciaDelete(int? id)
        {
            return RedirectToAction("Delete", "Zajecia", new { id = id });
        }
        public ActionResult ZajeciaEdit(int? id)
        {
            return RedirectToAction("Edit", "Zajecia", new { id = id });
        }
        public ActionResult ZajeciaObecnosci(int? id)
        {
            return RedirectToAction("Obecnosci", "Zajecia", new { id = id });
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
