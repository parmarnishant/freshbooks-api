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
    public class TaxTest
    {
        static ITaxService Service { get { return AuthenticationTest.Default.Tax; } }

        static string NewName() { return Guid.NewGuid().ToString("N").Substring(0, 10); }

        [TestMethod]
        public void CreateTaxTest()
        {
            string name = NewName();
            Tax rec = new Tax {Name = name};

            TaxIdentity id = Service.Create(new TaxRequest {Tax = rec});

            try
            {
                Tax fetched = Service.Get(id).Tax;
                Assert.AreEqual(name, fetched.Name);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void UpdateTaxTest()
        {
            string name = NewName();
            Tax rec = new Tax { Name = name };

            TaxIdentity id = Service.Create(new TaxRequest { Tax = rec });

            try
            {
                rec = Service.Get(id).Tax;
                Assert.AreEqual(name, rec.Name);

                Service.Update(
                    new TaxRequest
                        {
                            Tax =
                                new Tax(rec)
                                    {
                                        Number = "6277272474",
                                        Compound = true,
                                        Rate = 5.25,
                                    }
                        });

                Tax fetched = Service.Get(id).Tax;
                Assert.AreEqual(name, fetched.Name);
                Assert.AreEqual("6277272474", fetched.Number);
                Assert.AreEqual(true, fetched.Compound);
                Assert.AreEqual(5.25, fetched.Rate);
            }
            finally
            {
                Service.Delete(id);
            }
        }
      
        [TestMethod]
        public void ListTaxesTest()
        {
            string name = NewName();
            Tax rec = new Tax { Name = name };

            TaxIdentity id = Service.Create(new TaxRequest { Tax = rec });

            try
            {
                IList<Tax> items =
                    Service.List(new TaxesRequest()).Taxes.TaxList;
                Assert.AreNotEqual(0, items.Count);

                Assert.AreEqual(1, items.Count(x => x.TaxId == id.TaxId));
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void DeleteTaxTest()
        {
            string name = NewName();
            Service.Create(new TaxRequest { Tax = new Tax { Name = name } });

            IList<Tax> items = Service.List(new TaxesRequest()).Taxes.TaxList;
            Assert.AreNotEqual(0, items.Count);

            foreach (Tax rec in items)
                Service.Delete(new TaxIdentity {TaxId = rec.TaxId});

            items = Service.List(new TaxesRequest()).Taxes.TaxList;
            Assert.AreEqual(0, items.Count);
        }
    }
}
