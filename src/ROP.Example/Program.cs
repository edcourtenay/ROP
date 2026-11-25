using Microsoft.AspNetCore.Builder;

namespace ROP.Example;

public class Program
{
    public static void Main(string[] args)
    {
        using var app = CreateApplication(args);
        app.Run();
    }

    public static WebApplication CreateApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.ConfigureServices();

        return builder
            .Build()
            .Configure();
    }
}