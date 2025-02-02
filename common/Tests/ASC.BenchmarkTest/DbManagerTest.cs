﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Web;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Jobs;

namespace ASC.BenchmarkTest
{
    [SimpleJob(RuntimeMoniker.Net48)]
    //[SimpleJob(RuntimeMoniker.Mono)]
    [NativeMemoryProfiler]
    [MemoryDiagnoser]
    public class DbManagerTest
    {
        public DbManagerTest()
        {
            DbRegistry.Configure();
        }

        [Benchmark]
        public void WithHttpContext()
        {
            Parallel.For(0, 1000, (i) =>
            {
                HttpContext.Current = new HttpContext(
                   new HttpRequest("hack", "http://localhost", string.Empty),
                   new HttpResponse(new StringWriter()));

                int[] result;

                using (var dbManager = DbManager.FromHttpContext("core"))
                {
                    var query = new SqlQuery("tenants_tenants t")
                     .Select("t.id", "t.alias", "t.mappeddomain", "t.version", "t.version_changed", "t.name", "t.language", "t.timezone", "t.owner_id")
                     .Select("t.trusteddomains", "t.trusteddomainsenabled", "t.creationdatetime", "t.status", "t.statuschanged", "t.payment_id", "t.last_modified")
                     .Select("t.industry", "t.spam", "t.calls");

                    result = dbManager.ExecuteList(query).Select(r => Convert.ToInt32(r[0])).ToArray();
                }

                using (var dbManager = DbManager.FromHttpContext("core"))
                {
                    var query = new SqlQuery("tenants_tariff t")
                     .Select("t.id");

                    result = dbManager.ExecuteList(query).Select(r => Convert.ToInt32(r[0])).ToArray();
                }

                DisposableHttpContext.Current.Dispose();
            });

        }

        [Benchmark]
        public void WithoutHttpContext()
        {
            Parallel.For(0, 1000, (i) =>
            {
                int[] result;

                using (var dbManager = DbManager.FromHttpContext("core"))
                {
                    //Console.WriteLine("test");
                    var query = new SqlQuery("tenants_tenants t")
                     .Select("t.id", "t.alias", "t.mappeddomain", "t.version", "t.version_changed", "t.name", "t.language", "t.timezone", "t.owner_id")
                     .Select("t.trusteddomains", "t.trusteddomainsenabled", "t.creationdatetime", "t.status", "t.statuschanged", "t.payment_id", "t.last_modified")
                     .Select("t.industry", "t.spam", "t.calls");

                    result = dbManager.ExecuteList(query).Select(r => Convert.ToInt32(r[0])).ToArray();
                }

                using (var dbManager = DbManager.FromHttpContext("core"))
                {
                    var query = new SqlQuery("tenants_tariff t")
                     .Select("t.id");

                    result = dbManager.ExecuteList(query).Select(r => Convert.ToInt32(r[0])).ToArray();
                }
            });

        }
    }
}
