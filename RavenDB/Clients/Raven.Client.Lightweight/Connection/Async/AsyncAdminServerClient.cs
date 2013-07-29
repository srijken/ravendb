// -----------------------------------------------------------------------
//  <copyright file="AdminAsyncServerClient.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Imports.Newtonsoft.Json;
using Raven.Json.Linq;

namespace Raven.Client.Connection.Async
{
	public class AsyncAdminServerClient : IAsyncAdminDatabaseCommands, IAsyncGlobalAdminDatabaseCommands
	{
		internal readonly AsyncServerClient innerAsyncServerClient;
		private readonly AdminRequestCreator adminRequest;

		public AsyncAdminServerClient(AsyncServerClient asyncServerClient)
		{
			innerAsyncServerClient = asyncServerClient;
			adminRequest =
				new AdminRequestCreator((url, method) => innerAsyncServerClient.ForSystemDatabase().CreateRequest(url, method),
				                        (url, method) => innerAsyncServerClient.CreateRequest(url, method),
										(currentServerUrl, requestUrl, method) => innerAsyncServerClient.CreateReplicationAwareRequest(currentServerUrl, requestUrl, method));
		}

		public Task CreateDatabaseAsync(DatabaseDocument databaseDocument)
		{
			RavenJObject doc;
			var req = adminRequest.CreateDatabase(databaseDocument, out doc);

			return req.ExecuteWriteAsync(doc.ToString(Formatting.Indented));
		}

		public Task DeleteDatabaseAsync(string databaseName, bool hardDelete = false)
		{
			return adminRequest.DeleteDatabase(databaseName, hardDelete).ExecuteRequestAsync();
		}

		public Task StopIndexingAsync()
		{
			return innerAsyncServerClient.ExecuteWithReplication("POST", operationUrl => adminRequest.StopIndexing(operationUrl).ExecuteRequestAsync());
		}

		public Task StartIndexingAsync()
		{
			return innerAsyncServerClient.ExecuteWithReplication("POST", operationUrl => adminRequest.StartIndexing(operationUrl).ExecuteRequestAsync());
		}

		public async Task<AdminStatistics> GetStatisticsAsync()
		{
			var json = (RavenJObject)await adminRequest.AdminStats().ReadResponseJsonAsync();

			return json.Deserialize<AdminStatistics>(innerAsyncServerClient.convention);
		}
	}
}