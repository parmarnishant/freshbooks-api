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

namespace Freshbooks.Library
{
    public sealed class ServerErrorEventArgs : EventArgs
    {
        private readonly HttpStatusCode _hstatus;
        private readonly Uri _uri;
        private readonly string _request;
        private readonly Exception _exception;

        public ServerErrorEventArgs(HttpStatusCode hstatus, Uri uri, string request, Exception exception)
        {
            _hstatus = hstatus;
            _uri = uri;
            _request = request;
            _exception = exception;
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        public HttpStatusCode Status
        {
            get { return _hstatus; }
        }

        public string Request
        {
            get { return _request; }
        }

        public Exception GetException()
        {
            return _exception;
        }
    }

    public sealed class BeginRequestEventArgs : EventArgs
    {
        private readonly Uri _uri;
        private readonly string _request;

        public BeginRequestEventArgs(Uri uri, string request)
        {
            _uri = uri;
            _request = request;
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        public string Request
        {
            get { return _request; }
        }
    }

    public sealed class CompleteRequestEventArgs : EventArgs
    {
        private readonly HttpStatusCode _hstatus;
        private readonly Uri _uri;
        private readonly string _request;
        private readonly string _response;

        public CompleteRequestEventArgs(HttpStatusCode hstatus, Uri uri, string request, string response)
        {
            _hstatus = hstatus;
            _uri = uri;
            _request = request;
            _response = response;
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        public HttpStatusCode Status
        {
            get { return _hstatus; }
        }

        public string Request
        {
            get { return _request; }
        }

        public string Response
        {
            get { return _response; }
        }
    }
}
