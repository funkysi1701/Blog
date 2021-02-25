using System.Threading.Tasks;

namespace WebBlog.Data
{
    public interface IMetric
    {
        Task SaveData(decimal value, int type);

        Metric LoadData(int type, int maxmin);
    }
}
