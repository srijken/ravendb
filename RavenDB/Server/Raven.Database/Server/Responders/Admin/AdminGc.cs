using System;
using System.Linq.Expressions;
using Raven.Database.Server.Abstractions;

namespace Raven.Database.Server.Responders.Admin
{
	using System.Runtime;

	public class AdminGc : AdminResponder
	{
		public override string[] SupportedVerbs
		{
			get { return new[] { "POST", "GET" }; }
		}

		// this is just the code below, but we have to run on 4.5, not just 4.5.1
		// GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
		private static readonly Lazy<Action> setCompactLog = new Lazy<Action>(() =>
			{
				var prop = typeof(GCSettings).GetProperty("LargeObjectHeapCompactionMode");
				if (prop == null)
					return (() => { }); 
				var enumType = Type.GetType("System.Runtime.GCLargeObjectHeapCompactionMode, mscorlib");
				var value = Enum.Parse(enumType, "CompactOnce");
				var lambda = Expression.Lambda<Action>(Expression.Assign(Expression.MakeMemberAccess(null, prop), Expression.Constant(value)));
				return lambda.Compile();
			});

		public override void RespondToAdmin(IHttpContext context)
		{
			if (EnsureSystemDatabase(context) == false)
				return;

			CollectGarbage(Database, compactLoh: false);
		}

		public static void CollectGarbage(DocumentDatabase database, bool compactLoh)
		{
			if (compactLoh)
			{
				setCompactLog.Value();
			}

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
			database.TransactionalStorage.ClearCaches();
			GC.WaitForPendingFinalizers();
		}
	}
}