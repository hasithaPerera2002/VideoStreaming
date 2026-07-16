using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.S3;
using server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://0.0.0.0:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var awsEndpoint = Environment.GetEnvironmentVariable("AWS_ENDPOINT_URL") ?? "http://localhost:4566";
var awsCredentials = new BasicAWSCredentials("test", "test");


builder.Services.AddSingleton<IAmazonS3>(sp =>
    new AmazonS3Client(awsCredentials, new AmazonS3Config
    {
        ServiceURL = awsEndpoint,
        ForcePathStyle = true
    }));

builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
    new AmazonDynamoDBClient(awsCredentials, new AmazonDynamoDBConfig
    {
        ServiceURL = awsEndpoint
    }));

builder.Services.AddScoped<IVideoStorageService, S3Service>();
builder.Services.AddScoped<IVideoMetadataService, DynamodbService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("CorsPolicy");
app.MapControllers();  // this makes [ApiController]-based controllers actually get routed

app.Run();
