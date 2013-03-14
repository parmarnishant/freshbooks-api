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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Freshbooks.Library.Model;
using System.Collections.Generic;

namespace Freshbooks.Test
{
    [TestClass]
    public class PaymentTest
    {
        static IPaymentService Service { get { return AuthenticationTest.Default.Payment; } }
        private ClientId _sampleClient;
        private ClientId ClientId { get { return _sampleClient.HasValue ? _sampleClient : (_sampleClient = ClientTest.SampleClient().ClientId); } }

        [TestMethod]
        public void CreatePaymentTest()
        {
            Payment rec = new Payment { ClientId = ClientId, Amount = 0.01, Type = "Check" };

            PaymentIdentity id = Service.Create(new PaymentRequest {Payment = rec});

            try
            {
                Payment fetched = Service.Get(id).Payment;
                Assert.AreEqual(ClientId, fetched.ClientId);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void UpdatePaymentTest()
        {
            Payment rec = new Payment { ClientId = ClientId, Amount = 0.01, Type = "Check" };

            PaymentIdentity id = Service.Create(new PaymentRequest { Payment = rec });

            try
            {
                rec = Service.Get(id).Payment;
                Assert.AreEqual(ClientId, rec.ClientId);

                Service.Update(
                    new PaymentUpdateRequest
                        {
                            Payment =
                                new PaymentUpdate(rec)
                                    {
                                        Notes = "Nunit update test",
                                    }
                        });

                Payment fetched = Service.Get(id).Payment;
                Assert.AreEqual(ClientId, rec.ClientId);
                Assert.AreEqual("Nunit update test", fetched.Notes);
            }
            finally
            {
                Service.Delete(id);
            }
        }
      
        [TestMethod]
        public void ListPaymentsTest()
        {
            Payment rec = new Payment { ClientId = ClientId };

            PaymentIdentity id = Service.Create(new PaymentRequest { Payment = rec });

            try
            {
                IList<Payment> items =
                    Service.List(PaymentsRequest.DefaultInstance).Payments.PaymentList;
                Assert.AreNotEqual(0, items.Count);

                Assert.AreEqual(1, items.Count(x => x.PaymentId == id.PaymentId));
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void DeletePaymentTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Service.Create(new PaymentRequest { Payment = new Payment { ClientId = ClientId } });

            IList<Payment> items = Service.List(new PaymentsRequest() { ClientId = ClientId }).Payments.PaymentList;
            Assert.AreNotEqual(0, items.Count);

            foreach (Payment rec in items.Where(i => i.ClientId == ClientId))
                Service.Delete(new PaymentIdentity {PaymentId = rec.PaymentId});

            items = Service.List(new PaymentsRequest()).Payments.PaymentList;
            Assert.AreEqual(0, items.Count(i => i.ClientId == ClientId));
        }
    }
}
