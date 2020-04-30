using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Soru_Cevap.Models;

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
    }
}