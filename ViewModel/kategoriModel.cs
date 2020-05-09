using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Soru_Cevap.ViewModel
{
    public class kategoriModel
    {
        public int KategoriID { get; set; }

        [Required(ErrorMessage = "Kategori Giriniz")]
        [Display(Name ="Kategori")]
        [StringLength(50,ErrorMessage ="Kategori Ad En Fazla 50 Karakter Olabilir")]
        public string KategoriAd { get; set; }

    }
}