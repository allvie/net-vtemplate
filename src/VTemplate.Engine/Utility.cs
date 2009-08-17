/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  Utility
 *
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Collections.Specialized;

namespace VTemplate.Engine
{
    /// <summary>
    /// 实用类
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// 
        /// </summary>
        static Utility()
        {
            RenderInstanceCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }

        #region 数据格式化函数块
        /// <summary>
        /// 判断是否是空数据(null或DBNull)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool IsNothing(object value)
        {
            return value == null || value == DBNull.Value;
        }
        /// <summary>
        /// XML编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string XmlEncode(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                StringWriter stringWriter = new StringWriter();
                XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
                xmlWriter.WriteString(value);
                xmlWriter.Flush();
                value = stringWriter.ToString();
            }
            return value;
        }

        /// <summary>
        /// 文本编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string TextEncode(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = HttpUtility.HtmlEncode(value);
                value = value.Replace(" ", "&nbsp;");
                value = value.Replace("\t", "&nbsp;&nbsp;");
                value = Regex.Replace(value, "\r\n|\r|\n", "<br />");
            }
            return value;
        }

        /// <summary>
        /// JS脚本编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string JsEncode(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("\\", "\\\\");
                value = value.Replace("\"", "\\\"");
                value = value.Replace("\'", "\\'");
                value = value.Replace("\r", "\\r");
                value = value.Replace("\n", "\\n");
            }
            return value;
        }
        #endregion

        #region 数据转换函数块
        /// <summary>
        /// 转换字符串为布尔值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool ConverToBoolean(string value)
        {
            if (value == "1" || string.Equals(value, Boolean.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 转换字符串为整型值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static int ConverToInt32(string value)
        {
            int v;
            if (!int.TryParse(value, out v))
            {
                v = 0;
            }
            return v;
        }

        /// <summary>
        /// 转换字符串为数值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static decimal ConverToDecimal(object value)
        {
            if (value == null || value == DBNull.Value) return 0M;
            decimal v;
            try
            {
                v = Convert.ToDecimal(value);
            }
            catch
            {
                v = 0M;
            }
            return v;
        }

        /// <summary>
        /// 截取字符
        /// </summary>
        /// <param name="value">要截取的字符串</param>
        /// <param name="maxLength">最大大小</param>
        /// <param name="charset">采用的编码</param>
        /// <returns></returns>
        internal static string CutString(string value, int maxLength, Encoding charset)
        {
            StringBuilder buffer = new StringBuilder(maxLength);
            int length = 0;
            int index = 0;
            while (index < value.Length)
            {
                char c = value[index];
                length += charset.GetByteCount(new char[] { c });
                if (length <= maxLength)
                {
                    buffer.Append(c);
                }
                else
                {
                    break;
                }
                index++;
            }
            return buffer.ToString();
        }

        /// <summary>
        /// 从字符集名称获取编码器
        /// </summary>
        /// <param name="charset"></param>
        /// <returns></returns>
        internal static Encoding GetEncodingFromCharset(string charset)
        {
            Encoding e = Encoding.Default;
            try
            {
                e = Encoding.GetEncoding(charset);
            }
            catch
            {
                e = Encoding.Default;
            }
            return e;
        }

        /// <summary>
        /// 转换为某种数据类型
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <param name="type">最终的数据类型</param>
        /// <returns>如果转换失败返回null</returns>
        internal static object ConvertTo(string value, Type type)
        {
            object result = value;
            if (value != null)
            {
                try
                {
                    if (type.IsEnum)
                    {
                        //枚举类型
                        result = Enum.Parse(type, value, true);
                    }
                    else if (Type.GetTypeCode(type) == TypeCode.DateTime)
                    {
                        //日期型
                        result = DateTime.Parse(value);
                    }
                    else
                    {
                        //其它值
                        result = (value as IConvertible).ToType(type, null);
                    }
                }
                catch
                {
                    result = null;
                }
            }
            return result;
        }
        #endregion

        #region 数据源处理函数块
        /// <summary>
        /// 获取某个属性的值
        /// </summary>
        /// <param name="container">数据源</param>
        /// <param name="propName">属性名</param>
        /// <param name="exist">是否存在此属性</param>
        /// <returns>属性值</returns>
        internal static object GetPropertyValue(object container, string propName, out bool exist)
        {
            exist = false;
            object value = null;
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            if (string.IsNullOrEmpty(propName))
            {
                throw new ArgumentNullException("propName");
            }
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(container).Find(propName, true);
            if (descriptor != null)
            {
                exist = true;
                value = descriptor.GetValue(container);
            }
            else if (container is IDictionary)
            {
                //是IDictionary集合
                IDictionary idic = (IDictionary)container;
                if (idic.Contains(propName))
                {
                    exist = true;
                    value = idic[propName];
                }
            }
            else if (container is NameObjectCollectionBase)
            {
                //是NameObjectCollectionBase派生对象
                NameObjectCollectionBase nob = (NameObjectCollectionBase)container;
                //调用私有方法
                MethodInfo method = nob.GetType().GetMethod("BaseGet", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, new ParameterModifier[] { new ParameterModifier(1) });
                if (method != null)
                {
                    value = method.Invoke(container, new object[] { propName });
                    exist = value == null;
                }
            }
            return value;
        }
        /// <summary>
        /// 获取方法的结果值
        /// </summary>
        /// <param name="container"></param>
        /// <param name="methodName"></param>
        /// <param name="exist"></param>
        /// <returns></returns>
        internal static object GetMethodResult(object container, string methodName, out bool exist)
        {
            exist = true;
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentNullException("methodName");
            }
            MethodInfo method = container.GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance
                                                                          | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase,
                                                                          null, CallingConventions.Any, new Type[0], new ParameterModifier[0]);
            if (method != null)
            {
                return method.Invoke(method.IsStatic ? null : container, null);
            }
            else
            {
                //不存在此值
                exist = false;
                return null;
            }
        }

        /// <summary>
        /// 返回数据源的枚举数
        /// </summary>
        /// <param name="dataSource">要处理的数据源</param>
        /// <returns>如果非IListSource与IEnumerable实例则返回null</returns>
        internal static IEnumerable GetResolvedDataSource(object dataSource)
        {
            if (dataSource != null)
            {
                IListSource source = dataSource as IListSource;
                if (source != null)
                {
                    IList list = source.GetList();
                    if (!source.ContainsListCollection)
                    {
                        return list;
                    }
                    if ((list != null) && (list is ITypedList))
                    {
                        PropertyDescriptorCollection itemProperties = ((ITypedList)list).GetItemProperties(new PropertyDescriptor[0]);
                        if ((itemProperties == null) || (itemProperties.Count == 0))
                        {
                            return null;
                        }
                        PropertyDescriptor descriptor = itemProperties[0];
                        if (descriptor != null)
                        {
                            object component = list[0];
                            object value = descriptor.GetValue(component);
                            if ((value != null) && (value is IEnumerable))
                            {
                                return (IEnumerable)value;
                            }
                        }
                        return null;
                    }
                }
                if (dataSource is IEnumerable)
                {
                    return (IEnumerable)dataSource;
                }
            }
            return null;
        }
        #endregion

        #region 模版引擎相关辅助函数块
        /// <summary>
        /// 修正文件地址
        /// </summary>
        /// <param name="template"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static string ResolveFilePath(Template template, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && fileName.IndexOf(":") == -1 && !fileName.StartsWith("\\\\"))
            {
                string referPath = string.Empty;
                while (string.IsNullOrEmpty(referPath) && template != null)
                {
                    referPath = template.File;
                    template = template.OwnerTemplate;
                }
                if (!string.IsNullOrEmpty(referPath))
                {
                    fileName = Path.Combine(Path.GetDirectoryName(referPath), fileName);
                }
                fileName = Path.GetFullPath(fileName);
            }
            return fileName;
        }
        /// <summary>
        /// 统计行号与列号(x = 列号, y = 行号)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static Point GetLineAndColumnNumber(string text, int offset)
        {
            int line, column, p;
            line = column = 1;
            p = 0;
            while (p < offset && p < text.Length)
            {
                char c = text[p];
                if (c == '\r' || c == '\n')
                {
                    if (c == '\r' && p < (text.Length - 1))
                    {
                        //\r\n字符
                        if (text[p + 1] == '\n') p++;
                    }
                    line++;
                    column = 1;
                }
                else
                {
                    column++;
                }
                p++;
            }
            return new Point(column, line);
        }
        /// <summary>
        /// 从模版中获取某个变量.如果不存在此变量则添加新的变量
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <param name="varName"></param>
        /// <returns></returns>
        internal static Variable GetVariableOrAddNew(Template ownerTemplate, string varName)
        {
            Variable var = ownerTemplate.Variables[varName];
            if (var == null)
            {
                var = new Variable(ownerTemplate, varName);
                ownerTemplate.Variables.Add(var);
            }
            return var;
        }

        /// <summary>
        /// 根据前缀获取变量的模版所有者
        /// </summary>
        /// <param name="template"></param>
        /// <param name="prefix"></param>
        /// <returns>如果prefix值为null则返回template的根模版.如果为空值.则为template.如果为#则返回template的父模版.否则返回对应Id的模版</returns>
        internal static Template GetVariableTemplateByPrefix(Template template, string prefix)
        {
            if (prefix == string.Empty) return template;               //前缀为空.则返回当前模版
            if (prefix == "#") return template.OwnerTemplate ?? template;   //前缀为#.则返回父模版(如果父模版不存在则返回当前模版)

            //取得根模版
            while (template.OwnerTemplate != null) template = template.OwnerTemplate;

            //如果没有前缀.则返回根模版.否则返回对应Id的模版
            return prefix == null ? template : template.GetChildTemplateById(prefix);
        }
        #endregion

        #region 模版数据解析相关辅助函数块
        /// <summary>
        /// 存储模版解析器实例的缓存
        /// </summary>
        private static Dictionary<string, object> RenderInstanceCache;
        /// <summary>
        /// 获取解析器的实例
        /// </summary>
        /// <param name="renderInstance"></param>
        /// <returns></returns>
        private static object GetRenderInstance(string renderInstance)
        {
            if (string.IsNullOrEmpty(renderInstance)) return null;

            string[] k = renderInstance.Split(new char[] { ',' }, 2);
            if (k.Length != 2) return null;

            string assemblyKey = k[1].Trim();
            string typeKey = k[0].Trim();
            string cacheKey = string.Concat(typeKey, ",", assemblyKey);

            //从缓存读取
            object render;
            bool flag = false;
            lock (RenderInstanceCache)
            {
                flag = RenderInstanceCache.TryGetValue(cacheKey, out render);
            }
            if (!flag || render == null)
            {
                //重新生成实例
                render = null;
                //生成实例
                Assembly assembly;
                if (assemblyKey.IndexOf(":") != -1)
                {
                    assembly = Assembly.LoadFrom(assemblyKey);
                }
                else
                {
                    assembly = Assembly.Load(assemblyKey);
                }
                if (assembly != null)
                {
                    render = assembly.CreateInstance(typeKey, false);
                }
                if (render != null)
                {
                    //缓存
                    lock (RenderInstanceCache)
                    {
                        if (RenderInstanceCache.ContainsKey(cacheKey))
                        {
                            RenderInstanceCache[cacheKey] = render;
                        }
                        else
                        {
                            RenderInstanceCache.Add(cacheKey, render);
                        }
                    }
                }
            }
            return render;
        }

        /// <summary>
        /// 预解析模版数据
        /// </summary>
        /// <param name="renderInstance">模版解析器实例的配置</param>
        /// <param name="template">要解析处理的模版</param>
        internal static void PreRenderTemplate(string renderInstance, Template template)
        {
            ITemplateRender render = GetRenderInstance(renderInstance) as ITemplateRender;
            if (render != null) render.PreRender(template);
        }
        /// <summary>
        /// 使用特性方法预解析模版数据
        /// </summary>
        /// <param name="renderInstance"></param>
        /// <param name="renderMethod"></param>
        /// <param name="template"></param>
        internal static void PreRenderTemplateByAttributeMethod(string renderInstance, string renderMethod, Template template)
        {
            object render = GetRenderInstance(renderInstance);
            if (render != null)
            {
                //取得特性方法
                MethodInfo method = null;

                MethodInfo[] methods = render.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (MethodInfo m in methods)
                {
                    TemplateRenderMethodAttribute att = System.Attribute.GetCustomAttribute(m, typeof(TemplateRenderMethodAttribute)) as TemplateRenderMethodAttribute;
                    if (att != null)
                    {
                        if (renderMethod.Equals(m.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            method = m;
                            break;
                        }
                    }
                }

                if (method != null)
                {
                    ParameterInfo[] pars = method.GetParameters();
                    if (pars.Length == 1 && pars[0].ParameterType == typeof(Template))
                    {
                        method.Invoke(method.IsStatic ? null : render, new object[] { template });
                    }
                }
            }
        }
        #endregion
    }
}
