﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using OfflineMediaV3.Business.Models.WeatherModel;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Framework.Singleton;

namespace OfflineMediaV3.Business.Sources.OpenWeatherMap
{
    public class OpenWeatherMapHelper : SingletonBase<OpenWeatherMapHelper>
    {
        public Forecast EvaluateFeed(string feed, Dictionary<string, string> weatherFontMapping)
        {
            var res = new Forecast();
            if (feed != null)
            {
                try
                {
                    var f = JsonConvert.DeserializeObject<Models.Forecast.RootObject>(feed);
                    res.City = f.city.name;
                    if (!string.IsNullOrEmpty(f.city.country))
                        res.City += " (" + f.city.country + ")";
                    res.ForecastItems = new List<ForecastItem>();
                    foreach (var entry in f.list)
                    {
                        var item = new ForecastItem { Date = ConvertFromUnixTimestamp(entry.dt) };

                        if (entry.main != null)
                        {
                            item.HumidityPercentage = entry.main.humidity;
                            item.PressurehPa = entry.main.pressure;
                            item.TemperatureKelvin = entry.main.temp;
                        }

                        if (entry.weather != null && entry.weather.Any())
                        {
                            var weather = entry.weather.FirstOrDefault();
                            item.ConditionId = weather.id;
                            if (weatherFontMapping.ContainsKey(weather.id.ToString()))
                                item.ConditionFontIcon = ((char)int.Parse(weatherFontMapping[weather.id.ToString()], System.Globalization.NumberStyles.HexNumber)).ToString();
                            item.Description = weather.description;
                        }

                        if (entry.clouds != null)
                        {
                            item.CloudinessPercentage = entry.clouds.all;
                        }

                        if (entry.wind != null)
                        {
                            item.WindDegreee = entry.wind.deg;
                            item.WindSpeed = entry.wind.speed;
                        }

                        if (entry.rain != null)
                        {
                            item.RainVolume = entry.rain._3h;
                        }

                        if (entry.snow != null)
                        {
                            item.RainVolume = entry.snow._3h;
                        }

                        res.ForecastItems.Add(item);
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.EvaluateFeed failed", ex);
                }
            }
            return res;
        }

        private static DateTime ConvertFromUnixTimestamp(int timestamp)
        {
            var original = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return original.AddSeconds(timestamp);

        }
    }
}