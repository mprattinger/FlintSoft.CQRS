using FlintSoft.CQRS.Decorators;
using FlintSoft.CQRS.Events;
using FlintSoft.CQRS.Handlers;
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
            Console.WriteLine($"Error decorating ICommandHandler: {ex.Message}");
        }

        try
        {
            builder?.Services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));
        }
        catch (DecorationException ex)
        {
            Console.WriteLine($"Error decorating ICommandHandler: {ex.Message}");
        }

        try
        {
            builder?.Services.Decorate(typeof(IQueryHandler<,>), typeof(ExceptionDecorator.QueryHandler<,>));
        }
        catch (DecorationException ex)
        {
            Console.WriteLine($"Error decorating IQueryHandler: {ex.Message}");
        }

        try
        {
            builder?.Services.Decorate(typeof(ICommandHandler<,>), typeof(ExceptionDecorator.CommandHandler<,>));
        }
        catch (DecorationException ex)
        {
            Console.WriteLine($"Error decorating ICommandHandler: {ex.Message}");
        }

        try
        {
            builder?.Services.Decorate(typeof(ICommandHandler<>), typeof(ExceptionDecorator.CommandBaseHandler<>));
        }
        catch (DecorationException ex)
        {
            Console.WriteLine($"Error decorating ICommandHandler: {ex.Message}");
        }

        builder?.Services.Scan(scan => scan.FromAssembliesOf(type)
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        builder?.Services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();

        return builder;
    }
}
