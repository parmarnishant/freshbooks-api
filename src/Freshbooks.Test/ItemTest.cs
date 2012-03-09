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
    public class ItemTest
    {
        static IItemService Service { get { return AuthenticationTest.Default.Item; } }

        [TestMethod]
        public void CreateItemTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Item rec = new Item {Name = name};

            ItemIdentity id = Service.Create(new ItemRequest {Item = rec});

            try
            {
                Item fetched = Service.Get(id).Item;
                Assert.AreEqual(name, fetched.Name);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void UpdateItemTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Item rec = new Item { Name = name };

            ItemIdentity id = Service.Create(new ItemRequest { Item = rec });

            try
            {
                rec = Service.Get(id).Item;
                Assert.AreEqual(name, rec.Name);

                Service.Update(
                    new ItemRequest
                        {
                            Item =
                                new Item(rec)
                                    {
                                        Description = "A widget for the whatsit",
                                        Inventory = 10,
                                        Quantity = 5,
                                        UnitCost = 12.34,
                                    }
                        });

                Item fetched = Service.Get(id).Item;
                Assert.AreEqual(name, fetched.Name);
                Assert.AreEqual("A widget for the whatsit", fetched.Description);
                Assert.AreEqual(10.0, fetched.Inventory);
                Assert.AreEqual(5.0, fetched.Quantity);
                Assert.AreEqual(12.34, fetched.UnitCost);
            }
            finally
            {
                Service.Delete(id);
            }
        }
      
        [TestMethod]
        public void ListItemsTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Item rec = new Item { Name = name };

            ItemIdentity id = Service.Create(new ItemRequest { Item = rec });

            try
            {
                IList<Item> items =
                    Service.List(new ItemsRequest()).Items.ItemList;
                Assert.AreNotEqual(0, items.Count);

                Assert.AreEqual(1, items.Count(x => x.ItemId == id.ItemId));
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void DeleteItemTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Service.Create(new ItemRequest { Item = new Item { Name = name } });

            IList<Item> items = Service.List(new ItemsRequest()).Items.ItemList;
            Assert.AreNotEqual(0, items.Count);

            foreach (Item rec in items)
                Service.Delete(new ItemIdentity {ItemId = rec.ItemId});

            items = Service.List(new ItemsRequest()).Items.ItemList;
            Assert.AreEqual(0, items.Count);
        }
    }
}
