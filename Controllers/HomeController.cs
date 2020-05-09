using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Soru_Cevap.Auth;
using Soru_Cevap.Models;
using Soru_Cevap.ViewModel;

namespace Soru_Cevap.Controllers
{
    public class HomeController : Controller
    {
        dbSoruCevapEntities db = new dbSoruCevapEntities();
        // GET: Home
        public ActionResult Index()
        {
            List<Soru> soru = db.Soru.ToList();
            return View(soru);
        }

        public ActionResult Kategoriler()
        {
            List<Kategori> kategori = db.Kategori.ToList();
            return PartialView(kategori);
        }

        public ActionResult KategoriSoru(int? id)
        {
            Kategori kategori = db.Kategori.Where(s => s.KategoriID == id).SingleOrDefault();
            if (kategori == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.kategori = kategori.KategoriAd;

            List<Soru> soru = db.Soru.Where(s => s.KategoriID == id).ToList();
            if (soru.Count == 0)
            {
                ViewBag.kayityok = "Henüz bu kategoride soru sorulmamış.";
            }
            return View(soru);
        }

        public ActionResult SoruDetay(int? id)
        {
            Soru soru = db.Soru.Where(s => s.SoruID == id).SingleOrDefault();
            if (soru == null)
            {
                return RedirectToAction("Index");
            }
            soru.Okunma += 1;
            db.SaveChanges();
            return View(soru);
        }

        public PartialViewResult Cevaplar(int? SoruId)
        {
            List<Cevap> cevap = db.Cevap.Where(s => s.SoruID == SoruId).ToList();
            return PartialView(cevap);
        }

        public ActionResult OturumAc(uyeModel model, string returnUrl)
        {
            Uye uye = db.Uye.Where(m => m.KullaniciAd == model.KullaniciAd && m.Sifre == model.Sifre).SingleOrDefault();
            if (uye!=null)
            {
                Session["uyeOturum"] = true;
                Session["uyeID"] = uye.UyeID;
                Session["uyeKulAd"] = uye.KullaniciAd;
                Session["uyeAdmin"] = uye.UyeAdmin;

                if (returnUrl == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    Redirect(returnUrl);
                }
            }
            else
            {
                ViewBag.hata = "Kullanıcı Adı veya Parola Geçersizdir.";
                return View();
            }
            return View();
        }

        public ActionResult OturumKapat(string returnUrl)
        {
            Session.Abandon();
            return Redirect(returnUrl);
        }

        public ActionResult UyeOl()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UyeOl(uyeModel model)
        {
            if (db.Uye.Where(m=>m.KullaniciAd == model.KullaniciAd).Count()>0)
            {
                ViewBag.hata = "Girilen Kullannıcı Adı Kaydı Mevcut";
                return View();
            }

            Uye yeni = new Uye();        
            yeni.Ad = model.Ad;
            yeni.Soyad = model.Soyad;
            yeni.KullaniciAd = model.KullaniciAd;
            yeni.Email = model.Email;
            yeni.Sifre = model.Sifre;
            yeni.UyeAdmin = 0;
            db.Uye.Add(yeni);
            db.SaveChanges();

            Uye uye = db.Uye.OrderByDescending(m => m.UyeID).FirstOrDefault();
            Session["uyeOturum"] = true;
            Session["uyeID"] = uye.UyeID;
            Session["uyeKulAd"] = uye.KullaniciAd;
            Session["uyeAdmin"] = uye.UyeAdmin;

            return RedirectToAction("Index");
        }
        public JsonResult CevapYaz(string cevap, int SoruId)
        {
            if (cevap != null && Session["uyeID"] != null)
            {
                int uyeId = Convert.ToInt32(Session["uyeID"].ToString());
                db.Cevap.Add(new Cevap()
                {
                    Icerik = cevap,
                    UyeID = uyeId,
                    SoruID = SoruId,
                    Tarih = DateTime.Now
                    
                });
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CevapSil(int cevapID)
        {
            if (Session["uyeID"] != null)
            {
                var silinenCevap = db.Cevap.Where(y => y.CevapID== cevapID).FirstOrDefault();
                db.Cevap.Remove(silinenCevap);
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UyeDetay(int? id)
        {
            Uye uye = db.Uye.Where(x => x.UyeID == id).SingleOrDefault();
            if (uye == null )
            {
                RedirectToAction("Index");
            }
            return View(uye);
        }

        public ActionResult SonEklenenler()
        {
            List<Soru> sorular = db.Soru.OrderByDescending(s => s.SoruID).Take(5).ToList();

            return PartialView(sorular);
        }

        public ActionResult SoruEkle()
        {
            
            if (Session["uyeID"] == null)
            {
                return RedirectToAction("OturumAc");
            }
            else
            {
                soruModel model = getModel();
                return View(model);
                
            }
                        

        }

        [HttpPost]
        public ActionResult SoruEkle(soruModel model)
        {
            int uyeID = Convert.ToInt32(Session["uyeID"].ToString());
            if (db.Soru.Where(m=>m.Baslik == model.Baslik).Count() > 0)
            {
                ViewBag.hata = "Benzer soru sitemizde mevcut. Lütfen sitemizi kontrol ediniz.";
                model = getModel();
                return View(model);
            }
            else
            {
                Soru soru = new Soru();
                soru.Baslik = model.Baslik;
                soru.KategoriID = model.KategoriID;
                soru.Icerik = model.Icerik;
                soru.Tarih = DateTime.Now;
                soru.Okunma = 0;
                soru.UyeID = uyeID;

                db.Soru.Add(soru);
                db.SaveChanges();

                ViewBag.sonuc = "Soru Eklendi";
                model = getModel();
                return View(model);
            }
        }

        public ActionResult SoruDuzenle(int? id)
        {
            Soru soru = db.Soru.Where(m => m.SoruID == id).SingleOrDefault();
            if (soru == null)
            {
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }
            List<Cevap> cevaplar = db.Cevap.Where(m => m.SoruID == id).ToList();
            db.Cevap.RemoveRange(cevaplar);
            db.Soru.Remove(soru);
            db.SaveChanges();

            return RedirectToAction("Index");
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
    }
}