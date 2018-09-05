using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HowardCoupon.Models.Dao;
using Newtonsoft.Json.Linq;
using System.Collections;
using HowardCoupon.Models.Dao.impl;
using HowardCoupon.Poco;
using HowardCoupon_vs2010.Models.Edmx;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.ComponentModel;
using System.Text;


namespace HowardCoupon_vs2010.Models.Service
{
    public class CommonService
    {
        private ICommonDao iCommonDao=new CommonDaoImpl();
        private IntraEntities db = new IntraEntities();

        public JObject getCodeList(EasyuiParamPoco param,String kind, String type)
        {
            JObject jo = new JObject();            
            JArray ja = new JArray();
            List<Hashtable> list = iCommonDao.getCodeList(param,kind,type);
            foreach (Hashtable ht in list)
            {                
                var itemObject = new JObject
                    {   
                        {"UUID",HashtableUtil.getValue(ht, "UUID")},                
                        {"NAME",HashtableUtil.getValue(ht, "NAME")},
                        {"CODE",HashtableUtil.getValue(ht, "CODE")},
                    };
                ja.Add(itemObject);                
            }            
            jo.Add("rows", ja);
            jo.Add("total", ja.Count());
            return jo;
        }


        public JArray getCodeList(String kind, String type)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            List<Hashtable> list = iCommonDao.getCodeList(kind, type);
            foreach (Hashtable ht in list)
            {
                var itemObject = new JObject
                    {   
                        {"UUID",HashtableUtil.getValue(ht, "UUID")},                
                        {"NAME",HashtableUtil.getValue(ht, "CODE") + " "+HashtableUtil.getValue(ht, "NAME")},
                        {"CODE",HashtableUtil.getValue(ht, "CODE")},
                    };
                ja.Add(itemObject);
            }

            return ja;
        }



        public JArray getENT()
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
         //   var query = from t in db.I_COMPANY select t;
            var query = from t in db.COUPON_COMPANY select new { CID=t.CODE,CODE=t.CODE,NAME=t.NAME};
                        
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"CID",item.CID},                
                        {"NAME",item.CODE +" " +item.NAME},
                        
                    };
                ja.Add(itemObject);
            }
          //  jo.Add("rows", ja);            
            return ja;
        }

        public JArray getDPT(String CID)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
        //    Decimal cid = StringUtils.getDecimal(CID);
           // var query = from t in db.I_DEPARTMENT where t.CID == cid select t;
            var query = from t in db.COUPON_DEPARTMENT where t.OUTID == CID select t;

            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"DID",item.UUID},
                        //{"NAME",item.CODE.Substring(3,4) + " " +item.NAME},
                        {"NAME",item.CODE + " " +item.NAME},
                        
                    };
                ja.Add(itemObject);
            }
          
            return ja;
        }

        public JArray getDPT()
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            //    Decimal cid = StringUtils.getDecimal(CID);
            // var query = from t in db.I_DEPARTMENT where t.CID == cid select t;
            var query = from t in db.COUPON_DEPARTMENT select t;

            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"DID",item.UUID},
                        //{"NAME",item.CODE.Substring(3,4) + " " +item.NAME},
                        {"NAME",item.CODE + " " +item.NAME},
                        
                    };
                ja.Add(itemObject);
            }

            return ja;
        }


        public Hashtable getUserInfo(String USERID)
        {
            Hashtable ht = new Hashtable();
            //Decimal x = Decimal.Parse(ID);

            var query = from a in db.COUPON_EMPLOYEE
                        join b in db.COUPON_COMPANY on a.ENTID equals b.CODE into compg
                        from comp in compg.DefaultIfEmpty()
                        join c in db.COUPON_DEPARTMENT on a.DPTID equals c.CODE into deptg
                        from dept in deptg.DefaultIfEmpty()
                        where a.USERID  == USERID
                        select new {USERNAME=a.USERNAME, DEPART_NAME = dept.NAME, DEPART_CODE = dept.CODE, COMP_NAME = comp.NAME, COMP_CODE = comp.CODE };


            foreach (var item in query)
            {                
                ht.Add("USERNAME", item.USERNAME);
                ht.Add("DEPART_NAME", item.DEPART_NAME);
                ht.Add("DEPART_CODE", item.DEPART_CODE);
                ht.Add("COMP_NAME", item.COMP_NAME);
                ht.Add("COMP_CODE", item.COMP_CODE);
                break;
            }
            return ht;
        }


        public static byte[] getPdf()
        {
            var templatePath = HttpContext.Current.Server.MapPath("~/Report/test.pdf");
            var fontPath = HttpContext.Current.Server.MapPath("~/Report/test.pdf");
            BaseFont baseFont = BaseFont.CreateFont(fontPath,BaseFont.IDENTITY_H,BaseFont.NOT_EMBEDDED);
            var reader = new PdfReader(templatePath);
            var outStream = new MemoryStream();
            var stamper = new PdfStamper(reader, outStream);

            var form = stamper.AcroFields;
            var fieldKeys = form.Fields.Keys;

            foreach (string fieldKey in fieldKeys)
            {
                if (form.GetField(fieldKey) == "MyTemplatesOriginalTextFieldA")
                    form.SetField(fieldKey, "1234");
                if (form.GetField(fieldKey) == "MyTemplatesOriginalTextFieldB")
                    form.SetField(fieldKey, "5678");
            }

            // "Flatten" the form so it wont be editable/usable anymore  
            stamper.FormFlattening = true;

            stamper.Close();
            reader.Close();

            return outStream.ToArray();
        }



        public void convPdf()
        {
            var outPath = HttpContext.Current.Server.MapPath("~/Report/out.pdf");
            var templatePath = HttpContext.Current.Server.MapPath("~/Report/test.pdf");
            var fontPath = HttpContext.Current.Server.MapPath("~/Report/fonts/FREE3OF9.TTF");
            BaseFont baseFont = BaseFont.CreateFont(fontPath,BaseFont.IDENTITY_H,BaseFont.NOT_EMBEDDED);
            Font barcodeFont =new Font(baseFont,20);  //字體大小
            Font numFont = new Font(Font.FontFamily.HELVETICA, 9);
            PdfReader reader;    
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document,new FileStream(outPath,FileMode.Create));
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage newPage;
         for(int i = 0;i < 10;i++)
            {
                reader = new PdfReader(templatePath);
                int iPageNum = reader.NumberOfPages;
                for(int j=1;j<=iPageNum;j++)
                {
                    //document.Open();
                    
                    //document.Close();
                    document.NewPage();
                    document.Add(new Paragraph("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n"));
                    Paragraph pg = new Paragraph(10f, "12345678910" + i, barcodeFont);
                    pg.Alignment = Element.ALIGN_RIGHT;
                    Paragraph pg2 = new Paragraph(10f, "12345678910" + i, numFont);
                    pg2.Alignment = Element.ALIGN_RIGHT;
                    document.Add(pg);
                    document.Add(pg2);
                    newPage = writer.GetImportedPage(reader,j);
                    cb.AddTemplate(newPage,0,0);
                }
            }
            document.Close();
        }        

    }
}