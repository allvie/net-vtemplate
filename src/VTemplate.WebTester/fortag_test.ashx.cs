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
    /// 测试for标签
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class fortag_test : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            TemplateDocument document = new TemplateDocument(context.Server.MapPath("template/fortag_test.html"), Encoding.UTF8);
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
