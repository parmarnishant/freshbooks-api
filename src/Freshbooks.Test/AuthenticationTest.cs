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
using System.Diagnostics;
using Freshbooks.Test.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Freshbooks.Library;
using Freshbooks.Test.Properties;

namespace Freshbooks.Test
{
    [TestClass]
    public class AuthenticationTest
    {
        internal static FreshbooksApi Default = CreateDefault();
        static FreshbooksApi CreateDefault()
        {
            FreshbooksApi api = new FreshbooksApi(UserSettings.FreshbooksAccountName, UserSettings.ConsumerKey);
            api.UseLegacyToken(UserSettings.UserToken);
            return api;
        }

        [TestMethod]
        public void DefaultAuthTest()
        {
            Default.Staff.Current();
        }

        [TestMethod]
        public void TokenBasedAuthTest()
        {
            FreshbooksApi api = new FreshbooksApi(UserSettings.FreshbooksAccountName, UserSettings.ConsumerKey);
            api.UseLegacyToken(UserSettings.UserToken);
            api.Staff.Current();
        }

        [TestMethod]
        [Ignore]
        public void OAuthInBrowserTestWithoutIdentity()
        {
            FreshbooksApi api = new FreshbooksApi(UserSettings.FreshbooksAccountName, UserSettings.ConsumerKey);
            Uri redir;

            using (HttpCallback callback = new HttpCallback())
            {
                api.OAuthCallback = callback.CallbackUri;
                Process.Start(api.GetAuthroizationUrl().AbsoluteUri);

                callback.WaitForCallback(120000);
                redir = callback.LastRequest;
            }

            api.AuthorizeToken(redir);
            api.Staff.Current();
        }

        [TestMethod]
        [Ignore]
        public void OAuthInBrowserTestWithConsumerIdentity()
        {
            FreshbooksApi api = new FreshbooksApi(UserSettings.FreshbooksAccountName, UserSettings.ConsumerKey, UserSettings.OAuthSecret);
            Uri redir;

            using (HttpCallback callback = new HttpCallback())
            {
                api.OAuthCallback = callback.CallbackUri;
                Process.Start(api.GetAuthroizationUrl().AbsoluteUri);

                callback.WaitForCallback(120000);
                redir = callback.LastRequest;
            }

            api.AuthorizeToken(redir);
            api.Staff.Current();
        }
    }
}
