using JuanMartin.PhotoGallery.Services;
using JuanMartin.PhotoGallery.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(5); //  default is 20 minutes
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews();
var implementationInstance = new PhotoService(builder.Configuration);
builder.Services.AddSingleton<IPhotoService>(implementationInstance);
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Configure the HTTP request pipeline.
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    //        app.UseExceptionHandler("/Gallery/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//    pattern: "{controller=Gallery}/{action=Index}/{id?}");
app.MapGet("/", (HttpContext context) => HttpUtility.InitializeSession(context.Session,builder.Configuration,context));
app.Run();
