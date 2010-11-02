﻿extern alias database;
using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json.Linq;
using Raven.Bundles.CascadeDelete;
using Raven.Bundles.Tests.Versioning;
using Raven.Client.Document;
using Raven.Database;
using Raven.Server;
using Xunit;
using System.Linq;

namespace Raven.Bundles.Tests.CascadeDelete
{
    public class CascadeDelete : IDisposable
    {
        private readonly DocumentStore documentStore;
        private readonly string path;
        private readonly RavenDbServer ravenDbServer;

        public CascadeDelete()
        {
            path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Versioning.Versioning)).CodeBase);
            path = Path.Combine(path, "TestDb").Substring(6);
            database::Raven.Database.Extensions.IOExtensions.DeleteDirectory("Data");
            ravenDbServer = new RavenDbServer(
                new database::Raven.Database.RavenConfiguration
                {
                    Port = 58080,
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                    DataDirectory = path,
                    Catalog =
                    {
                        Catalogs =
                                {
                                    new AssemblyCatalog(typeof (CascadeDeleteTrigger).Assembly)
                                }
                    },
                });

            documentStore = new DocumentStore
            {
                Url = "http://localhost:58080"
            };
            documentStore.Initialize();
        }

        #region IDisposable Members

        public void Dispose()
        {
            documentStore.Dispose();
            ravenDbServer.Dispose();
            database::Raven.Database.Extensions.IOExtensions.DeleteDirectory(path);
        }

        #endregion

        [Fact]
        public void Can_cascade_delete_single_referenced_document()
        {
            var master = new CascadeTester {Name="Master"};
            var child = new CascadeTester {Name="Child"};
            
            using (var session = documentStore.OpenSession())
            {
                session.Store(master);
                session.Store(child);
                session.Advanced.GetMetadataFor(master)[MetadataKeys.DocumentsToCascadeDelete] = new JValue(new string[] {child.Id});
                session.SaveChanges();
            }

            // assert intial creation
            using (var session = documentStore.OpenSession())
            {
                // assert master created
                Assert.NotNull(session.Load<CascadeTester>(master.Id));
                // assert child created
                Assert.NotNull(session.Load<CascadeTester>(child.Id));
            }


            using (var session = documentStore.OpenSession())
            {
                session.Delete<CascadeTester>(master);
                session.SaveChanges();
                // assert master deleted
                Assert.Null(session.Load<CascadeTester>(master.Id));
                // assert child deleted
                Assert.Null(session.Load<CascadeTester>(child.Id));
            }

        }

        [Fact]
        public void Can_cascade_delete_multiple_referenced_documents()
        {
            var master = new CascadeTester {Name="Master"};
            var child1 = new CascadeTester {Name="Child 1"};
            var child2 = new CascadeTester {Name="Child 2"};
            
            using (var session = documentStore.OpenSession())
            {
                session.Store(master);
                session.Store(child1);
                session.Store(child2);
                session.Advanced.GetMetadataFor(master)[MetadataKeys.DocumentsToCascadeDelete] = new JValue(new string[] {child1.Id, child2.Id});
                session.SaveChanges();
            }

            // assert intial creation
            using (var session = documentStore.OpenSession())
            {
                // assert master created
                Assert.NotNull(session.Load<CascadeTester>(master.Id));
                // assert child 1 created
                Assert.NotNull(session.Load<CascadeTester>(child1.Id));
                // assert child 2 created
                Assert.NotNull(session.Load<CascadeTester>(child2.Id));
            }


            using (var session = documentStore.OpenSession())
            {
                session.Delete<CascadeTester>(master);
                session.SaveChanges();
                // assert master deleted
                Assert.Null(session.Load<CascadeTester>(master.Id));
                // assert child 1 deleted
                Assert.Null(session.Load<CascadeTester>(child1.Id));
                // assert child 2 deleted
                Assert.Null(session.Load<CascadeTester>(child1.Id));
            }

        }


        [Fact]
        public void Can_cascade_delete_single_referenced_attachment()
        {
            var master = new CascadeTester {Name="Master"};
            
            using (var session = documentStore.OpenSession())
            {
                session.Store(master);
                documentStore.DatabaseCommands.PutAttachment("Cascade-Delete-Me", null, new byte[] {1, 2, 3}, null);
                session.Advanced.GetMetadataFor(master)[MetadataKeys.AttachmentsToCascadeDelete] = new JValue(new string[] {"Cascade-Delete-Me"});
                session.SaveChanges();
            }



            // assert intial creation
            using (var session = documentStore.OpenSession())
            {
                // assert master created
                Assert.NotNull(session.Load<CascadeTester>(master.Id));
                // assert attachment created
                Assert.NotNull(documentStore.DatabaseCommands.GetAttachment("Cascade-Delete-Me"));
            }


            using (var session = documentStore.OpenSession())
            {
                session.Delete<CascadeTester>(master);
                session.SaveChanges();
                // assert master deleted
                Assert.Null(session.Load<CascadeTester>(master.Id));
                // assert attachment deleted
                Assert.Null(documentStore.DatabaseCommands.GetAttachment("Cascade-Delete-Me"));
            }

        }

        [Fact]
        public void Can_cascade_delete_multiple_referenced_attachments()
        {
            var master = new CascadeTester {Name="Master"};
            
            using (var session = documentStore.OpenSession())
            {
                session.Store(master);
                documentStore.DatabaseCommands.PutAttachment("Cascade-Delete-Me-1", null, new byte[] {1, 2, 3}, null);
                documentStore.DatabaseCommands.PutAttachment("Cascade-Delete-Me-2", null, new byte[] {1, 2, 3}, null);
                session.Advanced.GetMetadataFor(master)[MetadataKeys.AttachmentsToCascadeDelete] = new JValue(new string[] {"Cascade-Delete-Me-1","Cascade-Delete-Me-2"});
                session.SaveChanges();
            }



            // assert intial creation
            using (var session = documentStore.OpenSession())
            {
                // assert master created
                Assert.NotNull(session.Load<CascadeTester>(master.Id));
                // assert attachment 1 created
                Assert.NotNull(documentStore.DatabaseCommands.GetAttachment("Cascade-Delete-Me-1"));
                // assert attachment 2 created
                Assert.NotNull(documentStore.DatabaseCommands.GetAttachment("Cascade-Delete-Me-2"));

            }


            using (var session = documentStore.OpenSession())
            {
                session.Delete<CascadeTester>(master);
                session.SaveChanges();
                // assert master deleted
                Assert.Null(session.Load<CascadeTester>(master.Id));
                // assert attachment 1 deleted
                Assert.Null(documentStore.DatabaseCommands.GetAttachment("Cascade-Delete-Me-1"));
                // assert attachment 2 deleted
                Assert.Null(documentStore.DatabaseCommands.GetAttachment("Cascade-Delete-Me-2"));

            }

        }

       [Fact]
        public void Can_cascade_delete_multiple_referenced_documents_and_attachments()
        {
            var master = new CascadeTester {Name="Master"};
            var child1 = new CascadeTester {Name="Child 1"};
            var child2 = new CascadeTester {Name="Child 2"}; 

            using (var session = documentStore.OpenSession())
            {
                session.Store(master);
                session.Store(child1);
                session.Store(child2);
                session.Advanced.GetMetadataFor(master)[MetadataKeys.DocumentsToCascadeDelete] = new JValue(new string[] {child1.Id, child2.Id});
                documentStore.DatabaseCommands.PutAttachment("Cascade-Delete-Me-1", null, new byte[] {1, 2, 3}, null);
                documentStore.DatabaseCommands.PutAttachment("Cascade-Delete-Me-2", null, new byte[] {1, 2, 3}, null);
                session.Advanced.GetMetadataFor(master)[MetadataKeys.AttachmentsToCascadeDelete] = new JValue(new string[] {"Cascade-Delete-Me-1","Cascade-Delete-Me-2"});
                session.SaveChanges();
            }



            // assert intial creation
            using (var session = documentStore.OpenSession())
            {
                // assert master created
                Assert.NotNull(session.Load<CascadeTester>(master.Id));
                // assert child 1 created
                Assert.NotNull(session.Load<CascadeTester>(child1.Id));
                // assert child 2 created
                Assert.NotNull(session.Load<CascadeTester>(child2.Id)); 
                // assert attachment 1 created
                Assert.NotNull(documentStore.DatabaseCommands.GetAttachment("Cascade-Delete-Me-1"));
                // assert attachment 2 created
                Assert.NotNull(documentStore.DatabaseCommands.GetAttachment("Cascade-Delete-Me-2"));

            }


            using (var session = documentStore.OpenSession())
            {
                session.Delete<CascadeTester>(master);
                session.SaveChanges();
                // assert master deleted
                Assert.Null(session.Load<CascadeTester>(master.Id));
                // assert child 1 deleted
                Assert.Null(session.Load<CascadeTester>(child1.Id));
                // assert child 2 deleted
                Assert.Null(session.Load<CascadeTester>(child2.Id)); 
                // assert attachment 1 deleted
                Assert.Null(documentStore.DatabaseCommands.GetAttachment("Cascade-Delete-Me-1"));
                // assert attachment 2 deleted
                Assert.Null(documentStore.DatabaseCommands.GetAttachment("Cascade-Delete-Me-2"));

            }

        }

#region nested test document classes

        public class CascadeTester
        {
            public string Name { get; set; }

            public string Id { get; set; }
        }

    }

#endregion

}
