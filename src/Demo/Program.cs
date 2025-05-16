using Demo.Features.User.Queries;
using FlintSoft.CQRS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateApplicationBuilder(args);

host?.Services.Scan(scan => scan.FromAssembliesOf(typeof(Program))
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

host.AddFlintSoftCQRS();

var app = host.Build();

var sp = app.Services;
var gun = sp.GetRequiredService<IQueryHandler<GetUserName.Query, string>>();
Console.WriteLine(await gun.Handle(new GetUserName.Query(), new CancellationToken()));

await app.StartAsync();