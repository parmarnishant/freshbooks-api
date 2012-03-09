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
    public class ExpenseTest
    {
        static IExpenseService Service { get { return AuthenticationTest.Default.Expense; } }

        [TestMethod]
        public void CreateExpenseTest()
        {
            var client = ClientTest.SampleClient().ClientId;
            double amt = new Random().Next(10000)/100.0;
            Expense rec = new Expense
                              {
                                  StaffId = StaffTest.Self.StaffId,
                                  ClientId = client,
                                  Amount = amt,
                                  CategoryId = CategoryTest.SampleCategory().CategoryId,
                              };

            ExpenseIdentity id = Service.Create(new ExpenseRequest {Expense = rec});

            try
            {
                Expense fetched = Service.Get(id).Expense;
                Assert.AreEqual(amt, fetched.Amount);
                Assert.AreEqual(client, fetched.ClientId);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void UpdateExpenseTest()
        {
            var client = ClientTest.SampleClient().ClientId;
            double amt = new Random().Next(10000)/100.0;

            ExpenseIdentity id = Service.Create(
                new ExpenseRequest
                    {
                        Expense =
                            new Expense
                                {
                                    StaffId = StaffTest.Self.StaffId,
                                    ClientId = client,
                                    Amount = amt,
                                    CategoryId = CategoryTest.SampleCategory().CategoryId,
                                }
                    });

            try
            {
                Expense rec = Service.Get(id).Expense;
                Assert.AreEqual(amt, rec.Amount);

                Service.Update(
                    new ExpenseRequest { Expense = new Expense(rec) { Amount = amt * 10.0 }}
                    );

                Expense copy = Service.Get(id).Expense;
                Assert.AreEqual((amt * 10.0).ToString("f2"), copy.Amount.ToString("f2"));
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void ListExpensesTest()
        {
            var client = ClientTest.SampleClient().ClientId;
            double amt = new Random().Next(10000)/100.0;

            ExpenseIdentity id = Service.Create(
                new ExpenseRequest
                    {
                        Expense =
                            new Expense
                                {
                                    StaffId = StaffTest.Self.StaffId,
                                    ClientId = client,
                                    Amount = amt,
                                    CategoryId = CategoryTest.SampleCategory().CategoryId,
                                }
                    });

            try
            {
                IList<Expense> items =
                    Service.List(new ExpensesRequest {ClientId = client}).Expenses.ExpenseList;
                Assert.AreNotEqual(0, items.Count);

                Assert.AreEqual(1, items.Count(x => x.ExpenseId == id.ExpenseId));
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void DeleteExpenseTest()
        {
            var client = ClientTest.SampleClient().ClientId;
            double amt = new Random().Next(10000)/100.0;

            Service.Create(
                new ExpenseRequest
                    {
                        Expense =
                            new Expense
                                {
                                    StaffId = StaffTest.Self.StaffId,
                                    ClientId = client,
                                    Amount = amt,
                                    CategoryId = CategoryTest.SampleCategory().CategoryId,
                                }
                    });

            IList<Expense> items = Service.List(new ExpensesRequest()).Expenses.ExpenseList;
            Assert.AreNotEqual(0, items.Count);

            foreach (Expense rec in items)
                Service.Delete(new ExpenseIdentity {ExpenseId = rec.ExpenseId});

            items = Service.List(new ExpensesRequest()).Expenses.ExpenseList;
            Assert.AreEqual(0, items.Count);
        }
    }
}
