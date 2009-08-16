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
    /// 测试template标签
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class templatetag_test : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            TemplateDocument document = new TemplateDocument(context.Server.MapPath("template/templatetag_test.html"), Encoding.UTF8);
            List<object> users1 = new List<object>()
            {
                new {Name = "张三", age="20"},
                new {Name = "李四", age="24"},
                new {Name = "王五", age="30"}
            };

            List<object> users2 = new List<object>()
            {
                new {Name = "黄丽", age="18"},
                new {Name = "张燕", age="20"},
                new {Name = "陈梅", age="23"}
            };
            document.Variables.SetValue("users", users1);
            document.GetChildTemplateById("female").Variables.SetValue("users", users2);
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
