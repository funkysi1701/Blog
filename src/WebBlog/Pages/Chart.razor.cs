using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public int Type { get; set; } = 0;

        protected override async Task OnInitializedAsync()
        {
            await Load();
        }

        protected void Prev()
        {
            Type--;
            if (Type < 0)
            {
                Type = 15;
            }
            UriHelper.NavigateTo("/metrics/chart/" + Type, true);
        }

        protected void Next()
        {
            Type++;
            if (Type > 15)
            {
                Type = 0;
            }
            UriHelper.NavigateTo("/metrics/chart/" + Type, true);
        }

        private async Task Load()
        {
            dailyChart = await MetricService.GetChart(Type, MyChartType.Daily);
            if (Type == 14 || Type == 15)
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

            hourlyChart = await MetricService.GetChart(Type, MyChartType.Hourly);
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

            weeklyChart = await MetricService.GetChart(Type, MyChartType.Weekly);
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

            title = Type switch
            {
                0 => "Twitter Followers",
                1 => "Twitter Following",
                2 => "Number Of Tweets",
                3 => "Twitter Favourites",
                4 => "GitHub Followers",
                5 => "GitHub Following",
                6 => "GitHub Repo",
                7 => "GitHub Stars",
                8 => "GitHub Commits",
                9 => "DevTo Posts",
                10 => "DevTo Published Posts",
                11 => "DevTo Views",
                12 => "DevTo Reactions",
                13 => "DevTo Comments",
                14 => "Gas",
                15 => "Elec",
                _ => "Unknown",
            };
        }
    }
}
