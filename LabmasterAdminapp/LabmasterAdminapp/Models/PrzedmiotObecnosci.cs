using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabmasterAdminapp.Models
{
    public class PrzedmiotObecnosci
    {
        public int id { get; set; }
        public int student_id { get; set; }
        public int przedmiot_id { get; set; }
        public string student_imie { get; set; }
        public string student_nazwisko { get; set; }
        public string przedmiot_nazwa { get; set; }
        public int liczba_zajec { get; set; }
        public int udzial { get; set; }
        public string grupa { get; set; }

    }
}