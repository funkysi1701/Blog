using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBlog.Data.Context;

namespace WebBlog.Data.Services
{
    public class MetricService : IMetric
    {
        private readonly MetricsContext _context;

        public MetricService(MetricsContext context)
        {
            _context = context;
        }

        public async Task<List<Profile>> GetProfiles()
        {
            return await Task.FromResult(_context.Profiles.ToList());
        }

        public async Task SaveProfileData(string Value)
        {
            _context.Add(new Profile
            {
                UserName = Value,
                Id = DateTime.UtcNow.Ticks,
                PartitionKey = "1"
            });
            await _context.SaveChangesAsync();
        }

        public async Task SaveData(decimal value, int type, DateTime To)
        {
            _context.Add(new Metric
            {
                Id = DateTime.UtcNow.Ticks,
                Date = To,
                Type = type,
                Value = value,
                PartitionKey = "1"
            });

            await _context.SaveChangesAsync();
        }

        public async Task SaveData(decimal value, int type)
        {
            _context.Add(new Metric
            {
                Id = DateTime.UtcNow.Ticks,
                Date = DateTime.UtcNow,
                Type = type,
                Value = value,
                PartitionKey = "1"
            });

            await _context.SaveChangesAsync();
        }

        public async Task Delete()
        {
            var m = _context.Metrics.Where(x => x.Value == 0).ToList();
            _context.Metrics.RemoveRange(m);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int type, DateTime dt)
        {
            var m = _context.Metrics.Where(x => x.Type == type && x.Date == dt).ToList();
            _context.Metrics.RemoveRange(m);
            await _context.SaveChangesAsync();
        }

        public Metric LoadData(int type, int maxmin)
        {
            try
            {
                if (maxmin == 3)
                {
                    return _context.Metrics.Where(x => x.Type == type).OrderByDescending(x => x.Value).First();
                }
                else if (maxmin == 2)
                {
                    return _context.Metrics.Where(x => x.Type == type).OrderBy(x => x.Value).First();
                }
                else if (maxmin == 1)
                {
                    return _context.Metrics.Where(x => x.Type == type).OrderByDescending(x => x.Date).First();
                }
                else
                {
                    return _context.Metrics.Where(x => x.Type == type).OrderBy(x => x.Date).First();
                }
            }
            catch
            {
                return new Metric();
            }
        }

        public async Task<List<Metric>> Get(int type)
        {
            return await _context.Metrics.Where(x => x.Type == type).OrderByDescending(x => x.Date).ToListAsync();
        }

        private static IList<IList<ChartView>> GetResult(List<Metric> metrics, List<Metric> Prevmetrics)
        {
            var result = new List<ChartView>();
            foreach (var item in metrics.Where(x => x.Date != null))
            {
                var c = new ChartView
                {
                    Date = item.Date.Value.Year.ToString("D4") + "-" + item.Date.Value.Month.ToString("D2") + "-" + item.Date.Value.Day.ToString("D2") + " " + item.Date.Value.Hour.ToString("D2") + ":" + item.Date.Value.Minute.ToString("D2") + ":" + item.Date.Value.Second.ToString("D2"),
                    Total = item.Value
                };
                result.Add(c);
            }
            var prevresult = new List<ChartView>();
            foreach (var previtem in Prevmetrics.Where(x => x.Date != null))
            {
                var c = new ChartView
                {
                    Date = previtem.Date.Value.Year.ToString("D4") + "-" + previtem.Date.Value.Month.ToString("D2") + "-" + previtem.Date.Value.Day.ToString("D2") + " " + previtem.Date.Value.Hour.ToString("D2") + ":" + previtem.Date.Value.Minute.ToString("D2") + ":" + previtem.Date.Value.Second.ToString("D2"),
                    Total = previtem.Value
                };
                prevresult.Add(c);
            }
            var final = new List<IList<ChartView>>
            {
                result,
                prevresult
            };
            return final;
        }

        public async Task<IList<IList<ChartView>>> GetChart(MetricType type, MyChartType day, int OffSet)
        {
            var metrics = await _context.Metrics.Where(x => x.Type == (int)type).ToListAsync();
            List<Metric> LiveMetrics;
            List<Metric> PrevMetrics;
            if (type >= MetricType.Gas)
            {
                OffSet++;
            }

            if (day == MyChartType.Hourly)
            {
                LiveMetrics = metrics.Where(x => x.Date > DateTime.Now.AddHours(-24 * (OffSet + 1)) && x.Date <= DateTime.Now.AddHours(-24 * OffSet)).ToList();
                PrevMetrics = metrics.Where(x => x.Date > DateTime.Now.AddHours(-24 * (OffSet + 2)) && x.Date <= DateTime.Now.AddHours(-24 * (OffSet + 1))).ToList();
                return GetResult(LiveMetrics, PrevMetrics);
            }
            else if (day == MyChartType.Daily)
            {
                LiveMetrics = metrics.Where(x => x.Date > DateTime.Now.AddDays(-14)).ToList();
                PrevMetrics = metrics.Where(x => x.Date <= DateTime.Now.AddDays(-14) && x.Date > DateTime.Now.AddDays(-28)).ToList();
                return GetResult(LiveMetrics, PrevMetrics);
            }
            else
            {
                LiveMetrics = metrics.ToList();
                PrevMetrics = metrics.ToList();
                return GetResult(LiveMetrics, PrevMetrics);
            }
        }
    }
}
