//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Soru_Cevap.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cevap
    {
        public int CevapID { get; set; }
        public string Icerik { get; set; }
        public Nullable<int> UyeID { get; set; }
        public Nullable<int> SoruID { get; set; }
        public Nullable<System.DateTime> Tarih { get; set; }
    
        public virtual Soru Soru { get; set; }
        public virtual Uye Uye { get; set; }
    }
}
