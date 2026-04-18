using DrivingLessonApp.Components;
using DrivingLessonApp.Services;

var builder = WebApplication.CreateBuilder(args);

// this enables razor components with interactive server rendering
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// this sets the backend api base address for frontend requests
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5125/")
});

// this stores learner signup data across multiple pages
builder.Services.AddScoped<LearnerSignupState>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// this handles not found pages
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

// this redirects the frontend app to https
app.UseHttpsRedirection();

// this serves css js and image files
app.UseStaticFiles();

// this enables antiforgery protection
app.UseAntiforgery();

// this maps the razor component app
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();