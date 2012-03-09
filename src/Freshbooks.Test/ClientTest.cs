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
    public class ClientTest
    {
        const string SampleClientEmail = "sample.client@example.com";
        static IClientService Service { get { return AuthenticationTest.Default.Client; } }

        internal static Client SampleClient()
        {
            foreach (Client item in Service.List(new ClientsRequest { Email = SampleClientEmail }).Clients.ClientList)
                return item;

            ClientIdentity id = Service.Create(
                new ClientRequest
                    {
                        Client = new Client
                                     {
                                         FirstName = "Sample",
                                         LastName = "Client",
                                         Email = SampleClientEmail,
                                         Organization = "Samples",
                                         Notes = "Used for testing related records."
                                     }
                    });

            return Service.Get(id).Client;
        }

        [TestMethod]
        public void CreateClientTest()
        {
            Client rec = new Client
                             {
                                 FirstName = "Test",
                                 LastName = Guid.NewGuid().ToString("N"),
                                 Email = Guid.NewGuid().ToString("N") + "@example.com",
                                 Organization = "Testing",
                             };

            ClientIdentity id = Service.Create(new ClientRequest {Client = rec});

            try
            {
                Client fetched = Service.Get(id).Client;
                Assert.AreEqual(rec.LastName, fetched.LastName);
                Assert.AreEqual(id.ClientId, fetched.ClientId);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void UpdateClientTest()
        {
            Client rec = new Client
            {
                FirstName = "Test",
                LastName = Guid.NewGuid().ToString("N"),
                Email = Guid.NewGuid().ToString("N") + "@example.com",
                Organization = "Testing",
            };

            ClientIdentity id = Service.Create(new ClientRequest { Client = rec });

            try
            {
                rec = Service.Get(id).Client;
                rec.WorkPhone = "1-800-776-5433";

                Service.Update(new ClientRequest { Client = rec });

                Client copy = Service.Get(id).Client;
                Assert.AreEqual(rec.WorkPhone, copy.WorkPhone);
                Assert.AreEqual("1-800-776-5433", copy.WorkPhone);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void ListClientsTest()
        {
            Client rec = new Client
            {
                FirstName = "Test",
                LastName = Guid.NewGuid().ToString("N"),
                Email = Guid.NewGuid().ToString("N") + "@example.com",
                Organization = "Testing",
            };

            ClientIdentity id = Service.Create(new ClientRequest { Client = rec });

            try
            {
                IList<Client> clients =
                    Service.List(new ClientsRequest()).Clients.ClientList;
                Assert.AreNotEqual(0, clients.Count);

                Assert.AreEqual(1, clients.Count(x => x.ClientId == id.ClientId));
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void DeleteClientTest()
        {
            Client rec = new Client
            {
                FirstName = "Test",
                LastName = Guid.NewGuid().ToString("N"),
                Email = Guid.NewGuid().ToString("N") + "@example.com",
                Organization = "Testing",
            };

            Service.Create(new ClientRequest { Client = rec });

            IList<Client> clients = Service.List(new ClientsRequest()).Clients.ClientList;
            Assert.AreNotEqual(0, clients.Count(c => c.FirstName == "Test"));

            foreach (Client item in clients.Where(c => c.FirstName == "Test"))
                Service.Delete(new ClientIdentity { ClientId = item.ClientId });

            clients = Service.List(new ClientsRequest()).Clients.ClientList;
            Assert.AreEqual(0, clients.Count(c => c.FirstName == "Test"));
        }
    }
}
