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
    public class InvoiceTest
    {
        static IInvoiceService Service { get { return AuthenticationTest.Default.Invoice; } }

        [TestMethod]
        public void CreateInvoiceTest()
        {
            Invoice rec =
                new Invoice
                    {
                        ClientId = ClientTest.SampleClient().ClientId,
                        PoNumber = "1234",
                        Lines = new LineItems
                        {
                            LineList =
                                {
                                    new LineItem
                                        {
                                            Name = "Widget 1.0a (revision 3B)",
                                            Description = "A widget for the whatsit",
                                            UnitCost = 22.44,
                                            Quantity = 100,
                                        },
                                    new LineItem
                                        {
                                            Name = "Monarch 2",
                                            Description = "A pretty little butterfly",
                                            UnitCost = 123.56,
                                            Quantity = 1,
                                        }
                                }
                        }
                    };

            InvoiceIdentity id = Service.Create(new InvoiceRequest {Invoice = rec,});

            try
            {
                Invoice fetched = Service.Get(id).Invoice;
                Assert.AreEqual("1234", fetched.PoNumber);
                Assert.AreEqual(2367.56, fetched.Amount);
                Assert.AreEqual(2, fetched.Lines.LineList.Count);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void UpdateInvoiceTest()
        {
            InvoiceIdentity id = Service.Create(
                new InvoiceRequest
                    {
                        Invoice =
                            new Invoice
                                {
                                    ClientId = ClientTest.SampleClient().ClientId,
                                    PoNumber = "1234",
                                }
                    });

            try
            {
                Invoice rec = Service.Get(id).Invoice;
                Assert.AreEqual(0.0, rec.Amount);

                Service.Update(
                    new InvoiceRequest
                        {
                            Invoice =
                                new Invoice(rec)
                                    {
                                        Lines =
                                            new LineItems
                                                {
                                                    LineList =
                                                        {
                                                            new LineItem
                                                                {
                                                                    Name = "Widget 1.0a (revision 3B)",
                                                                    Description = "A widget for the whatsit",
                                                                    UnitCost = 22.44,
                                                                    Quantity = 1,
                                                                }
                                                        }
                                                }
                                    }
                        });

                Invoice copy = Service.Get(id).Invoice;
                Assert.AreEqual(22.44, copy.Amount);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void ListInvoicesTest()
        {
            InvoiceIdentity id = Service.Create(
                new InvoiceRequest
                    {
                        Invoice =
                            new Invoice
                                {
                                    ClientId = ClientTest.SampleClient().ClientId,
                                    PoNumber = "1234",
                                }
                    });

            try
            {
                IList<Invoice> items =
                    Service.List(new InvoicesRequest()).Invoices.InvoiceList;
                Assert.AreNotEqual(0, items.Count);

                Assert.AreEqual(1, items.Count(x => x.InvoiceId == id.InvoiceId));
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void DeleteInvoiceTest()
        {
            Service.Create(
                new InvoiceRequest
                    {
                        Invoice =
                            new Invoice
                                {
                                    ClientId = ClientTest.SampleClient().ClientId,
                                    PoNumber = "1234",
                                }
                    });

            IList<Invoice> items = Service.List(new InvoicesRequest()).Invoices.InvoiceList;
            Assert.AreNotEqual(0, items.Count);

            foreach (Invoice rec in items)
                Service.Delete(new InvoiceIdentity { InvoiceId = rec.InvoiceId });

            items = Service.List(new InvoicesRequest()).Invoices.InvoiceList;
            Assert.AreEqual(0, items.Count);
        }

        [TestMethod]
        public void SendInvoiceTest()
        {
            InvoiceIdentity id = Service.Create(
                new InvoiceRequest
                    {
                        Invoice =
                            new Invoice
                                {
                                    ClientId = ClientTest.SampleClient().ClientId,
                                    PoNumber = "1234",
                                }
                    });

            try
            {
                Service.SendByEmail(
                    new InvoiceEmailRequest
                        {
                            InvoiceId = id.InvoiceId,
                            Subject = "This is a sample subject",
                            Message = "This is the body of the sample message",
                        }
                    );
            }
            finally
            {
                Service.Delete(id);
            }
        }
    }
}
