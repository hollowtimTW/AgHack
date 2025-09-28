using AgHack.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<AgHackContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

// Add services to the container.
builder.Services.AddControllersWithViews();

// 添加 API 控制器支援
builder.Services.AddControllers();

// 添加 Swagger 服務
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AgHack API",
        Version = "v1",
        Description = "農業水質監測系統 API 文件",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "AgHack Team",
            Email = "contact@aghack.com"
        }
    });

    // 為不同的 API 分組
    c.TagActionsBy(api => new[] 
    {
        api.ActionDescriptor.RouteValues["controller"] switch
        {
            "WaterQualityApi" => "水質監測 API",
            "GroundwaterApi" => "地下水監測 API", 
            "IndustrialWastewaterApi" => "工業廢水監測 API",
            "ReferenceApi" => "參考資料 API",
            "SearchApi" => "搜尋查詢 API",
            _ => api.ActionDescriptor.RouteValues["controller"] ?? "未分類"
        }
    });

    c.DocInclusionPredicate((name, api) => true);

    // 啟用 XML 註解支援 (如果需要)
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // c.IncludeXmlComments(xmlPath);
});

// 添加 CORS 支援 (如果需要前端呼叫 API)
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
    // 只在開發環境啟用 Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgHack API v1");
        c.RoutePrefix = "swagger"; // 設定 Swagger UI 的路徑為 /swagger
        c.DocumentTitle = "AgHack API 文件";
        c.DefaultModelsExpandDepth(-1); // 隱藏 Models 區塊
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // 預設展開方式
    });
}

app.UseHttpsRedirection();
app.UseRouting();

// 使用 CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// 添加 API 路由
app.MapControllers();

// 測試資料庫連線
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
