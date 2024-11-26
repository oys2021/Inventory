using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<Databasehelper>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var configuration = provider.GetRequiredService<IConfiguration>();  
    return new Databasehelper(connectionString, configuration);  
});

builder.Services.AddSingleton<IConfiguration>(builder.Configuration); 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(
    options=>{
        options.Cookie.HttpOnly=true;
        options.Cookie.IsEssential=true;
        options.IdleTimeout = TimeSpan.FromMinutes(30);
    }
);
var app = builder.Build();

app.UseRouting();
app.UseStaticFiles();
app.UseSession();

app.MapControllerRoute(
    name:"default",
    pattern:"{controller=user}/{action=userlogin}"
);

app.MapControllerRoute(
    name:"welcome",
    pattern:"{controller=user}/{action=welcome}"
);

app.MapControllerRoute(
name:"admin_home",
pattern:"{controller=admin}/{action=home}"
);

app.MapControllerRoute(
name:"admin_home",
pattern:"{controller=admin}/{action=adduser}"
);
app.MapControllerRoute(
name:"admin_home",
pattern:"{controller=admin}/{action=addrole}"
);

app.MapControllerRoute(
name:"admin_home",
pattern:"{controller=user}/{action=changepassword}"
);


app.Run();
