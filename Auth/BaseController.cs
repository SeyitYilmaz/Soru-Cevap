using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Soru_Cevap.Auth
{
    public class BaseController:System.Web.Mvc.Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["uyeOturum"] == null || Session["uyeAdmin"].ToString() != "1")
            {
                filterContext.Result = new RedirectResult("~/Home/OturumAc");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}