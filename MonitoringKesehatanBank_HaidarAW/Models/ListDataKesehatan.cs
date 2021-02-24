using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitoringKesehatanBank_HaidarAW.Models
{
    public class ListDataKesehatan
    {
        public DateTime Periode { get; set; }
        public string SandiBank { get; set; }
        public double KreditKol1 { get; set; }
        public double KreditKol2 { get; set; }
        public double KreditKol3 { get; set; }
        public double KreditKol4 { get; set; }
        public double KreditKol5 { get; set; }
        public double Laba { get; set; }
        public double Modal { get; set; }
        public double TotalAset { get; set; }
        public double ATMR{ get; set; }
        public double BebanOperasional { get; set; }
        public double PendapatanOperasional { get; set; }
        public double DanaPihakKe3 { get; set; }
        public double NPL { get; set; }
        public double ROE { get; set; }
        public double ROA { get; set; }
        public double LDR { get; set; }
        public double BOPO { get; set; }
        public double CAR { get; set; }
    }
}