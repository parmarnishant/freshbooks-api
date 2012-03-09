#region Copyright (c) 2012 SmartVault, Inc.
//  Permission is hereby granted, free of charge, to any person obtaining a copy 
//  of this software and associated documentation files (the "Software"), to deal 
//  in the Software without restriction, including without limitation the rights 
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
//  copies of the Software, and to permit persons to whom the Software is 
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//  IN THE SOFTWARE.
#endregion
using System;
using System.IO;
using System.Text;
using System.Xml;
using NClassify.Library;
using Freshbooks.Library.Model;
using Freshbooks.Library.Properties;

namespace Freshbooks.Library
{
    internal class ServiceProxy : IDispatchStub
    {
        private readonly FreshbooksApi _api;
        private readonly string _namePrefix;
        private readonly XmlWriterSettings _xmlSettings;

        public ServiceProxy(FreshbooksApi api, string typename)
        {
            _api = api;
            _namePrefix = typename + ".";
            _xmlSettings = new XmlWriterSettings
                               {
                                   Indent = true,
                                   IndentChars = "\t",
                                   Encoding = new UTF8Encoding(false),
                                   CloseOutput = false, 
                                   OmitXmlDeclaration = true,
                               };
        }

        public void Dispose() { }

        public void CallMethod<TRequest, TResponse>(string method, TRequest request, TResponse response)
            where TRequest : class, IMessage
            where TResponse : class, IBuilder
        {
            string xmlRequest = CreateRequest(method, request);
            string xmlResponse = _api.Post(xmlRequest);

            ParseResponse(xmlResponse, response);
        }

        private string CreateRequest(string method, IMessage request)
        {
            request.AssertValid();
            using (StringWriter sw = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sw, _xmlSettings))
                {
                    writer.WriteStartElement("request");
                    writer.WriteAttributeString("method", _namePrefix + method);
                    request.MergeTo(writer);
                    writer.WriteFullEndElement();
                    writer.Flush();
                }
                string xmlRequest = sw.ToString();
                //System.Diagnostics.Debug.WriteLine(xmlRequest, _api.AccountName);
                return xmlRequest;
            }
        }

        private void ParseResponse(string xmlResponse, IBuilder builder)
        {
            //System.Diagnostics.Debug.WriteLine(xmlResponse, _api.AccountName);

            using (XmlReader xrdr = XmlReader.Create(new StringReader(xmlResponse)))
            {
                while (!xrdr.EOF && !xrdr.IsStartElement("response"))
                    xrdr.Read();

                string responseType = xrdr.GetAttribute("status");
                if (!StringComparer.OrdinalIgnoreCase.Equals(responseType, "ok"))
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(responseType, "fail"))
                    {
                        xrdr.Read();
                        ErrorResponse msg = new ErrorResponse();
                        msg.MergeFrom(xrdr);

                        if (msg.HasCode && msg.ErrorList.Count > 0)
                            throw new FreshbooksServerException(unchecked((int) msg.Code), msg.ErrorList[0]);
                    }
                    throw new UnknownServerException();
                }

                xrdr.Read();
                builder.MergeFrom(xrdr);
            }

            builder.AssertValid();
        }
    }
}
