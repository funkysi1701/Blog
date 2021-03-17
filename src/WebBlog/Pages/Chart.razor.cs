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

        [Parameter]
        public int OffSet { get; set; }

        [Parameter]
        public MetricType Type { get; set; } = 0;

        protected string Title;

        protected IList<IList<ChartView>> dailyChart;
        private IList<IList<ChartView>> monthlyChart;

        protected List<DateTime> hourlyLabel = new();
        protected List<DateTime> dailyLabel = new();
        protected List<DateTime> monthlyLabel = new();

        protected List<decimal> hourlyData = new();
        protected List<decimal> dailyData = new();
        protected List<decimal> monthlyData = new();

        protected List<decimal> hourlyPrevData = new();
        protected List<decimal> dailyPrevData = new();
        protected List<decimal> monthlyPrevData = new();

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

            UriHelper.NavigateTo($"/metrics/chart/{(int)Type}/{OffSet}", true);
        }

        protected void PrevDay()
        {
            OffSet--;
            if (OffSet < 0)
            {
                OffSet = 30;
            }

            UriHelper.NavigateTo($"/metrics/chart/{(int)Type}/{OffSet}", true);
        }

        protected void NextDay()
        {
            OffSet++;
            if (OffSet > 30)
            {
                OffSet = 0;
            }

            UriHelper.NavigateTo($"/metrics/chart/{(int)Type}/{OffSet}", true);
        }

        protected void Next()
        {
            Type++;
            if (Type > MetricType.Electricity)
            {
                Type = MetricType.TwitterFollowers;
            }

            UriHelper.NavigateTo($"/metrics/chart/{(int)Type}/{OffSet}", true);
        }

        protected async Task LoadHourly()
        {
            IList<IList<ChartView>> hourlyChart = await MetricService.GetChart(Type, MyChartType.Hourly, OffSet);
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
        }

        protected async Task LoadDaily()
        {
            dailyChart = await MetricService.GetChart(Type, MyChartType.Daily, OffSet);
            if (Type == MetricType.Gas || Type == MetricType.Electricity)
            {
                var result =
                    from s in dailyChart[0].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };

                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date);
                    dailyData.Add(item.Value.Value);
                }

                result =
                    from s in dailyChart[1].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };

                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date);
                    dailyPrevData.Add(item.Value.Value);
                }
            }
            else
            {
                var result =
                    from s in dailyChart[0].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Max(x => x.Total),
                    };

                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date);
                    dailyData.Add(item.Value.Value);
                }

                result =
                    from s in dailyChart[1].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Max(x => x.Total),
                    };

                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date);
                    dailyPrevData.Add(item.Value.Value);
                }
            }
        }

        protected async Task LoadMonthly()
        {
            monthlyChart = await MetricService.GetChart(Type, MyChartType.Monthly, OffSet);
            if (Type == MetricType.Gas || Type == MetricType.Electricity)
            {
                var result =
                    from s in monthlyChart[0].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, 1)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };

                foreach (var item in result)
                {
                    monthlyLabel.Add(item.Date);
                    monthlyData.Add(item.Value.Value);
                }
            }
            else
            {
                var result =
                    from s in monthlyChart[0].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, 1)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Max(x => x.Total),
                    };

                foreach (var item in result)
                {
                    monthlyLabel.Add(item.Date);
                    monthlyData.Add(item.Value.Value);
                }
            }
        }

        protected async Task Load()
        {
            await LoadHourly();

            await LoadDaily();

            await LoadMonthly();

            Title = GetEnumDescription(Type);
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
