using System.Text;
using BibliotecaApi.Autorizacao;
using BibliotecaApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<BibliotecaStore>();

var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GerenciarAcervo", p => p.RequireRole("Bibliotecario", "Admin"));
    options.AddPolicy("RemoverLivro", p => p.RequireRole("Admin"));
    options.AddPolicy("VerRelatorios", p => p.RequireRole("Admin"));

    options.AddPolicy("PodeRenovarEmprestimo",
        p => p.Requirements.Add(new DonoDoEmprestimoRequirement()));

    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddSingleton<IAuthorizationHandler, DonoDoEmprestimoHandler>();

builder.Configuration.AddJsonFile("politicas.json", optional: false, reloadOnChange: true);
builder.Services.Configure<PoliticasOptions>(builder.Configuration.GetSection("Politicas"));
builder.Services.AddSingleton<MotorDePoliticas>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
