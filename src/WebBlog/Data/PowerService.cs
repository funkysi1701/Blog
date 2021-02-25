﻿using ImpSoft.OctopusEnergy.Api;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlog.Data
{
    public class PowerService
    {
        private readonly MetricService _service;
        private IConfiguration Configuration { get; set; }

        private readonly string Key;
        private readonly DateTimeOffset From;
        private readonly DateTimeOffset To;
        private readonly IOctopusEnergyClient Client;

        public PowerService(IConfiguration configuration, MetricService MetricService, IOctopusEnergyClient client)
        {
            Configuration = configuration;
            _service = MetricService;
            Client = client;
            Key = Configuration.GetValue<string>("OctopusKey");
            From = new DateTimeOffset(DateTime.Now.AddDays(-2).AddHours(-1), TimeSpan.FromHours(0));
            To = new DateTimeOffset(DateTime.Now.AddDays(-2), TimeSpan.FromHours(0));
        }

        public async Task GetGas()
        {
            var consumption = await Client.GetGasConsumptionAsync(Key, Configuration.GetValue<string>("OctopusGasMPAN"), Configuration.GetValue<string>("OctopusGasSerial"), From, To, Interval.Hour);
            var value = consumption.ToList().Sum(x => x.Quantity);
            await _service.SaveData(value, 14, To.UtcDateTime);
        }

        public async Task GetElec()
        {
            var consumption = await Client.GetElectricityConsumptionAsync(Key, Configuration.GetValue<string>("OctopusElecMPAN"), Configuration.GetValue<string>("OctopusElecSerial"), From, To, Interval.Hour);
            var value = consumption.ToList().Sum(x => x.Quantity);
            await _service.SaveData(value, 15, To.UtcDateTime);
        }
    }
}
