using Pulumi;
using System.Threading.Tasks;

class Program
{
    static Task<int> Main() => Deployment.RunAsync<MyStack>();
}