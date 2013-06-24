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


		//var json = "{'Grade': 'a', 'Val': 2, 'Students': [{'Name': '1', 'Use': true }]}";

		//var defaultRavenContractResolver = new DefaultRavenContractResolver(true);
		//var jsonSerializer = new JsonSerializer
		//{
		//	Formatting = Formatting.Indented,
		//	ContractResolver = defaultRavenContractResolver
		//};

		//var dic = new Dictionary<object, Dictionary<string, JToken>>(ObjectReferenceEqualityComparer<object>.Default);

		//jsonSerializer.BeforeClosingObject += (o, writer) =>
		//{
		//	Dictionary<string, JToken> value;
		//	if (dic.TryGetValue(o, out value) == false)
		//		return;

		//	foreach (var item in value)
		//	{
		//		writer.WritePropertyName(item.Key);
		//		if (item.Value == null)
		//			writer.WriteNull();
		//		else
		//			item.Value.WriteTo(writer);
		//	}
		//};

		//using (defaultRavenContractResolver.RegisterForExtensionData((o, key, value) =>
		//{
		//	Dictionary<string, JToken> dictionary;
		//	if (dic.TryGetValue(o, out dictionary) == false)
		//	{
		//		dic[o] = dictionary = new Dictionary<string, JToken>();
		//	}
		//	dictionary[key] = value;
		//	Console.WriteLine(key + " " + value);
		//}))
		//{
		//	var x = jsonSerializer.Deserialize<Class>(new JsonTextReader(new StringReader(json)));
		//	x.Students.Insert(0, new Person { Name = "123" });

		//	Console.WriteLine(dic.ContainsKey(x));

		//	jsonSerializer.Serialize(Console.Out, x);
		//}
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
