﻿/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  TemplateDocument
 *
 * ***********************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace VTemplate.Engine
{
    /// <summary>
    /// 模版文档
    /// </summary>
    [Serializable]
    public class TemplateDocument : Template
    {
        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        private TemplateDocument() { }
        /// <summary>
        /// 根据TextRader数据进行解析
        /// </summary>
        /// <param name="reader"></param>
        public TemplateDocument(TextReader reader)
        {
            this.ParseString(reader.ReadToEnd());
        }
        /// <summary>
        /// 根据文件内容进行解析
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="charset"></param>
        public TemplateDocument(string fileName, Encoding charset)
        {
            string text = System.IO.File.ReadAllText(fileName, charset);
            this.File = Path.GetFullPath(fileName);            
            this.Charset = charset;
            this.AddFileDependency(this.File);
            this.ParseString(this, text);
        }
        /// <summary>
        /// 根据字符串进行解析
        /// </summary>
        /// <param name="text"></param>
        public TemplateDocument(string text)
        {
            this.ParseString(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentElement"></param>
        /// <param name="text"></param>
        internal TemplateDocument(Template documentElement, string text) : this(documentElement, documentElement, text) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentElement"></param>
        /// <param name="container"></param>
        /// <param name="text"></param>
        internal TemplateDocument(Template documentElement, Tag container, string text)
        {
            this.AppendChild(documentElement);
            this.ChildTemplates.Add(documentElement);
            this.ParseString(documentElement, container, text);
        }
        #endregion

        #region 属性定义
        /// <summary>
        /// 模版文本数据
        /// </summary>
        ///public string Text { get; private set; }

        /// <summary>
        /// 根文档模版
        /// </summary>
        public Template DocumentElement
        {
            get
            {
                return this.TagContainer;
            }
            set
            {
                this.TagContainer = value;
            }
        }
        #endregion

        #region 方法定义
        /// <summary>
        /// 获取此模版文档的呈现数据
        /// </summary>
        public string GetRenderText()
        {
            using (StringWriter writer = new StringWriter())
            {
                this.Render(writer);
                return writer.ToString();
            }
        }
        #endregion

        #region 解析字符串
        /// <summary>
        /// 解析字符串
        /// </summary>
        /// <param name="text"></param>
        private void ParseString(string text)
        {
            this.ParseString(this, text);
        }
        /// <summary>
        /// 解析字符串
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <param name="text"></param>
        private void ParseString(Template ownerTemplate, string text)
        {
            this.ParseString(ownerTemplate, ownerTemplate, text);
        }
        /// <summary>
        /// 解析字符串
        /// </summary>
        /// <param name="ownerTemplate">宿主模版</param>
        /// <param name="container">标签的容器</param>
        /// <param name="text">模版文本数据</param>
        private void ParseString(Template ownerTemplate, Tag container, string text)
        {
            //设置根文档模版
            this.DocumentElement = ownerTemplate;
            //this.Text = text;

            if (string.IsNullOrEmpty(text)) return;

            int charOffset = 0, offset = 0;
            bool isClosedTag;
            Match match = null;

            //标签堆栈
            Stack<Tag> tagStack = new Stack<Tag>();
            tagStack.Push(container);

            try
            {
                while (offset < text.Length)
                {
                    if ((match = ParserRegex.VarRegex.Match(text, offset)).Success)  //匹配到模版变量
                    {
                        //构建文本节点
                        ParserHelper.CreateTextNode(ownerTemplate, container, text, charOffset, match.Index - charOffset);
                        //构建模版变量
                        ParserHelper.CreateVariableTag(ownerTemplate, container, match);
                    }
                    else if ((match = ParserRegex.TagRegex.Match(text, offset)).Success)  //匹配到某种类型的标签
                    {
                        //构建文本节点
                        ParserHelper.CreateTextNode(ownerTemplate, container, text, charOffset, match.Index - charOffset);
                        //构建标签
                        Tag tag = ParserHelper.CreateTag(ownerTemplate, match, out isClosedTag);

                        //将标签加入堆栈
                        tagStack.Push(tag);
                        //将解析权交给标签
                        bool flag = tag.ProcessBeginTag(ownerTemplate, container, tagStack, text, ref match, isClosedTag);
                        //非已闭合标签或者是单标签则处理标签的结束标签
                        if (flag)
                        {
                            tag.ProcessEndTag(ownerTemplate, tag, tagStack, text, ref match);
                        }
                        if (tagStack.Count > 0 && tagStack.Peek() == tag && isClosedTag)
                        {
                            //闭合标签则回滚一级
                            tagStack.Pop();
                        }
                        //取得容器
                        if (tagStack.Count > 0) container = tagStack.Peek();
                    }
                    else if ((match = ParserRegex.EndTagRegex.Match(text, offset)).Success)            //匹配到某个结束标签
                    {
                        //取得标签名称
                        string name = match.Groups["tagname"].Value;
                        //非匹配的结束标签.则模版有错
                        throw new ParserException("无效的结束标签");
                    }
                    //处理偏移位置
                    if (match != null && match.Success)
                    {
                        charOffset = offset = match.Index + match.Length;                       
                    }
                    else
                    {
                        offset++;
                    }
                }
                //处理文本字符
                ParserHelper.CreateTextNode(ownerTemplate, container, text, charOffset, text.Length - charOffset);
                if (tagStack.Count > 1)
                {
                    //堆栈里还有其它元素.则有错误
                    throw new ParserException(string.Format("{0}标签未闭合", tagStack.Pop()));
                }
            }
            catch (ParserException ex)
            {
                //如果错误中不包含行号与列号.则计算行号与列号
                if (!ex.HaveLineAndColumnNumber && match != null && match.Success)
                {
                    throw new ParserException(Utility.GetLineAndColumnNumber(text, match.Index), match.ToString(), ex.Message);
                }
                else
                {
                    //继续抛出错误
                    throw;
                }
            }
            finally
            {
                //清空堆栈
                tagStack.Clear();
                tagStack = null;
            }
        }
        #endregion

        #region 从文件缓存中构建模版文档对象
        /// <summary>
        /// 从文件缓存中构建模版文档对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static TemplateDocument FromFileCache(string fileName, Encoding charset)
        {
            Cache cache = HttpRuntime.Cache;
            //没有缓存则直接返回实例
            if (cache == null) return new TemplateDocument(fileName, charset);
            fileName = Path.GetFullPath(fileName);

            string cacheKey = string.Format("TEMPLATE_DOCUMENT_CACHE_ITEM_{0}", fileName);
            TemplateDocument document = cache.Get(cacheKey) as TemplateDocument;
            if (document == null)
            {
                document = new TemplateDocument(fileName, charset);
                cache.Insert(cacheKey, document, new CacheDependency(document.FileDependencies));
            }
            //返回副本
            return document.Clone();
        }
        #endregion

        #region 克隆模版文档对象
        /// <summary>
        /// 克隆模版文档对象
        /// </summary>
        /// <returns></returns>
        private TemplateDocument Clone()
        {
            return (TemplateDocument)Clone(null);
        }

        #endregion

        #region 克隆当前元素到新的宿主模版
        /// <summary>
        /// 克隆当前元素到新的宿主模版
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <returns></returns>
        internal override Element Clone(Template ownerTemplate)
        {
            TemplateDocument tag = new TemplateDocument();
            tag.Id = this.Id;
            tag.Name = this.Name;
            tag.Attributes = this.Attributes;
            tag.Charset = this.Charset;
            tag.File = this.File;
            tag.fileDependencies = this.fileDependencies;
            tag.Visible = this.Visible;
            //优先克隆变量
            foreach (Variable var in this.Variables)
            {
                tag.Variables.Add(var.Clone(tag));
            }

            foreach (Element element in this.InnerElements)
            {
                Element item = element.Clone(tag);
                tag.AppendChild(item);
                if (item is Template)
                {
                    //加入子模版列表
                    tag.ChildTemplates.Add((Template)item);
                }

                if (this.DocumentElement == element) tag.DocumentElement = (Template)item;
            }

            if (tag.DocumentElement == null) tag.DocumentElement = tag;
            return tag;
        }
        #endregion
    }
}