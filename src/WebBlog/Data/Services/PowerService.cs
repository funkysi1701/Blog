using ImpSoft.OctopusEnergy.Api;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlog.Data.Services
{
    public class PowerService
    {
        private readonly MetricService _service;
        private IConfiguration Configuration { get; set; }

        private readonly string Key;
        private DateTimeOffset From;
        private DateTimeOffset To;
        private readonly IOctopusEnergyClient Client;

        public PowerService(IConfiguration configuration, MetricService MetricService, IOctopusEnergyClient client)
        {
            Configuration = configuration;
            _service = MetricService;
            Client = client;
            Key = Configuration.GetValue<string>("OctopusKey");
        }

        public async Task GetGas()
        {
            From = new DateTimeOffset(DateTime.Now.AddDays(-1 * Configuration.GetValue<int>("GasDayOffset")).AddHours(-1).AddMinutes(-1 * DateTime.Now.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            To = new DateTimeOffset(DateTime.Now.AddDays(-1 * Configuration.GetValue<int>("GasDayOffset")).AddMinutes(-1 * DateTime.Now.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            var consumption = await Client.GetGasConsumptionAsync(Key, Configuration.GetValue<string>("OctopusGasMPAN"), Configuration.GetValue<string>("OctopusGasSerial"), From, To, Interval.Hour);
            var value = consumption.ToList().Sum(x => x.Quantity);
            await _service.SaveData(value, 14, To.UtcDateTime);
        }

        public async Task GetElec()
        {
            From = new DateTimeOffset(DateTime.Now.AddDays(-1 * Configuration.GetValue<int>("GasDayOffset")).AddHours(-1).AddMinutes(-1 * DateTime.Now.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            To = new DateTimeOffset(DateTime.Now.AddDays(-1 * Configuration.GetValue<int>("GasDayOffset")).AddMinutes(-1 * DateTime.Now.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            var consumption = await Client.GetElectricityConsumptionAsync(Key, Configuration.GetValue<string>("OctopusElecMPAN"), Configuration.GetValue<string>("OctopusElecSerial"), From, To, Interval.Hour);
            var value = consumption.ToList().Sum(x => x.Quantity);
            await _service.SaveData(value, 15, To.UtcDateTime);
        }
    }
}
