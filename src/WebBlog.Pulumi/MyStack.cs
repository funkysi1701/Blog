using Pulumi;
using Pulumi.Azure.AppInsights;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.Core;

public class MyStack : Stack
{
    public MyStack()
    {
        var resourceGroup = new ResourceGroup("blog-pulumi", new ResourceGroupArgs
        {
            Name = "blog-pulumi",
        });

        var appServicePlan = GenerateAppServicePlan("Blog-Test", "UKSouth", resourceGroup, "D1");
        var appInsights = GenerateAppInsights("blog-test", resourceGroup);
        var app = GenerateAppService("FSiBlogTest", "UKSouth", appServicePlan, resourceGroup, appInsights);
    }

    public Plan GenerateAppServicePlan(string name, string Region, ResourceGroup resourceGroup, string size)
    {
        return new Plan(name, new PlanArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Name = name,
            Kind = "App",
            Location = Region,
            Sku = new PlanSkuArgs
            {
                Tier = "Shared",
                Size = size,
            },
        });
    }

    public AppService GenerateAppService(string name, string Region, Plan appServicePlan, ResourceGroup resourceGroup, Insights appInsights)
    {
        return new AppService(name, new AppServiceArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AppServicePlanId = appServicePlan.Id,
            Location = Region,
            Name = name,
            SiteConfig = new AppServiceSiteConfigArgs()
            {
                AlwaysOn = false,
                DotnetFrameworkVersion = "v5.0",
                Use32BitWorkerProcess = true
            }
        });
    }

    public Insights GenerateAppInsights(string name, ResourceGroup resourceGroup)
    {
        return new Insights(name, new InsightsArgs
        {
            Name = name,
            Location = "UKSouth",
            ResourceGroupName = resourceGroup.Name,
            ApplicationType = "web",
        });
    }
}
