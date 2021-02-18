using ChartJs.Blazor.ChartJS.Common.Axes;
using ChartJs.Blazor.ChartJS.Common.Axes.Ticks;
using ChartJs.Blazor.ChartJS.Common.Enums;
using ChartJs.Blazor.ChartJS.Common.Properties;
using ChartJs.Blazor.ChartJS.Common.Time;
using ChartJs.Blazor.ChartJS.LineChart;
using ChartJs.Blazor.Charts;
using ChartJs.Blazor.Util;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace WebBlog.Components
{
    public class LineBase : ComponentBase
    {
        protected LineConfig _config;
        protected ChartJsLineChart _lineChartJs;

        [Parameter]
        public List<string> Labels { get; set; }

        [Parameter]
        public List<decimal> Data { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public int Day { get; set; }

        protected override void OnInitialized()
        {
            if (Day == 1)
            {
                _config = new LineConfig
                {
                    Options = new LineOptions
                    {
                        Title = new OptionsTitle
                        {
                            Display = true,
                            Text = Title + " Last Day"
                        },
                        Responsive = true,
                        Animation = new ArcAnimation
                        {
                            AnimateRotate = true,
                            AnimateScale = true
                        },
                        Scales = new Scales
                        {
                            xAxes = new List<CartesianAxis>
                        {
                            new TimeAxis
                            {
                                Distribution = TimeDistribution.Linear,
                                Ticks = new TimeTicks
                                {
                                    Source = TickSource.Auto,
                                    Reverse = true
                                },
                                Time = new TimeOptions
                                {
                                    Unit = TimeMeasurement.Hour,
                                    Round = TimeMeasurement.Second,
                                    TooltipFormat = "DD.MM.YYYY HH:mm:ss",
                                    DisplayFormats = TimeDisplayFormats.DE_CH
                                },

                                ScaleLabel = new ScaleLabel
                                {
                                    LabelString = "Time"
                                }
                            }
                        }
                        }
                    }
                };
            }
            else if (Day == 0)
            {
                _config = new LineConfig
                {
                    Options = new LineOptions
                    {
                        Title = new OptionsTitle
                        {
                            Display = true,
                            Text = Title + " Last 2 Weeks"
                        },
                        Responsive = true,
                        Animation = new ArcAnimation
                        {
                            AnimateRotate = true,
                            AnimateScale = true
                        },
                        Scales = new Scales
                        {
                            xAxes = new List<CartesianAxis>
                            {
                                new TimeAxis
                                {
                                    Distribution = TimeDistribution.Linear,
                                    Ticks = new TimeTicks
                                    {
                                        Source = TickSource.Auto,
                                        Reverse = true
                                    },
                                    Time = new TimeOptions
                                    {
                                        Unit = TimeMeasurement.Day,
                                        Round = TimeMeasurement.Hour,
                                        TooltipFormat = "DD.MM.YYYY",
                                        DisplayFormats = TimeDisplayFormats.DE_CH
                                    },

                                    ScaleLabel = new ScaleLabel
                                    {
                                        LabelString = "Date"
                                    }
                                }
                            }
                        }
                    }
                };
            }
            else
            {
                _config = new LineConfig
                {
                    Options = new LineOptions
                    {
                        Title = new OptionsTitle
                        {
                            Display = true,
                            Text = Title + " Weekly"
                        },
                        Responsive = true,
                        Animation = new ArcAnimation
                        {
                            AnimateRotate = true,
                            AnimateScale = true
                        },
                        Scales = new Scales
                        {
                            xAxes = new List<CartesianAxis>
                            {
                                new TimeAxis
                                {
                                    Distribution = TimeDistribution.Linear,
                                    Ticks = new TimeTicks
                                    {
                                        Source = TickSource.Auto,
                                        Reverse = true
                                    },
                                    Time = new TimeOptions
                                    {
                                        Unit = TimeMeasurement.Week,
                                        Round = TimeMeasurement.Day,
                                        TooltipFormat = "DD.MM.YYYY",
                                        DisplayFormats = TimeDisplayFormats.DE_CH
                                    },

                                    ScaleLabel = new ScaleLabel
                                    {
                                        LabelString = "Date"
                                    }
                                }
                            }
                        }
                    }
                };
            }

            var Set = new LineDataset<TimeTuple<int>>
            {
                BackgroundColor = ColorUtil.RandomColorString(),
                BorderColor = ColorUtil.RandomColorString(),
                Fill = false,
                BorderWidth = 1,
                PointRadius = 5,
                PointBorderWidth = 1,
                SteppedLine = SteppedLine.False,
                ShowLine = true
            };

            for (int i = 0; i < Data.Count; i++)
            {
                var s = Labels[i];
                var points = new TimeTuple<int>(new Moment(DateTime.Parse(s)), Convert.ToInt32(Data[i]));
                Set.Add(points);
            }

            _config.Data.Datasets.Add(Set);
        }
    }
}
