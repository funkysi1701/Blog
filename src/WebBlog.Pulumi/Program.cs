using Pulumi;
using System.Threading.Tasks;

internal class Program
{
    private static Task<int> Main() => Deployment.RunAsync<MyStack>();
}
