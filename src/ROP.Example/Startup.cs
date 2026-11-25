using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ROP.Example.Services;

namespace ROP.Example;

public static class Startup
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IValidationService, ValidationService>();

        services.AddMvc(options =>
        {
            options.EnableEndpointRouting = false;
        });

        services.AddOpenApi();
    }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public static WebApplication Configure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.MapOpenApi();
        }
        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //app.UseHttpsRedirection();
        app.UseMvc();

        return app;
    }
}