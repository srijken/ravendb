using System.Collections.Generic;
using Raven.Json.Linq;
using Xunit;

namespace Raven.Tests.Issues
{
	public class RavenDB_1163 : RavenTest
	{
		[Fact]
		public void WillKeepMissingPropertiesFromClass()
		{
			var json = "{'Grade': 'a', 'Val': 2, 'Students': [{'Name': '1', 'Use': true }]}";

			using (var store = NewDocumentStore())
			{
				store.DatabaseCommands.Put("classes/1", null, RavenJObject.Parse(json), new RavenJObject());

				using(var session = store.OpenSession())
				{
					var item = session.Load<Class>("classes/1");
					session.SaveChanges();
				}

				var result = store.DatabaseCommands.Get("classes/1").DataAsJson;

				Assert.Equal(result["Val"], 2);
				Assert.Equal(((RavenJArray)result["Students"])[0].SelectToken("Use"), true);
			}
			
		}

		public class Class
		{
			public string Grade { get; set; }
			public List<Person> Students { get; set; }
		}

		public class Person
		{
			public string Name { get; set; }
		}
	}
}
