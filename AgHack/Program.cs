using AgHack.Models;
using AgHack.Services;
using AgHack.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<AgHackContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

// 註冊 Repository 和 Service
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IWaterQualityService, WaterQualityService>();
builder.Services.AddScoped<IIrrigationWaterService, IrrigationWaterService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// �K�[ API ����䴩
builder.Services.AddControllers();

// �K�[ Swagger �A��
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AgHack API",
        Version = "v1",
        Description = "�A�~����ʴ��t�� API ���",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "AgHack Team",
            Email = "contact@aghack.com"
        }
    });

    // �����P�� API ����
    c.TagActionsBy(api => new[] 
    {
        api.ActionDescriptor.RouteValues["controller"] switch
        {
            "WaterQualityApi" => "����ʴ� API",
            "GroundwaterApi" => "�a�U���ʴ� API", 
            "IrrigationWaterApi" => "灌溉水質監測 API",
            "ReferenceApi" => "�ѦҸ�� API",
            "SearchApi" => "�j�M�d�� API",
            _ => api.ActionDescriptor.RouteValues["controller"] ?? "������"
        }
    });

    c.DocInclusionPredicate((name, api) => true);

    // �ҥ� XML ���Ѥ䴩 (�p�G�ݭn)
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // c.IncludeXmlComments(xmlPath);
});

// �K�[ CORS �䴩 (�p�G�ݭn�e�ݩI�s API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // �u�b�}�o���ұҥ� Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgHack API v1");
        c.RoutePrefix = "swagger"; // �]�w Swagger UI �����|�� /swagger
        c.DocumentTitle = "AgHack API ���";
        c.DefaultModelsExpandDepth(-1); // ���� Models �϶�
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // �w�]�i�}�覡
    });
}

app.UseHttpsRedirection();

// 使用全域異常處理中間件
app.UseGlobalExceptionHandler();

app.UseRouting();

// �ϥ� CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// �K�[ API ����
app.MapControllers();

// ���ո�Ʈw�s�u
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AgHackContext>();
//    try
//    {
//        db.Database.CanConnect();
//        Console.WriteLine("��Ʈw�s�u���\�I");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"��Ʈw�s�u����: {ex.Message}");
//    }
//}

app.Run();
