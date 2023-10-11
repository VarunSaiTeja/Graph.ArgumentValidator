using Graph.ArgumentValidator;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddArgumentValidator()
    .AddQueryType<Query>();
builder.Services
    .AddSingleton<DuplicateEmailValidatorService>();
    
var app = builder.Build();

app.MapGraphQL();

app.Run();