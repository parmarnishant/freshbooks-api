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
    public class ProjectTest
    {
        static IProjectService Service { get { return AuthenticationTest.Default.Project; } }
        static ITaskService Tasks { get { return AuthenticationTest.Default.Task; } }
        static ITimeEntryService Time { get { return AuthenticationTest.Default.TimeEntry; } }

        private ClientId _sampleClient;
        private ClientId ClientId { get { return _sampleClient.HasValue ? _sampleClient : (_sampleClient = ClientTest.SampleClient().ClientId); } }

        [TestMethod]
        public void CreateProjectTest()
        {
            TaskId sampleTaskId = Tasks.Create(new TaskRequest { Task = new Task { Name = "Something to do." } }).TaskId;

            string name = Guid.NewGuid().ToString("N");
            Project rec = new Project
                              {
                                  Name = name,
                                  ClientId = ClientId,
                                  BillMethod = "task-rate",
                                  Tasks = new ProjectTasks { TaskList = { new ProjectTask { TaskId = sampleTaskId, Rate = 5.25 } } },
                                  Staff = new ProjectStaffList { StaffList = { new ProjectStaff { StaffId = StaffTest.Self.StaffId } } },
                              };

            ProjectIdentity id = Service.Create(new ProjectRequest { Project = rec });
            Service.Update(new ProjectRequest { Project = new Project(rec) { ProjectId = id.ProjectId } });

            try
            {
                Project fetched = Service.Get(id).Project;
                Assert.AreEqual(name, fetched.Name);
                Assert.AreEqual(1, fetched.Tasks.TaskList.Count);
                Assert.AreEqual(1, fetched.Staff.StaffList.Count);
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void UpdateProjectTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Project rec = new Project { Name = name, BillMethod = "flat-rate" };

            ProjectIdentity id = Service.Create(new ProjectRequest { Project = rec });

            try
            {
                rec = Service.Get(id).Project;
                Assert.AreEqual(name, rec.Name);

                Service.Update(
                    new ProjectRequest
                        {
                            Project =
                                new Project(rec)
                                    {
                                        Description = "A widget for the whatsit",
                                    }
                        });

                Project fetched = Service.Get(id).Project;
                Assert.AreEqual(name, fetched.Name);
                Assert.AreEqual("A widget for the whatsit", fetched.Description);
            }
            finally
            {
                Service.Delete(id);
            }
        }
      
        [TestMethod]
        public void ListProjectsTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Project rec = new Project { Name = name, BillMethod = "flat-rate" };

            ProjectIdentity id = Service.Create(new ProjectRequest { Project = rec });

            try
            {
                IList<Project> items =
                    Service.List(new ProjectsRequest()).Projects.ProjectList;
                Assert.AreNotEqual(0, items.Count);

                Assert.AreEqual(1, items.Count(x => x.ProjectId == id.ProjectId));
            }
            finally
            {
                Service.Delete(id);
            }
        }

        [TestMethod]
        public void DeleteProjectTest()
        {
            string name = Guid.NewGuid().ToString("N");
            Service.Create(new ProjectRequest { Project = new Project { Name = name, BillMethod = "flat-rate" } });

            IList<Project> items = Service.List(new ProjectsRequest()).Projects.ProjectList;
            Assert.AreNotEqual(0, items.Count);

            foreach (Project rec in items)
                Service.Delete(new ProjectIdentity {ProjectId = rec.ProjectId});

            items = Service.List(new ProjectsRequest()).Projects.ProjectList;
            Assert.AreEqual(0, items.Count);
        }

        [TestMethod]
        public void ProjectTimeEntry()
        {
            StaffId me = StaffTest.Self.StaffId;
            TaskId sampleTaskId = Tasks.Create(new TaskRequest { Task = new Task { Name = "Something to do." } }).TaskId;

            string name = Guid.NewGuid().ToString("N");
            Project rec = new Project
            {
                Name = name,
                ClientId = ClientId,
                BillMethod = "task-rate",
                Tasks = new ProjectTasks { TaskList = { new ProjectTask { TaskId = sampleTaskId, Rate = 5.25 } } },
                Staff = new ProjectStaffList { StaffList = { new ProjectStaff { StaffId = me } } },
            };

            ProjectId projectId = Service.Create(new ProjectRequest { Project = rec }).ProjectId;

            try
            {
                TimeEntryIdentity teid = Time.Create(
                    new TimeEntryRequest()
                        {
                            TimeEntry =
                                new TimeEntry()
                                    {
                                        ProjectId = projectId,
                                        StaffId = me,
                                        TaskId = sampleTaskId,
                                        Hours = TimeSpan.FromHours(3.5),
                                        Date = DateTime.Now,
                                        Notes = "All-nighter",
                                    }
                        });

                Time.Update(
                    new TimeEntryRequest()
                        {
                            TimeEntry =
                                new TimeEntry(Time.Get(teid).TimeEntry)
                                    {
                                        Notes = "foo",
                                        Hours = TimeSpan.FromDays(2),
                                    }
                        });

                TimeEntry te = Time.Get(teid).TimeEntry;
                Assert.AreEqual(TimeSpan.FromDays(2), te.Hours);
                Assert.AreEqual("foo", te.Notes);

                IList<TimeEntry> entries = Time.List(new TimeEntriesRequest {ProjectId = projectId}).TimeEntries.TimeEntryList;
                Assert.AreEqual(1, entries.Count);
                Assert.AreEqual(sampleTaskId.Value, entries[0].TaskId.Value);
                Assert.AreEqual(projectId.Value, entries[0].ProjectId.Value);

                Time.Delete(teid);

                entries = Time.List(new TimeEntriesRequest { ProjectId = projectId }).TimeEntries.TimeEntryList;
                Assert.AreEqual(0, entries.Count);
            }
            finally
            {
                Service.Delete(new ProjectIdentity { ProjectId = projectId });
            }
        }
    }
}
