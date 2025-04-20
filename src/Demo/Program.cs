using Demo.Features.User.Queries;
using FlintSoft.CQRS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateApplicationBuilder(args);


host.AddFlintSoftCQRS();

var app = host.Build();

var sp = app.Services;
var gun = sp.GetRequiredService<IRequestHandler<GetUserName.Query, string>>();
Console.WriteLine(gun.Handle(new GetUserName.Query()));

await app.StartAsync();