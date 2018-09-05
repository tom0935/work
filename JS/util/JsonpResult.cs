using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jasper.util
{
    public class JsonpResult : JsonResult
    {
        public string Callback { get; set; }
        public object JData { get; set; }
        public JsonpResult(object data)
        {
            JData = data;
        }
        public JsonpResult()
        {
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var ctx = context.HttpContext;
            Callback = ctx.Request["callback"];
            var dataToSend = JsonConvert.SerializeObject(JData);
            ctx.Response.Write(string.Format("{0}({1});", Callback, dataToSend));
        }
    } 
}