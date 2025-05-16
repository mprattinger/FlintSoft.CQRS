using Demo.Features.User.Commands;
using Demo.Features.User.Queries;
using FlintSoft.CQRS;
using Lib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateApplicationBuilder(args);

host.AddFlintSoftCQRS(typeof(Program));

var app = host.Build();

var sp = app.Services;
var gun = sp.GetRequiredService<IQueryHandler<GetUserName.Query, string>>();
var cru = sp.GetRequiredService<ICommandHandler<CreateUser.Command, Guid>>();

var result = await gun.Handle(new GetUserName.Query(), new CancellationToken());
Console.WriteLine(result.IsError ? result.Errors.First().Description : result.Value);

var createUserResult = await cru.Handle(new CreateUser.Command("John Doe"), new CancellationToken());
Console.WriteLine(createUserResult.IsError ? createUserResult.Errors.First().Description : createUserResult.Value);

await app.StartAsync();