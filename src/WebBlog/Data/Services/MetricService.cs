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

        public async Task<List<ChartView>> GetChart(int type, int day)
        {
            var res = await _context.Metrics.Where(x => x.Type == type).ToListAsync();
            if (day == 1)
            {
                if (type == 14 || type == 15)
                {
                    res = res.Where(x => x.Date > DateTime.Now.AddHours(-72) && x.Date < DateTime.Now.AddHours(-48)).ToList();
                }
                else res = res.Where(x => x.Date > DateTime.Now.AddHours(-24)).ToList();
                var result = new List<ChartView>();
                foreach (var item in res.Where(x => x.Date != null))
                {
                    var c = new ChartView
                    {
                        Date = item.Date.Value.Year.ToString("D4") + "-" + item.Date.Value.Month.ToString("D2") + "-" + item.Date.Value.Day.ToString("D2") + " " + item.Date.Value.Hour.ToString("D2") + ":" + item.Date.Value.Minute.ToString("D2") + ":" + item.Date.Value.Second.ToString("D2"),
                        Total = item.Value
                    };
                    result.Add(c);
                }
                return result;
            }
            else if (day == 0)
            {
                res = res.Where(x => x.Date > DateTime.Now.AddDays(-14)).ToList();
                var result = new List<ChartView>();
                foreach (var item in res.Where(x => x.Date != null))
                {
                    var c = new ChartView
                    {
                        Date = item.Date.Value.Year.ToString("D4") + "-" + item.Date.Value.Month.ToString("D2") + "-" + item.Date.Value.Day.ToString("D2") + " " + item.Date.Value.Hour.ToString("D2") + ":" + item.Date.Value.Minute.ToString("D2") + ":" + item.Date.Value.Second.ToString("D2"),
                        Total = item.Value
                    };
                    result.Add(c);
                }
                return result;
            }
            else
            {
                res = res.ToList();
                var result = new List<ChartView>();
                foreach (var item in res.Where(x => x.Date != null))
                {
                    var c = new ChartView
                    {
                        Date = item.Date.Value.Year.ToString("D4") + "-" + item.Date.Value.Month.ToString("D2") + "-" + item.Date.Value.Day.ToString("D2") + " " + item.Date.Value.Hour.ToString("D2") + ":" + item.Date.Value.Minute.ToString("D2") + ":" + item.Date.Value.Second.ToString("D2"),
                        Total = item.Value
                    };
                    result.Add(c);
                }
                return result;
            }
        }

        
    }
}
