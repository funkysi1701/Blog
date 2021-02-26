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
        protected IList<ChartView> chart;
        private IList<ChartView> chart2;
        private IList<ChartView> chart3;
        protected string title;
        protected List<string> label = new();
        protected List<decimal> data = new();
        protected List<string> label2 = new();
        protected List<decimal> data2 = new();
        protected List<string> label3 = new();
        protected List<decimal> data3 = new();

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
            chart = await MetricService.GetChart(Type, 0);
            if (Type == 14 || Type == 15)
            {
                var result =
                    from s in chart.OrderBy(x => x.Date)
                    group s by new { Date = new DateTime(DateTime.Parse(s.Date).Year, DateTime.Parse(s.Date).Month, DateTime.Parse(s.Date).Day) } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };
                foreach (var item in result)
                {
                        label.Add(item.Date.ToString());
                        data.Add(item.Value.Value);
                }
            }
            else
            {
                foreach (var item in chart.OrderBy(x => x.Date).Where(y => DateTime.Parse(y.Date).Hour == DateTime.Now.AddHours(-1).Hour))
                {
                    if (item.Total.HasValue)
                    {
                        label.Add(item.Date);
                        data.Add(item.Total.Value);
                    }
                }
            }

            chart2 = await MetricService.GetChart(Type, 1);
            foreach (var item in chart2.OrderBy(x => x.Date))
            {
                if (item.Total.HasValue)
                {
                    label2.Add(item.Date);
                    data2.Add(item.Total.Value);
                }
            }
            chart3 = await MetricService.GetChart(Type, 2);
            foreach (var item in chart3.OrderBy(x => x.Date).Where(y => DateTime.Parse(y.Date).DayOfWeek == DateTime.Now.DayOfWeek && DateTime.Parse(y.Date).Hour == DateTime.Now.AddHours(-1).Hour))
            {
                if (item.Total.HasValue)
                {
                    label3.Add(item.Date);
                    data3.Add(item.Total.Value);
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
