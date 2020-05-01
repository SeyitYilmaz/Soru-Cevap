using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Soru_Cevap.ViewModel
{
    public class uyeModel
    {
        public int UyeID { get; set; }

        [Required(ErrorMessage ="Kullanıcı Adı Giriniz")]
        [Display(Name ="Kullanıcı Adı")]
        public string KullaniciAd { get; set; }
        [Required(ErrorMessage = "Şifre Giriniz")]
        [Display(Name = "Şifre Giriniz")]
        public string Sifre { get; set; }
        [Required(ErrorMessage = "Adınızı Giriniz")]
        [Display(Name = "Adınız")]
        public string Ad { get; set; }
        [Required(ErrorMessage = "Soyadınızı Giriniz")]
        [Display(Name = "Soyadınız")]
        public string Soyad { get; set; }
        [Required(ErrorMessage = "Mailinizi Giriniz")]
        [Display(Name = "Mailiniz")]
        public string Email { get; set; }
        public int UyeAdmin { get; set; }

    }
}