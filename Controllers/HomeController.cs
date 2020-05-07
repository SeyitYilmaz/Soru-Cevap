using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
                Session["uyeFoto"] = uye.Foto;

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
            if (model.Foto != null && model.Foto.ContentLength > 0)
            {
                string dosya = Guid.NewGuid().ToString();
                string uzanti = Path.GetExtension(model.Foto.FileName).ToLower();

                if (uzanti != ".jpg" && uzanti != ".jpeg" && uzanti != ".png")
                {
                    ModelState.AddModelError("Foto", "Dosya Uzantısı JPG,JPEG veya PNG Olmalıdır!");
                    return View(model);
                }

                string dosyaAdi = dosya + uzanti;
                model.Foto.SaveAs(Server.MapPath("~/Content/UyeFoto/" + dosyaAdi));
                yeni.Foto = dosyaAdi;
            }

            
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
            Session["uyeFoto"] = uye.Foto;

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
    }
}