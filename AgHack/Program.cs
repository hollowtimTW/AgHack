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

// 加入 API 控制器註冊
builder.Services.AddControllers();

// 加入 Swagger 支援
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AgHack API",
        Version = "v1",
        Description = "農業資料平台 API 文件",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "AgHack Team",
            Email = "contact@aghack.com"
        }
    });

    // 設定分組 API 標籤
    c.TagActionsBy(api => new[]
    {
        api.ActionDescriptor.RouteValues["controller"] switch
        {
            "WaterQualityApi" => "水質監測 API",
            "GroundwaterApi" => "地下水監測 API",
            "IrrigationWaterApi" => "灌溉水質監測 API",
            "ReferenceApi" => "參考資料 API",
            "SearchApi" => "搜尋查詢 API",
            _ => api.ActionDescriptor.RouteValues["controller"] ?? "未分類"
        }
    });

    c.DocInclusionPredicate((name, api) => true);

    // 加入 XML 文件註解（如有需要）
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // c.IncludeXmlComments(xmlPath);
});

// 加入 CORS 設定（如需跨域存取 API）
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
    // 開發環境啟用 Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgHack API v1");
        c.RoutePrefix = "swagger"; // 設定 Swagger UI 路徑為 /swagger
        c.DocumentTitle = "AgHack API 文件";
        c.DefaultModelsExpandDepth(-1); // 隱藏 Models 區塊
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // 預設展開方式
    });
}

app.UseHttpsRedirection();

// 使用全域異常處理中間件
app.UseGlobalExceptionHandler();

app.UseRouting();

// 啟用 CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// 加入 API 路由
app.MapControllers();

// 資料庫連線測試（如有需要）
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AgHackContext>();
//    try
//    {
//        db.Database.CanConnect();
//        Console.WriteLine("資料庫連線成功！");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"資料庫連線失敗: {ex.Message}");
//    }
//}

app.Run();
