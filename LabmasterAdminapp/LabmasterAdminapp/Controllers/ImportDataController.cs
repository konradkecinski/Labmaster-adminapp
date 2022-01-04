using LabmasterAdminapp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LabmasterAdminapp.Controllers
{
    public class ImportDataController : Controller
    {
        private PZEntities db = new PZEntities();
        // GET: ImportData
        public ActionResult Index()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, string name)

        {

            string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);

            string filepath = "/excelfolder/" + filename;

            file.SaveAs(Path.Combine(Server.MapPath("/excelfolder"), filename));

            this.InsertStudentsToGroups(filepath, filename, name);


            ViewBag.Message = "Dodano";
            return View();

        }

        private void InsertStudentsToGroups(string filepath, string filename, string groupname)
        {
            string fullpath = Server.MapPath("/excelfolder/") + filename;

            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", fullpath);

            var Econ = new OleDbConnection(constr);

            string query = string.Format("Select * from [{0}]", "Studenci$");

            OleDbCommand Ecom = new OleDbCommand(query, Econ);

            Econ.Open();



            DataSet ds = new DataSet();

            OleDbDataAdapter oda = new OleDbDataAdapter(query, Econ);

            Econ.Close();

            oda.Fill(ds);

            if(db.Grupy.Where(s => s.nazwa == groupname).Count() == 0)
            {
                Grupy gr = new Grupy();
                gr.nazwa = groupname;
                db.Grupy.Add(gr);
                db.SaveChanges();
            }

            int group_id = db.Grupy.Where(s => s.nazwa == groupname).ToList().ElementAt(0).id;

            DataTable dt = ds.Tables[0];

            foreach (DataRow x in dt.Rows)
            {
                if (x.ItemArray.Length == 3)
                {
                    Studenci st = new Studenci();
                    st.indeks = Int32.Parse(x.ItemArray.ElementAt(0).ToString());
                    st.imie = x.ItemArray.ElementAt(1).ToString();
                    st.nazwisko = x.ItemArray.ElementAt(2).ToString();
                    if(db.Studenci.Where(s => s.indeks == st.indeks).Count() == 0)
                    {
                        db.Studenci.Add(st);
                        db.SaveChanges();
                    }

                    if(db.StudenciGrupy.Where(s => (s.id_studenta == st.indeks) && (s.id_grupy == group_id)).Count() == 0)
                    {
                        StudenciGrupy sg = new StudenciGrupy();
                        sg.id_grupy = group_id;
                        sg.id_studenta = st.indeks;
                        db.StudenciGrupy.Add(sg);
                        db.SaveChanges();
                    }
                }
                if (x.ItemArray.Length == 4)
                {
                    Studenci st = new Studenci();
                    st.indeks = Int32.Parse(x.ItemArray.ElementAt(0).ToString());
                    st.imie = x.ItemArray.ElementAt(1).ToString();
                    st.nazwisko = x.ItemArray.ElementAt(2).ToString();
                    st.mail = x.ItemArray.ElementAt(3).ToString();
                    if (db.Studenci.Where(s => s.indeks == st.indeks).Count() == 0)
                    {
                        db.Studenci.Add(st);
                        db.SaveChanges();
                    }
                    if (db.StudenciGrupy.Where(s => (s.id_studenta == st.indeks) && (s.id_grupy == group_id)).Count() == 0)
                    {
                        StudenciGrupy sg = new StudenciGrupy();
                        sg.id_grupy = group_id;
                        sg.id_studenta = st.indeks;
                        db.StudenciGrupy.Add(sg);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}