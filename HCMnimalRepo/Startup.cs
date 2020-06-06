using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HCMnimalRepo.Model;
using HotChocolate;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.Execution;
using HotChocolate.Execution.Batching;
using HotChocolate.Execution.Configuration;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Types.Relay;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HCMnimalRepo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //services.AddGraphQL(SchemaBuilder.New());

            /*
             * This is an alternative approach (workaround) to fix this behavior
             */
            #region Workaround

            Func<IServiceProvider, ISchema> factory = s => SchemaBuilder.New()
                .AddServices(s)
                .AddType<AuthorType>()
                .AddType<BookType>()
                .AddQueryType<Query>()
                .Create();

            var opts = new QueryExecutionOptions { };

            services.AddSingleton(sp =>
            {
                var d = sp.GetRequiredService<DiagnosticListener>();
                var o = sp.GetServices<IDiagnosticObserver>();

                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
                CultureInfo culture = null;
                var instantiatedType = Activator.CreateInstance(typeof(QueryExecutionDiagnostics), flags, null, new object[] { d, o }, culture);
                return instantiatedType as QueryExecutionDiagnostics;
            });

            QueryExecutionBuilder.New()
                .AddDefaultServices(opts)
                .UseRequestTimeout()
                .UseExceptionHandling()


                .UseQueryParser()
                .UseNoCachedQueryError()
                .UseValidation()
                .UseOperationResolver()
                .UseMaxComplexity()
                .UseOperationExecutor()
                .Populate(services);

            services.AddSingleton(factory);
            services.AddSingleton<IIdSerializer, IdSerializer>()
                .AddGraphQLSubscriptions();
            services.AddJsonQueryResultSerializer().AddJsonArrayResponseStreamSerializer();

            services.AddSingleton<IBatchQueryExecutor, BatchQueryExecutor>();

            /* END */
            #endregion

            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
