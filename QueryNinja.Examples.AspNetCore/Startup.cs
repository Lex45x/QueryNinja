using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using QueryNinja.Examples.AspNetCore.DbContext;
using QueryNinja.Examples.AspNetCore.DbContext.Entities;
using QueryNinja.Examples.AspNetCore.Extensions;
using QueryNinja.Extensions.AspNetCore.Swagger;
using QueryNinja.Sources.AspNetCore;
using QueryNinja.Sources.GraphQL;
using QueryNinja.Targets.EntityFrameworkCore;

namespace QueryNinja.Examples.AspNetCore
{
    internal class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.WithQueryNinja();
                options.IncludeXmlComments("./QueryNinja.Examples.AspNetCore.xml");
            });

            services.AddCors(options => options.AddDefaultPolicy(builder =>
                builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .DisallowCredentials()));

            services
                .AddQueryNinjaGraphQL()
                .WithEntityFrameworkTarget()
                .AddFilter<GradeFilter, GradeOperations>(configure =>
                {
                    configure.Define<ICollection<Grade>, Mark>(GradeOperations.NoLowerThan,
                        (grades, mark) => grades.Min(grade => grade.Mark) >= mark);
                });

            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddDbContext<UniversityDbContext>(options => options.UseSqlite("Data Source=University.db"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            app.UseSwagger();

            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "QueryNinja.Example"); });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                endpoints.MapControllers();
            });
        }
    }
}