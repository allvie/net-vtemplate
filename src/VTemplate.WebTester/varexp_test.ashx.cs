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
    /// 测试变量表达式
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class varexp_test : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            TemplateDocument document = new TemplateDocument(context.Server.MapPath("template/varexp_test.html"), Encoding.UTF8);
            document.Variables.SetValue("user", new { name = "张三", age = 20 });
            document.GetChildTemplateById("t1").Variables.SetValue("user", new { name = "李四", age = 26 });
            document.GetChildTemplateById("t2").Variables.SetValue("user", new { name = "王五", age = 30 });
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
