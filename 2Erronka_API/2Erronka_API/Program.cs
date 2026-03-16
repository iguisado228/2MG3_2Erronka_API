    using _2Erronka_API;
    using _2Erronka_API.Repositorioak;
using System.ComponentModel;
using System.Text.Json.Serialization;




    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    // CORS konfigurazioa gehitu => Web-etik errorea ez emateko
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy
                .AllowAnyOrigin()     //.WithOrigins("http://localhost:8000") Jakiteko zein IPtatik etorri daitekeen
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });


    builder.Services.AddControllers();

    
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSingleton(NHibernateHelper.SessionFactory);
    builder.Services.AddTransient<ErreserbaRepository>();
    builder.Services.AddTransient<ProduktuaRepository>();
    builder.Services.AddTransient<EskariaRepository>();
    builder.Services.AddTransient<MahaiaRepository>();
    builder.Services.AddTransient<LangileaRepository>();
    builder.Services.AddTransient<LanpostuaRepository>();
    builder.Services.AddTransient<HornitzaileaRepository>();
    builder.Services.AddTransient<MotaRepository>();
    builder.Services.AddScoped<OsagaiaRepository>();
    builder.Services.AddScoped<ProduktuaOsagaiaRepository>();






var app = builder.Build();


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();

    app.UseCors();

    app.UseAuthorization();

    app.UseMiddleware<NHibernateSessionMiddleware>();
    app.MapControllers();

    app.Run();


