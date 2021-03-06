﻿using ChartJs.Blazor.ChartJS.Common.Axes;
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
using System.Drawing;
using System.Linq;
using WebBlog.Data;

namespace WebBlog.Components
{
    public class LineBase : ComponentBase
    {
        protected LineConfig _config;
        protected ChartJsLineChart _lineChartJs;

        [Parameter]
        public List<DateTime> Labels { get; set; }

        [Parameter]
        public List<decimal> Data { get; set; }

        [Parameter]
        public List<decimal> PrevData { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public MyChartType Day { get; set; }

        protected override void OnInitialized()
        {
            if (Day == MyChartType.Hourly)
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
            else if (Day == MyChartType.Daily)
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
                            Text = Title + " Monthly"
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
                                        Unit = TimeMeasurement.Month,
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

            var Set = new LineDataset<TimeTuple<decimal>>
            {
                BackgroundColor = ColorUtil.FromDrawingColor(Color.Blue),
                BorderColor = ColorUtil.FromDrawingColor(Color.Blue),
                Fill = false,
                BorderWidth = 1,
                PointRadius = 5,
                PointBorderWidth = 1,
                SteppedLine = SteppedLine.False,
                ShowLine = true,
                Label = "Current"
            };

            var PrevSet = new LineDataset<TimeTuple<decimal>>
            {
                BackgroundColor = ColorUtil.FromDrawingColor(Color.LightBlue),
                BorderDash = new int[] { 10, 5 },
                BorderColor = ColorUtil.FromDrawingColor(Color.LightBlue),
                Fill = false,
                BorderWidth = 1,
                PointRadius = 3,
                PointBorderWidth = 1,
                SteppedLine = SteppedLine.False,
                ShowLine = true,
                Label = "Previous"
            };

            for (int i = 0; i < Data.Count; i++)
            {
                var s = Labels[i];
                var points = new TimeTuple<decimal>(new Moment(s), Convert.ToDecimal(Data[i]));
                Set.Add(points);
            }

            for (int i = 0; i < (Data.Count < PrevData.Count ? Data.Count : PrevData.Count); i++)
            {
                var s = Labels[i];
                var points = new TimeTuple<decimal>(new Moment(s), Convert.ToDecimal(PrevData[i]));
                PrevSet.Add(points);
            }

            if (Labels.Count > 0)
            {
                DateTime dt = Labels.OrderByDescending(s => s.Date).FirstOrDefault();
                Set.Label = dt.ToString("yyyy-MM-dd");
                if (Day == MyChartType.Hourly)
                {
                    PrevSet.Label = dt.AddDays(-1).ToString("yyyy-MM-dd");
                }
                else if (Day == MyChartType.Daily)
                {
                    PrevSet.Label = dt.AddDays(-14).ToString("yyyy-MM-dd");
                }
                else
                {
                    PrevSet.Label = "N/A";
                    PrevSet.Hidden = true;
                }
            }

            _config.Data.Datasets.Add(Set);
            _config.Data.Datasets.Add(PrevSet);
        }
    }
}
