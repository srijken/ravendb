#if !SILVERLIGHT && !NETFX_CORE
// -----------------------------------------------------------------------
//  <copyright file="AdminDatabaseCommands.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven.Abstractions.Data;
using Raven.Imports.Newtonsoft.Json;
using Raven.Json.Linq;

namespace Raven.Client.Connection
{
	public class AdminServerClient : IAdminDatabaseCommands, IGlobalAdminDatabaseCommands
	{
		internal readonly ServerClient innerServerClient;
		private readonly AdminRequestCreator adminRequest;

		public AdminServerClient(ServerClient serverClient)
		{
			innerServerClient = serverClient;
			adminRequest =
				new AdminRequestCreator(
					(url, method) => ((ServerClient) innerServerClient.ForSystemDatabase()).CreateRequest(url, method),
					(url, method) => innerServerClient.CreateRequest(url, method),
					(currentServerUrl, requestUrl, method) => innerServerClient.CreateReplicationAwareRequest(currentServerUrl, requestUrl, method));
		}

		public void CreateDatabase(DatabaseDocument databaseDocument)
		{
			RavenJObject doc;
			var req = adminRequest.CreateDatabase(databaseDocument, out doc);

			req.Write(doc.ToString(Formatting.Indented));
			req.ExecuteRequest();
		}

		public void DeleteDatabase(string databaseName, bool hardDelete = false)
		{
			adminRequest.DeleteDatabase(databaseName, hardDelete).ExecuteRequest();
		}

		public void StopIndexing()
		{
			innerServerClient.ExecuteWithReplication("POST", operationUrl => adminRequest.StopIndexing(operationUrl).ExecuteRequest());
		}

		public void StartIndexing()
		{
			innerServerClient.ExecuteWithReplication("POST", operationUrl => adminRequest.StartIndexing(operationUrl).ExecuteRequest());
		}

		public AdminStatistics GetStatistics()
		{
			var json = (RavenJObject) adminRequest.AdminStats().ReadResponseJson();

			return json.Deserialize<AdminStatistics>(innerServerClient.convention);
		}
	}
}
#endif