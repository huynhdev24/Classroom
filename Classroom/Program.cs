using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Classroom.Data;
using Classroom.Core;
using Classroom.Core.Repositories;
using Classroom.Repositories;
using Classroom.Application.Common;
using Classroom.Application.System.Users;
using Classroom.Application.Catalog.Classes;
using Classroom.Models.VNPAY;
using Classroom.Controllers.Hubs;
using Classroom.Application.Common.SignalR;
using Classroom.Application.Catalog.Rooms;
using Classroom.Application.Catalog.Notifications;
using Classroom.Application.Catalog.Comments;
using Microsoft.AspNetCore.Authentication;
using Classroom.Application.Catalog.Homeworks;
using Classroom.Application.Catalog.Submissions;
using Classroom.Application.Catalog.ExamSchedules;
using Classroom.Application.Catalog.Questions;
using Classroom.Application.Catalog.StudentExams;
using Classroom.Application.Catalog.Contacts;

var builder = WebApplication.CreateBuilder(args);
var mvcBuilder = builder.Services.AddRazorPages();
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(30));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
        {
            IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
            googleOptions.ClientId = googleAuthNSection["ClientId"];
            googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
            googleOptions.ClaimActions.MapJsonKey("image", "picture");
        })
    .AddFacebook(facebookOptions =>
    {
        IConfigurationSection facebookAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");
        facebookOptions.ClientId = facebookAuthNSection["AppId"];
        facebookOptions.ClientSecret = facebookAuthNSection["AppSecret"];
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
        IConfigurationSection microsoftAuthNSection = builder.Configuration.GetSection("Authentication:Microsoft");
        microsoftOptions.ClientId = microsoftAuthNSection["ClientId"];
        microsoftOptions.ClientSecret = microsoftAuthNSection["ClientSecret"];
    });

builder.Services.AddTransient<IStorageService, FileStorageService>();

builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Authorization

AddAuthorizationPolicies();

#endregion

AddTransient();

builder.Services.AddSingleton(builder.Configuration.GetSection("VNPayConfig").Get<VNPayConfig>());

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePages(appError =>
{
    appError.Run(async context =>
    {
        var respone = context.Response;
        var code = respone.StatusCode;
        var content = @$"<html>
        <head>
            <meta charset='utf-8'>
            <meta http-equiv='X-UA-Compatible' content='IE=edge'>
            <meta name='viewport' content='width=device-width, initial-scale=1, shrink-to-fit=no'>
            <meta name='description' content=''>
            <meta name='author' content=''>

            <title>404 - Classroom</title>
            <link rel='icon' href='/img/logo.png' type='image/x-icon'>
            <link href='/css/sb-admin-2.min.css' rel='stylesheet'>
        </head>
        <body id='page-top'>
        <div class='container-fluid'>

            <!-- 404 Error Text -->
            <div class='text-center'>
                <div class='error mx-auto' data-text='{code}'>{code}</div>
                <p class='lead text-gray-800 mb-5'>{(HttpStatusCode)code}</p>
                <p class='text-gray-500 mb-0'>Có vẻ như bạn đã tìm thấy một vấn đề...</p>
                <a href='/'>&larr; Quay về trang chủ</a>
            </div>
        </div>
    </body>
        </html>";
        await respone.WriteAsync(content);
    });
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub");

app.MapRazorPages();

app.Run();


void AddAuthorizationPolicies()
{
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(Constants.Policies.RequireAdmin, policy => policy.RequireRole(Constants.Roles.Administrator));
        options.AddPolicy(Constants.Policies.RequireManager, policy => policy.RequireRole(Constants.Roles.Manager));
    });
}

void AddTransient()
{
    builder.Services.AddTransient<IUserRepository, UserRepository>();
    builder.Services.AddTransient<IRoleRepository, RoleRepository>();
    builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
    builder.Services.AddTransient<IUserService, UserService>();
    builder.Services.AddTransient<IClassService, ClassService>();
    builder.Services.AddTransient<IFileValidator, FileValidator>();
    builder.Services.AddTransient<IRoomService, RoomService>();
    builder.Services.AddTransient<INotificationService, NotificationService>();
    builder.Services.AddTransient<ICommentService, CommentService>();
    builder.Services.AddTransient<IHomeworkService, HomeworkService>();
    builder.Services.AddTransient<ISubmissionService, SubmissionService>();
    builder.Services.AddTransient<IExamScheduleService, ExamScheduleService>();
    builder.Services.AddTransient<IQuestionService, QuestionService>();
    builder.Services.AddTransient<IStudentExamService, StudentExamService>();
    builder.Services.AddTransient<IContactService, ContactService>();
}