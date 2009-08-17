using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using VTemplate.Engine;
using System.Text;
using System.Collections.Generic;

namespace VTemplate.WebTester
{
    /// <summary>
    /// 测试serverdata标签
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class serverdatatag_test : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            TemplateDocument document = new TemplateDocument(context.Server.MapPath("template/serverdatatag_test.html"), Encoding.UTF8);
            document.Render(context.Response.Output);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
