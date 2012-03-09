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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Freshbooks.Library.Model;
using System.Collections.Generic;

namespace Freshbooks.Test
{
    [TestClass]
    public class CallbackTest
    {
        static ICallbackService Service { get { return AuthenticationTest.Default.Callback; } }
        
        [TestMethod]
        public void CreateAndDeleteCallbackTest()
        {
            Callback cb = new Callback
                              {
                                Event = new EventType("category.delete"),
                                Uri = new Url("http://example.com")
                              };

            CallbackIdentity id = Service.Create(new CallbackRequest { Callback = cb });
            Service.Delete(id);
        }

        [TestMethod]
        public void CreateAndListTest()
        {
            Callback cb = new Callback
            {
                Event = new EventType("category.delete"),
                Uri = new Url("http://example.com")
            };

            CallbackIdentity id = Service.Create(new CallbackRequest { Callback = cb });

            try
            {
                Assert.AreEqual(1, AuthenticationTest.Default.
                                       Callback.List(new CallbacksRequest())
                                       .Callbacks.CallbackList
                                       .Count(x => x.CallbackId == id.CallbackId));
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void DeleteCallbackTest()
        {
            Service.Create(new CallbackRequest
            {
                Callback = new Callback
                {
                    Event = new EventType("category.delete"),
                    Uri = new Url("http://example.com")
                }
            });

            IList<Callback> callbacks = Service.List(new CallbacksRequest()).Callbacks.CallbackList;
            Assert.AreNotEqual(0, callbacks.Count);

            foreach (Callback cb in callbacks)
                Service.Delete(new CallbackIdentity {CallbackId = cb.CallbackId});

            callbacks = Service.List(new CallbacksRequest()).Callbacks.CallbackList;
            Assert.AreEqual(0, callbacks.Count);
        }
    }
}
