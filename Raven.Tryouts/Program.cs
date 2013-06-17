using System;
using System.Collections.Generic;
using Raven.Client;
using Raven.Client.Document;
using Raven.Database;
using Raven.Database.Config;
using Raven.Tests.Issues;

class Program
{
	static void Main(string[] args)
	{
		using(var store = new DocumentStore{Url = "http://localhost:8080", DefaultDatabase = "LoadTest"}.Initialize())
		using (var session = store.OpenSession())
		{
			var item = session.Load<Class>("classes/1");
			//AddItem(session);
			session.SaveChanges();
		}
	}

	private static void AddItem(IDocumentSession session)
	{
		var item = new Class {Grade = "A", Students = new List<Person>()};
	//	item.Students.Add(new Person { Name = "Daniel", Temp = "Dar" });
		session.Store(item);
	}

	public class Class
	{
		public string Grade { get; set; }
		public List<Person> Students { get; set; } 
	}

	public class Person
	{
		public string Name { get; set; }
	//	public string Temp { get; set; }
	}
}
