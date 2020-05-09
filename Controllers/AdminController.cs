using Soru_Cevap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Soru_Cevap.ViewModel;
using Soru_Cevap.Auth;
using System.IO;

namespace Soru_Cevap.Controllers
{
    public class AdminController : BaseController
    {
        dbSoruCevapEntities db = new dbSoruCevapEntities();
        // GET: Admin
        public ActionResult Index()
        {
            List<Soru> sorular = db.Soru.ToList();
            ViewBag.soruSay = sorular.Count;

            List<Uye> uyeler = db.Uye.ToList();
            ViewBag.uyeSay = uyeler.Count;

            List<Kategori> kategoriler = db.Kategori.ToList();
            ViewBag.katSay = kategoriler.Count;

            return View();
        }

        public ActionResult Kategoriler(int? id)
        {
            if (id == 1)
            {
                ViewBag.hata = "Kategori Uzerinde Makele Kayitli Oldugu Icin Silinemez";
            }
            if (id == 2)
            {
                ViewBag.sonuc = "Kategori Silindi";
            }
            List<Kategori> kategori = db.Kategori.ToList();
            return View(kategori);
        }

        public ActionResult KategoriEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult KategoriEkle(kategoriModel model)
        {
            if (db.Kategori.Where(m => m.KategoriAd == model.KategoriAd).Count() > 0)
            {
                ViewBag.hata = "Girilen Kategori Adi Kayitlidir!";
            }
            else
            {
                Kategori kat = new Kategori();
                kat.KategoriAd = model.KategoriAd;
                db.Kategori.Add(kat);
                db.SaveChanges();
                ViewBag.sonuc = "Kayit Basarili";
            }
            return View();
        }

        public ActionResult KategoriDuzenle(int? id)
        {
            Kategori kat = db.Kategori.Where(m => m.KategoriID == id).SingleOrDefault();
            if (kat == null)
            {
                return RedirectToAction("Kategoriler");
            }

            kategoriModel model = new kategoriModel();
            model.KategoriID = kat.KategoriID;
            model.KategoriAd = kat.KategoriAd;
            return View(model);
        }

        [HttpPost]
        public ActionResult KategoriDuzenle(kategoriModel model)
        {
            Kategori kat = db.Kategori.Where(m => m.KategoriID == model.KategoriID).SingleOrDefault();
            if (kat == null)
            {
                return RedirectToAction("Kategoriler");
            }
            kat.KategoriAd = model.KategoriAd;
            db.SaveChanges();
            ViewBag.sonuc = "Kategori Duzenlendi";
            return View(model);
        }

        public ActionResult KategoriSil(int? id)
        {
            if (db.Soru.Where(m => m.KategoriID == id).Count() > 0)
            {
                return RedirectToAction("Kategoriler/1");
            }
            Kategori kat = db.Kategori.Where(m => m.KategoriID == id).SingleOrDefault();

            if (kat != null)
            {
                db.Kategori.Remove(kat);
                db.SaveChanges();
                return RedirectToAction("Kategoriler/2");
            }
            return RedirectToAction("Kategoriler");
        }

        public ActionResult Sorular(int? id)
        {

            if (id == 1)
            {
                ViewBag.sonuc = "Soru Silindi";
            }
            List<Soru> sorular = db.Soru.ToList();

            return View(sorular);
        }
        public ActionResult SoruDuzenle(int? id)
        {
            Soru soru = db.Soru.Where(m => m.SoruID == id).SingleOrDefault();
            if (soru == null)
            {
                return RedirectToAction("Sorular");
            }

            soruModel model = getModel();
            model.SoruID = soru.SoruID;
            model.Baslik = soru.Baslik;
            model.Icerik = soru.Icerik;
            model.KategoriID = soru.KategoriID;
            return View(model);
        }

