using Demo.Features.User;
using Demo.Features.User.Commands;
using Demo.Features.User.Queries;
using FlintSoft.CQRS;
using FlintSoft.CQRS.Events;
using FlintSoft.CQRS.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateApplicationBuilder(args);

host.AddFlintSoftCQRS(typeof(Program));

var app = host.Build();

var sp = app.Services;
var gun = sp.GetRequiredService<IQueryHandler<GetUserName.Query, string>>();
var cru = sp.GetRequiredService<ICommandHandler<CreateUser.Command, User>>();
var dsp = sp.GetRequiredService<IDomainEventDispatcher>();

var result = await gun.Handle(new GetUserName.Query(), new CancellationToken());
Console.WriteLine(result.IsError ? result.Errors.First().Description : result.Value);

var createUserResult = await cru.Handle(new CreateUser.Command("uname", "geheim", "u@u.com", "John", "Doe"), new CancellationToken());
Console.WriteLine(createUserResult.IsError ? createUserResult.Errors.First().Description : createUserResult.Value);

await dsp.DispatchAsync(createUserResult.Value.DomainEvents);
createUserResult.Value.ClearDomainEvents();

await app.StartAsync();