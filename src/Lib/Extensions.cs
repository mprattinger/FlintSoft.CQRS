using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace FlintSoft.CQRS;

public static class Extensions
{
    public static IHostApplicationBuilder? AddFlintSoftCQRS(this IHostApplicationBuilder? builder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var handlers = assembly
           .DefinedTypes
           .Where(type => type is { IsAbstract: false, IsInterface: false })
           .Where(type => type.GetInterfaces()
               .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
           .Select(type => ServiceDescriptor.Transient(type.GetInterfaces()
               .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)), type))
           .ToList();

        handlers.ForEach(r =>
        {
            builder?.Services.Add(r);
        });

        return builder;
    }
}
