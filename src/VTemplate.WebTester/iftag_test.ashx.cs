using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using VTemplate.Engine;
using System.Text;

namespace VTemplate.WebTester
{
    /// <summary>
    /// 测试If标签
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class iftag_test : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            TemplateDocument document = new TemplateDocument(context.Server.MapPath("template/iftag_test.html"), Encoding.UTF8);
            document.Variables.SetValue("user", new { name = "张三", age = 20 });
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
