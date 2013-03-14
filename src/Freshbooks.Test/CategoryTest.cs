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
    public class CategoryTest
    {
        const string SampleCategoryName = "Sample Category";
        static ICategoryService Service { get { return AuthenticationTest.Default.Category; } }

        internal static Category SampleCategory()
        {
            foreach (Category c in Service.List(new CategoriesRequest()).Categories
                .CategoryList.Where(c => c.Name == SampleCategoryName))
                return c;

            return new Category
                       {
                           Name = SampleCategoryName,
                           CategoryId = Service.Create(
                               new CategoryRequest { Category = new Category { Name = SampleCategoryName } }
                               ).CategoryId,
                       };
        }

        [TestMethod]
        public void CreateCategoryTest()
        {
            string randomName = Guid.NewGuid().ToString("N");
            Category cb = new Category { Name = randomName };

            CategoryIdentity id = Service.Create(
                new CategoryRequest {Category = cb});

            try
            {
                Category fetched = Service.Get(id).Category;
                Assert.AreEqual(randomName, fetched.Name);
                Assert.AreEqual(id.CategoryId, fetched.CategoryId);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void UpdateCategoryTest()
        {
            string name = Guid.NewGuid().ToString("N");
            CategoryIdentity id = Service.Create(
                               new CategoryRequest {Category = new Category {Name = name}}
                               );

            try
            {
                Category cat = Service.Get(id).Category;
                string newName = Guid.NewGuid().ToString("N");

                Service.Update(new CategoryRequest
                                   {
                                       Category = new Category { Name = newName, CategoryId = cat.CategoryId }
                                   });

                Category copy = Service.Get(id).Category;
                Assert.AreNotEqual(cat.Name, copy.Name);
                Assert.AreEqual(newName, copy.Name);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void ListCategoriesTest()
        {
            string name = Guid.NewGuid().ToString("N");
            CategoryIdentity id = Service.Create(
                               new CategoryRequest { Category = new Category { Name = name } }
                               );
            try
            {
                IList<Category> categories =
                    Service.List(new CategoriesRequest()).Categories.CategoryList;
                Assert.AreNotEqual(0, categories.Count);

                Assert.AreEqual(1, categories.Count(x => x.CategoryId == id.CategoryId));
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void DeleteCategoryTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Service.Create(new CategoryRequest { Category = new Category { Name = name } });

            IList<Category> categories = Service.List(new CategoriesRequest()).Categories.CategoryList;
            categories = new List<Category>(categories.Where(c => c.Name != SampleCategoryName));
            Assert.AreNotEqual(0, categories.Count);

            foreach (Category cat in categories)
                Service.Delete(new CategoryIdentity {CategoryId = cat.CategoryId});

            categories = Service.List(new CategoriesRequest()).Categories.CategoryList;
            Assert.AreEqual(0, categories.Count(c => c.Name != SampleCategoryName));
        }
    }
}
