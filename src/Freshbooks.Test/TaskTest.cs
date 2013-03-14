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
    public class TaskTest
    {
        static ITaskService Service { get { return AuthenticationTest.Default.Task; } }

        [TestMethod]
        public void CreateTaskTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Task rec = new Task {Name = name};

            TaskIdentity id = Service.Create(new TaskRequest {Task = rec});

            try
            {
                Task fetched = Service.Get(id).Task;
                Assert.AreEqual(name, fetched.Name);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void UpdateTaskTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Task rec = new Task { Name = name };

            TaskIdentity id = Service.Create(new TaskRequest { Task = rec });

            try
            {
                rec = Service.Get(id).Task;
                Assert.AreEqual(name, rec.Name);

                Service.Update(
                    new TaskRequest
                        {
                            Task =
                                new Task(rec)
                                    {
                                        Description = "A widget for the whatsit",
                                        Billable = true,
                                        Rate = 5.25,
                                    }
                        });

                Task fetched = Service.Get(id).Task;
                Assert.AreEqual(name, fetched.Name);
                Assert.AreEqual("A widget for the whatsit", fetched.Description);
                Assert.AreEqual(true, fetched.Billable);
                Assert.AreEqual(5.25, fetched.Rate);
            }
            finally
            {
                Service.Delete(id);
            }
        }
      
        [TestMethod]
        public void ListTasksTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Task rec = new Task { Name = name };

            TaskIdentity id = Service.Create(new TaskRequest { Task = rec });

            try
            {
                IList<Task> items =
                    Service.List(new TasksRequest()).Tasks.TaskList;
                Assert.AreNotEqual(0, items.Count);

                Assert.AreEqual(1, items.Count(x => x.TaskId == id.TaskId));
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void DeleteTaskTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Service.Create(new TaskRequest { Task = new Task { Name = name } });

            IList<Task> items = Service.List(new TasksRequest()).Tasks.TaskList;
            Assert.AreNotEqual(0, items.Count);

            foreach (Task rec in items)
                Service.Delete(new TaskIdentity {TaskId = rec.TaskId});

            items = Service.List(new TasksRequest()).Tasks.TaskList;
            Assert.AreEqual(0, items.Count);
        }
    }
}
