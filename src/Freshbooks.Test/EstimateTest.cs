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
using System;

namespace Freshbooks.Test
{
    [TestClass]
    public class EstimateTest
    {
        static IEstimateService Service { get { return AuthenticationTest.Default.Estimate; } }

        [TestMethod]
        public void CreateEstimateTest()
        {
            Estimate rec =
                new Estimate
                    {
                        ClientId = ClientTest.SampleClient().ClientId,
                        PoNumber = "1234",
                        Date = DateTime.Now.Date,
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

            EstimateIdentity id = Service.Create(new EstimateRequest {Estimate = rec,});

            try
            {
                Estimate fetched = Service.Get(id).Estimate;
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
        public void UpdateEstimateTest()
        {
            EstimateIdentity id = Service.Create(
                new EstimateRequest
                    {
                        Estimate =
                            new Estimate
                                {
                                    ClientId = ClientTest.SampleClient().ClientId,
                                    PoNumber = "1234",
                                }
                    });

            try
            {
                Estimate rec = Service.Get(id).Estimate;
                Assert.AreEqual(0.0, rec.Amount);

                Service.Update(
                    new EstimateRequest
                        {
                            Estimate =
                                new Estimate(rec)
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

                Estimate copy = Service.Get(id).Estimate;
                Assert.AreEqual(22.44, copy.Amount);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void ListEstimatesTest()
        {
            EstimateIdentity id = Service.Create(
                new EstimateRequest
                    {
                        Estimate =
                            new Estimate
                                {
                                    ClientId = ClientTest.SampleClient().ClientId,
                                    PoNumber = "1234",
                                }
                    });

            try
            {
                IList<Estimate> items =
                    Service.List(new EstimatesRequest()).Estimates.EstimateList;
                Assert.AreNotEqual(0, items.Count);

                Assert.AreEqual(1, items.Count(x => x.EstimateId == id.EstimateId));
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void DeleteEstimateTest()
        {
            Service.Create(
                new EstimateRequest
                    {
                        Estimate =
                            new Estimate
                                {
                                    ClientId = ClientTest.SampleClient().ClientId,
                                    PoNumber = "1234",
                                }
                    });

            IList<Estimate> items = Service.List(new EstimatesRequest()).Estimates.EstimateList;
            Assert.AreNotEqual(0, items.Count);

            foreach (Estimate rec in items)
                Service.Delete(new EstimateIdentity { EstimateId = rec.EstimateId });

            items = Service.List(new EstimatesRequest()).Estimates.EstimateList;
            Assert.AreEqual(0, items.Count);
        }

        [TestMethod]
        public void SendEstimateTest()
        {
            EstimateIdentity id = Service.Create(
                new EstimateRequest
                    {
                        Estimate =
                            new Estimate
                                {
                                    ClientId = ClientTest.SampleClient().ClientId,
                                    PoNumber = "1234",
                                }
                    });

            try
            {
                Service.SendByEmail(
                    new EstimateEmailRequest
                        {
                            EstimateId = id.EstimateId,
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
