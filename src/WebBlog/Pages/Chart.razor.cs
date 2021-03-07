using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebBlog.Data;
using WebBlog.Data.Services;

namespace WebBlog.Pages
{
    public class ChartBase : ComponentBase
    {
        [Inject] private NavigationManager UriHelper { get; set; }
        [Inject] private MetricService MetricService { get; set; }
        protected string title;
        private IList<IList<ChartView>> hourlyChart;
        protected IList<IList<ChartView>> dailyChart;
        private IList<IList<ChartView>> weeklyChart;

        protected List<string> hourlyLabel = new();
        protected List<string> dailyLabel = new();
        protected List<string> weeklyLabel = new();

        protected List<decimal> hourlyData = new();
        protected List<decimal> dailyData = new();
        protected List<decimal> weeklyData = new();

        protected List<decimal> hourlyPrevData = new();
        protected List<decimal> dailyPrevData = new();
        protected List<decimal> weeklyPrevData = new();

        [Parameter]
        public int OffSet { get; set; }

        [Parameter]
        public MetricType Type { get; set; } = 0;

        protected override async Task OnInitializedAsync()
        {
            await Load();
        }

        protected void Prev()
        {
            Type--;
            if (Type < MetricType.TwitterFollowers)
            {
                Type = MetricType.Electricity;
            }
            if (Type > MetricType.DevToComments)
            {
                UriHelper.NavigateTo("/metrics/chart/" + (int)Type + "/1", true);
            }
            else
            {
                UriHelper.NavigateTo("/metrics/chart/" + (int)Type, true);
            }
        }

        protected void Next()
        {
            Type++;
            if (Type > MetricType.Electricity)
            {
                Type = MetricType.TwitterFollowers;
            }
            if (Type > MetricType.DevToComments)
            {
                UriHelper.NavigateTo("/metrics/chart/" + (int)Type + "/1", true);
            }
            else
            {
                UriHelper.NavigateTo("/metrics/chart/" + (int)Type, true);
            }
        }

        protected void ReLoad(int val)
        {
            UriHelper.NavigateTo("/metrics/chart/" + (int)Type + "/" + val, true);
        }

        protected async Task Load()
        {
            hourlyChart = await MetricService.GetChart(Type, MyChartType.Hourly, OffSet);
            foreach (var subitem in hourlyChart[0].OrderBy(x => x.Date))
            {
                if (subitem.Total.HasValue)
                {
                    hourlyLabel.Add(subitem.Date);
                    hourlyData.Add(subitem.Total.Value);
                }
            }
            foreach (var subitem in hourlyChart[1].OrderBy(x => x.Date))
            {
                if (subitem.Total.HasValue)
                {
                    hourlyLabel.Add(subitem.Date);
                    hourlyPrevData.Add(subitem.Total.Value);
                }
            }

            dailyChart = await MetricService.GetChart(Type, MyChartType.Daily, OffSet);
            if (Type == MetricType.Gas || Type == MetricType.Electricity)
            {
                var result =
                    from s in dailyChart[0].OrderBy(x => x.Date)
                    group s by new { Date = new DateTime(DateTime.Parse(s.Date).Year, DateTime.Parse(s.Date).Month, DateTime.Parse(s.Date).Day) } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };
                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date.ToString());
                    dailyData.Add(item.Value.Value);
                }

                result =
                    from s in dailyChart[1].OrderBy(x => x.Date)
                    group s by new { Date = new DateTime(DateTime.Parse(s.Date).Year, DateTime.Parse(s.Date).Month, DateTime.Parse(s.Date).Day) } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };
                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date.ToString());
                    dailyPrevData.Add(item.Value.Value);
                }
            }
            else
            {
                foreach (var subitem in dailyChart[0].OrderBy(x => x.Date).Where(y => DateTime.Parse(y.Date).Hour == DateTime.Now.AddHours(-1).Hour))
                {
                    if (subitem.Total.HasValue)
                    {
                        dailyLabel.Add(subitem.Date);
                        dailyData.Add(subitem.Total.Value);
                    }
                }
                foreach (var subitem in dailyChart[1].OrderBy(x => x.Date).Where(y => DateTime.Parse(y.Date).Hour == DateTime.Now.AddHours(-1).Hour))
                {
                    if (subitem.Total.HasValue)
                    {
                        dailyLabel.Add(subitem.Date);
                        dailyPrevData.Add(subitem.Total.Value);
                    }
                }
            }

            weeklyChart = await MetricService.GetChart(Type, MyChartType.Weekly, OffSet);
            if (Type == MetricType.Gas || Type == MetricType.Electricity)
            {
                var result =
                    from s in weeklyChart[0].OrderBy(x => x.Date)
                    group s by new { Date = new DateTime(DateTime.Parse(s.Date).Year, DateTime.Parse(s.Date).Month, 1) } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };
                foreach (var item in result)
                {
                    weeklyLabel.Add(item.Date.ToString());
                    weeklyData.Add(item.Value.Value);
                }

                result =
                    from s in weeklyChart[1].OrderBy(x => x.Date)
                    group s by new { Date = new DateTime(DateTime.Parse(s.Date).Year, DateTime.Parse(s.Date).Month, 1) } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };
                foreach (var item in result)
                {
                    weeklyLabel.Add(item.Date.ToString());
                    weeklyPrevData.Add(item.Value.Value);
                }
            }
            else
            {
                foreach (var subitem in weeklyChart[0].OrderBy(x => x.Date).Where(y => DateTime.Parse(y.Date).DayOfWeek == DateTime.Now.DayOfWeek && DateTime.Parse(y.Date).Hour == DateTime.Now.AddHours(-1).Hour))
                {
                    if (subitem.Total.HasValue)
                    {
                        weeklyLabel.Add(subitem.Date);
                        weeklyData.Add(subitem.Total.Value);
                    }
                }
                foreach (var subitem in weeklyChart[1].OrderBy(x => x.Date).Where(y => DateTime.Parse(y.Date).DayOfWeek == DateTime.Now.DayOfWeek && DateTime.Parse(y.Date).Hour == DateTime.Now.AddHours(-1).Hour))
                {
                    if (subitem.Total.HasValue)
                    {
                        weeklyLabel.Add(subitem.Date);
                        weeklyPrevData.Add(subitem.Total.Value);
                    }
                }
            }

            title = GetEnumDescription(Type);
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}
