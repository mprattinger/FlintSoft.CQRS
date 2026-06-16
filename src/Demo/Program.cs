using Demo.Features.User;
using Demo.Features.User.Commands;
using Demo.Features.User.Queries;
using FlintSoft.CQRS;
using FlintSoft.CQRS.Events;
using FlintSoft.CQRS.Handlers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateApplicationBuilder(args);

host.AddFlintSoftCQRS(typeof(Program));

host.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = host.Build();

var sp = app.Services;
var gun = sp.GetRequiredService<IQueryHandler<GetUserName.Query, string>>();
var cru = sp.GetRequiredService<ICommandHandler<CreateUser.Command, User>>();
var dsp = sp.GetRequiredService<IDomainEventDispatcher>();

var result = await gun.Handle(new GetUserName.Query(), new CancellationToken());
Console.WriteLine(result.IsError ? result.Errors.First().Description : result.Value);

var createUserResult1 = await cru.Handle(new CreateUser.Command("error"), new CancellationToken());
Console.WriteLine(createUserResult1.IsError ? createUserResult1.Errors.First().Description : createUserResult1.Value);

var createUserResult2 = await cru.Handle(new CreateUser.Command("wrong"), new CancellationToken());
Console.WriteLine(createUserResult2.IsError ? createUserResult2.Errors.First().Description : createUserResult2.Value);

var createUserResult3 = await cru.Handle(new CreateUser.Command(""), new CancellationToken());
Console.WriteLine(createUserResult3.IsError ? createUserResult3.Errors.First().Description : createUserResult3.Value);

var createUserResult4 = await cru.Handle(new CreateUser.Command("Michael"), new CancellationToken());
Console.WriteLine(createUserResult4.IsError ? createUserResult4.Errors.First().Description : createUserResult4.Value);

await app.StartAsync();

Console.ReadLine();
