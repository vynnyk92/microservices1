﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using Nancy.Configuration;
using Nancy.Owin;

namespace ShoppingCart
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseOwin().UseNancy(opt => opt.Bootstrapper = new TracingBootstrapper());
        }


        public class TracingBootstrapper : Nancy.DefaultNancyBootstrapper
        {
            public override void Configure(INancyEnvironment env)
            {
                env.Tracing(enabled: true, displayErrorTraces: true);
            }
        }
    }
}