        [HttpPost]
        public ActionResult SoruDuzenle(soruModel model)
        {
            Soru soru = db.Soru.Where(m => m.SoruID == model.SoruID).SingleOrDefault();
            if (soru == null)
            {
                return RedirectToAction("Sorular");
            }

            soru.Baslik = model.Baslik;
            soru.Icerik = model.Icerik;
            soru.KategoriID = model.KategoriID;
            db.SaveChanges();
            ViewBag.sonuc = "Soru Duzenlendi";
            model = getModel();
            return View(model);
        }

        public ActionResult SoruSil(int? id)
        {
            Soru soru = db.Soru.Where(m => m.SoruID == id).SingleOrDefault();
            if (soru == null)
            {
                return RedirectToAction("Sorular");
            }
            List<Cevap> cevaplar = db.Cevap.Where(m => m.SoruID == id).ToList();
            db.Cevap.RemoveRange(cevaplar);
            db.Soru.Remove(soru);
            db.SaveChanges();

            return RedirectToAction("Sorular");
        }

        private soruModel getModel()
        {
            soruModel model = new soruModel();
            model.KategoriList = (from kat in db.Kategori.ToList()
                                  select new SelectListItem
                                  {
                                      Selected = false,
                                      Text = kat.KategoriAd,
                                      Value = kat.KategoriID.ToString()
                                  }).ToList();
            model.KategoriList.Insert(0, new SelectListItem
            {
                Selected = true,
                Value = "",
                Text = "Seciniz"
            });
            return model;
        }


        public ActionResult Uyeler()
        {
            List<Uye> uyeler = db.Uye.ToList();
            return View(uyeler);
        }

        public ActionResult UyeEkle()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult UyeEkle(uyeModel model)
        {
            if (db.Uye.Where(m => m.KullaniciAd == model.KullaniciAd).Count() > 0)
            {
                ViewBag.hata = "Girilen Kullanıcı Adı Kayıtlıdır!";
                return View();
            }
            Uye uye = new Uye();
            uye.KullaniciAd = model.KullaniciAd;
            uye.Sifre = model.Sifre;
            uye.Ad = model.Ad;
            uye.Soyad = model.Soyad;
            uye.Email = model.Email;
            uye.UyeAdmin = model.UyeAdmin;
            db.Uye.Add(uye);
            db.SaveChanges();
            ViewBag.sonuc = "Üye Eklendi";
            return View();
        }

        public ActionResult UyeDuzenle(int? id)
        {
            Uye uye = db.Uye.Where(m => m.UyeID == id).SingleOrDefault();
            if (uye == null)
            {
                return RedirectToAction("Uyeler");
            }
            uyeModel model = new uyeModel();
            model.UyeID = uye.UyeID;
            model.KullaniciAd = model.KullaniciAd;
            model.Sifre = uye.Sifre;
            model.Ad = uye.Ad;
            model.Soyad = uye.Soyad;
            model.Email = uye.Email;
            model.UyeAdmin = uye.UyeAdmin;
            return View(model);
        }
        [HttpPost]
        public ActionResult UyeDuzenle(uyeModel model)
        {
            Uye uye = db.Uye.Where(m => m.UyeID == model.UyeID).SingleOrDefault();
            uye.KullaniciAd = model.KullaniciAd;
            uye.Sifre = model.Sifre;
            uye.Ad = model.Ad;
            uye.Soyad = model.Soyad;
            uye.Email = model.Email;
            uye.UyeAdmin = model.UyeAdmin;
            db.SaveChanges();
            ViewBag.sonuc = "Uye Guncellendi";
            return View();
        }

        public ActionResult UyeSil(int? id)
        {
            if (db.Soru.Where(m => m.UyeID == id).Count() > 0)
            {
                return RedirectToAction("Uyeler/1");
            }

            Uye uye = db.Uye.Where(m => m.UyeID == id).SingleOrDefault();
            if (uye != null)
            {
                List<Cevap> yorumlar = db.Cevap.Where(m => m.UyeID == id).ToList();
                db.Cevap.RemoveRange(yorumlar);
                db.Uye.Remove(uye);
                db.SaveChanges();
                return RedirectToAction("Uyeler/2");
            }
            return RedirectToAction("Uyeler");
        }
    }

}
