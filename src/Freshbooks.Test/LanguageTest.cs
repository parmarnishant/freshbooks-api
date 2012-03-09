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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Freshbooks.Library.Model;
using System.Collections.Generic;

namespace Freshbooks.Test
{
    [TestClass]
    public class LanguageTest
    {
        static ILanguageService Service { get { return AuthenticationTest.Default.Language; } }

        [TestMethod]
        public void GetListTest()
        {
            LanguagesResponse list = Service.List(new LanguagesRequest());
            Assert.IsTrue(list.Languages.LanguageList.Count > 0);
        }

        [TestMethod]
        public void GetListPaginatedTest()
        {
            LanguagesResponse list = Service.List(new LanguagesRequest() { PerPage = 1 });

            Assert.AreEqual(1u, list.Languages.Page);
            Assert.AreEqual(1u, list.Languages.PerPage);
            Assert.IsTrue(list.Languages.Total > 3);
            Assert.AreEqual(list.Languages.Total, list.Languages.Pages);

            uint total = list.Languages.Total;
            uint per_page = total/3;
            uint page_cnt = (total + per_page - 1) / per_page;

            Dictionary<string, Language> all = new Dictionary<string, Language>();
            for(uint page=1; page <= page_cnt; page++)
            {
                list = Service.List(new LanguagesRequest() { PerPage = per_page, Page = page });
                Assert.AreEqual(page, list.Languages.Page);
                foreach (Language lang in list.Languages.LanguageList)
                    all.Add(lang.Name, lang);
            }

            Assert.AreEqual(total, (uint)all.Count);
            foreach (Language lang in Service.List(new LanguagesRequest() { PerPage = total }).Languages.LanguageList)
                Assert.IsTrue(all.ContainsKey(lang.Name));
        }
    }
}
