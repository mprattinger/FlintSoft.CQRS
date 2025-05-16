using Lib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scrutor;

namespace FlintSoft.CQRS;

public static class Extensions
{
    public static IHostApplicationBuilder? AddFlintSoftCQRS(this IHostApplicationBuilder? builder, Type type)
    {
        builder?.Services.Scan(scan => scan.FromAssembliesOf(type)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        try
        {
            builder?.Services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        }
        catch (DecorationException ex)
        {
            Console.WriteLine($"Error decorating IQueryHandler: {ex.Message}");
        }

        try
        {
            builder?.Services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        }
        catch (DecorationException ex)
        {
            Console.WriteLine($"Error decorating IQueryHandler: {ex.Message}");
        }

        try
        {
            builder?.Services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));
        }
        catch (DecorationException ex)
        {
            Console.WriteLine($"Error decorating IQueryHandler: {ex.Message}");
        }





        // var assembly = Assembly.GetExecutingAssembly();

        // var handlers = assembly
        //    .DefinedTypes
        //    .Where(type => type is { IsAbstract: false, IsInterface: false })
        //    .Where(type => type.GetInterfaces()
        //        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
        //    .Select(type => ServiceDescriptor.Transient(type.GetInterfaces()
        //        .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)), type))
        //    .ToList();

        // handlers.ForEach(r =>
        // {
        //     builder?.Services.Add(r);
        // });

        return builder;
    }
}
