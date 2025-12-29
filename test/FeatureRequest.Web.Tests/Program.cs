using Microsoft.AspNetCore.Builder;
using FeatureRequest;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();

builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("FeatureRequest.Web.csproj");
await builder.RunAbpModuleAsync<FeatureRequestWebTestModule>(applicationName: "FeatureRequest.Web" );

public partial class Program
{
}
