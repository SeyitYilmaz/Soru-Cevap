using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Soru_Cevap.ViewModel
{
    public class soruModel
    {
        public int SoruID { get; set; }

        [Display(Name = "Soru Başlığı")]
        [Required(ErrorMessage = "Soru Başlığı Giriniz")]
        [StringLength(50, ErrorMessage = "Soru Başlığı En Fazla 50 Karakter Olarak Giriniz")]
        public string Baslik { get; set; }

        [AllowHtml]
        [Display(Name = "Soru İçeriği")]
        [Required(ErrorMessage = "Soru İçeriği Giriniz")]
        public string Icerik { get; set; }
        public DateTime Tarih { get; set; }

        [Display(Name = "Soru Kategori")]
        [Required(ErrorMessage = "Soru Kategori Seçiniz")]
        public int KategoriID { get; set; }
        public List<SelectListItem> KategoriList { get; set; }
        public int UyeID { get; set; }
        public int Okunma { get; set; }
    }
}