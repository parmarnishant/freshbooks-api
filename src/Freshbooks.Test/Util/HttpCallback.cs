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
using System.Net;
using Freshbooks.Test.Properties;
using System.IO;
using System.Threading;

namespace Freshbooks.Test.Util
{
    class HttpCallback  : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly AutoResetEvent _wait;

        public HttpCallback()
        {
            _wait = new AutoResetEvent(false);

            _listener = new HttpListener();
            _listener.Prefixes.Add(CallbackUri);
            _listener.Start();
            _listener.BeginGetContext(OnCallback, null);
        }

        public Uri LastRequest { get; private set; }

        public string CallbackUri
        {
            get
            {
                return new UriBuilder
                           {
                               Scheme = "http",
                               Host = "localhost",
                               Port = UserSettings.HttpCallbackPort
                           }.Uri.AbsoluteUri;
            }
        }

        public event Action<HttpListenerContext> Callback;

        public void Dispose()
        {
            _listener.Close();
        }

        private void OnCallback(IAsyncResult ar)
        {
            HttpListenerContext context;
            try
            {
                context = _listener.EndGetContext(ar);
            }
            catch (ObjectDisposedException) { return; }

            if (Callback != null)
                Callback(context);
            else
            {
                Uri reqUri = context.Request.Url;
                LastRequest = reqUri;

                using (StreamWriter output = new StreamWriter(context.Response.OutputStream))
                {
                    output.WriteLine("<html>");
                    output.WriteLine("<body>You can close this window.");
                    output.WriteLine("<script>\r\n  window.open('', '_self', '');\r\n  window.close();\r\n</script>");
                    output.WriteLine("</body></html>");
                }
            }

            _wait.Set();
        }

        public void WaitForCallback(int timeout)
        {
            if (!_wait.WaitOne(timeout, false))
                throw new TimeoutException();
        }
    }
}
