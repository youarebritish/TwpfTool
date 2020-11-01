using NanoXLSX;
using NanoXLSX.Styles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TwpfTool
{
    public class SpreadsheetBuilder
    {
        private struct TimeColor
        {
            public readonly byte Red;
            public readonly byte Green;
            public readonly byte Blue;

            public TimeColor(byte red, byte green, byte blue)
            {
                this.Red = red;
                this.Green = green;
                this.Blue = blue;
            }

            public override string ToString() => $"FF{this.Red:X2}{this.Green:X2}{this.Blue:X2}";
        }

        private readonly WeatherParametersFile twpf;
        private readonly Workbook workbook;
        private readonly Style headerStyle = MakeHeaderStyle();
        private readonly Style tagStyle = MakeTagStyle();
        private readonly Style emptyStyle = MakeEmptyStyle();
        private readonly Style timeStyle = MakeTimeStyle();

        private readonly IDictionary<uint, TimeColor> timeColors = MakeTimeColors();

        private int width;

        public SpreadsheetBuilder (WeatherParametersFile twpf, string name)
        {
            this.twpf = twpf;
            this.workbook = new Workbook($"{name}.xlsx", "Weather 0");
        }

        public void Build()
        {
            var times = this.GetTimes();
            this.width = times.Count + 2;

            var allParams = this.twpf.TppGlobalVolumetricFog.Parameters;
            foreach(var @struct in this.twpf.GenericStructs)
            {
                foreach(var param in @struct.Parameters)
                {
                    allParams.Add(param);
                }
            }

            var weatherIds = (from param in allParams
                              from setting in param.Settings
                              from track in setting.Tracks
                              select track.WeatherId)
                              .Distinct()
                              .OrderBy(val => val)
                              .ToList();

            foreach (var weatherId in weatherIds)
            {
                if (weatherId != 0)
                {
                    this.workbook.AddWorksheet($"Weather={weatherId}");
                }

                foreach (var tag in this.twpf.Tags)
                {
                    this.RenderTagRow(tag);
                    this.RenderParameterHeaderRow();
                    this.RenderTimeRow(times);
                    this.RenderTagParams("TppGlobalVolumetricFog", this.twpf.TppGlobalVolumetricFog.Parameters, tag, weatherId, times);

                    foreach (var @struct in this.twpf.GenericStructs)
                    {
                        this.RenderTagParams(@struct.ToString(), @struct.Parameters, tag, weatherId, times);
                    }

                    this.RenderEmptyRow();
                }
            }

            workbook.Save();
        }

        private static IDictionary<uint, TimeColor> MakeTimeColors()
        {
            return new Dictionary<uint, TimeColor>
            {
                { 0, new TimeColor(50, 89, 134) },
                { 284, new TimeColor(46, 89, 139) },
                { 324, new TimeColor(146, 172, 205) },
                { 344, new TimeColor(143, 174, 207) },
                { 363, new TimeColor(219, 217, 216) },
                { 365, new TimeColor(252, 211, 181) },
                { 374, new TimeColor(246, 210, 179) },
                { 392, new TimeColor(249, 209, 177) },
                { 451, new TimeColor(250, 249, 203) },
                { 737, new TimeColor(248, 248, 211) },
                { 1024, new TimeColor(249, 248, 205) },
                { 1067, new TimeColor(250, 208, 181) },
                { 1084, new TimeColor(244, 206, 177) },
                { 1095, new TimeColor(247, 213, 180) },
                { 1096, new TimeColor(217, 217, 220) },
                { 1115, new TimeColor(145, 176, 206) },
                { 1134, new TimeColor(153, 172, 208) },
                { 1174, new TimeColor(48, 90, 113) }
            };
        }

        private TimeColor GetTimeColor(uint time)
        {
            if (this.timeColors.ContainsKey(time))
            {
                return this.timeColors[time];
            }

            var keys = this.timeColors.Keys.OrderBy(val => val).ToList();
            for(var i = 0; i < keys.Count; i++)
            {
                var keyTime = keys[i];
                if (keyTime <= time)
                {
                    continue;
                }

                var prevKeyTime = keys[i - 1];
                var range = (keyTime - prevKeyTime);
                var diff = time - prevKeyTime;
                var lerpFactor = (float)diff / (float)range;

                var prevColor = this.timeColors[prevKeyTime];
                var thisColor = this.timeColors[keyTime];
                var red = (byte)(prevColor.Red + (byte)(lerpFactor * (thisColor.Red - prevColor.Red)));
                var green = (byte)(prevColor.Green + (byte)(lerpFactor * (thisColor.Green - prevColor.Green)));
                var blue = (byte)(prevColor.Blue + (byte)(lerpFactor * (thisColor.Blue - prevColor.Blue)));

                return new TimeColor(red, green, blue);
            }

            return new TimeColor(0, 0, 0);
        }

        private void RenderTagParams(string structName, IEnumerable<Parameter> parameters, string tag, ushort weatherId, IList<uint> times)
        {
            var filteredParameters = (from param in parameters
                                     from setting in param.Settings
                                     where setting.Tag == tag
                                     select param).ToList();

            var settings = (from param in filteredParameters
                            from setting in param.Settings
                            where setting.Tag == tag
                            select setting).ToList();

            if (settings.Count == 0)
            {
                return;
            }

            this.workbook.WS.Value(structName);
            var isFirst = true;

            foreach (var param in filteredParameters)
            {
                if (!isFirst)
                {
                    this.workbook.WS.Value(string.Empty);
                }

                if (param.TrackType != WeatherParametersFile.TrackType.Rgb)
                {
                    this.RenderScalarParam(param, weatherId, times);
                }
                else
                {
                    this.RenderColorParam(param, weatherId, times);
                }

                this.workbook.WS.Down();

                isFirst = false;
            }
        }

        private void RenderKeyframes(IList<Keyframe> keyframes, IList<uint> times, Action<Keyframe> renderKeyframe)
        {
            var currentColumn = 0;
            foreach (var keyframe in keyframes)
            {
                var time = keyframe.Time;
                var timeIndex = times.IndexOf(time);

                while (currentColumn < timeIndex)
                {
                    this.workbook.WS.Value(string.Empty);
                    currentColumn++;
                }

                renderKeyframe(keyframe);
                currentColumn++;
            }
        }

        private void RenderTracks(IList<ITrack> tracks, ushort weatherId, IList<uint> times, Action<Keyframe> renderKeyframe)
        {
            foreach (var track in tracks)
            {
                if (track.WeatherId != weatherId)
                {
                    continue;
                }

                RenderKeyframes(track.GetKeyframes(), times, renderKeyframe);
            }
        }

        private void RenderScalarParam(Parameter param, ushort weatherId, IList<uint> times)
        {
            this.workbook.WS.Value(param.ToString());
            this.RenderTracks(param.Settings[0].Tracks, weatherId, times, keyframe => keyframe.WriteValue(this.workbook, this.emptyStyle));
        }

        private void RenderColorParam(Parameter param, ushort weatherId, IList<uint> times)
        {
            var tracks = param.Settings[0].Tracks;

            this.workbook.WS.Value(param.ToString() + "(.R)");
            this.RenderTracks(tracks, weatherId, times, keyframe => this.workbook.WS.Value((keyframe as ColorKeyframe)?.Value.Red));
            this.workbook.WS.Down();

            this.workbook.WS.Value(string.Empty);
            this.workbook.WS.Value(param.ToString() + "(.G)");
            this.RenderTracks(tracks, weatherId, times, keyframe => this.workbook.WS.Value((keyframe as ColorKeyframe)?.Value.Green));
            this.workbook.WS.Down();

            this.workbook.WS.Value(string.Empty);
            this.workbook.WS.Value(param.ToString() + "(.B)");
            this.RenderTracks(tracks, weatherId, times, keyframe => this.workbook.WS.Value((keyframe as ColorKeyframe)?.Value.Blue));
        }

        private void RenderTimeRow(IList<uint> times)
        {
            this.workbook.WS.Value("Time", this.timeStyle);
            this.workbook.WS.Value(string.Empty, this.timeStyle);

            for (var i = 2; i < this.width; i++)
            {
                var time = times[i - 2];
                var hours = Math.Floor((float)time / 60.0f);
                var minutes = time - (hours * 60);

                var timeStr = $"{hours:00}:{minutes:00}";

                var style = new Style();
                style.CurrentFill.ForegroundColor = this.GetTimeColor(time).ToString();
                style.CurrentFill.PatternFill = Fill.PatternValue.solid;

                this.workbook.WS.Value(timeStr, style);
            }

            this.workbook.WS.Down();
        }

        private void RenderEmptyRow()
        {
            for (var i = 0; i < this.width; i++)
            {
                this.workbook.WS.Value(string.Empty, this.emptyStyle);
            }

            this.workbook.WS.Down();
        }

        private void RenderTagRow(string tag)
        {
            this.workbook.WS.Value("Tag", this.headerStyle);
            this.workbook.WS.Value(tag, this.tagStyle);

            for(var i = 2; i < this.width; i++)
            {
                this.workbook.WS.Value(string.Empty, this.headerStyle);
            }

            this.workbook.WS.Down();
        }

        private void RenderParameterHeaderRow()
        {
            this.workbook.WS.Value("ParameterNames", this.headerStyle);
            this.workbook.WS.Value(string.Empty, this.headerStyle);
            this.workbook.WS.Value("Values", this.headerStyle);

            for (var i = 3; i < this.width; i++)
            {
                this.workbook.WS.Value(string.Empty, this.headerStyle);
            }

            this.workbook.WS.Down();
        }

        private static Style MakeHeaderStyle()
        {
            var style = new Style();
            style.CurrentFont.ColorValue = "FFFFFFFF";

            style.CurrentFill.ForegroundColor = "FF080403";
            style.CurrentFill.PatternFill = Fill.PatternValue.solid;

            return style;
        }

        private static Style MakeTagStyle()
        {
            var style = new Style();
            style.CurrentFont.ColorValue = "FFFFFFFF";
            style.CurrentFont.Bold = true;

            style.CurrentFill.ForegroundColor = "FF080403";
            style.CurrentFill.PatternFill = Fill.PatternValue.solid;

            return style;
        }

        private static Style MakeTimeStyle()
        {
            var style = new Style();

            style.CurrentFill.ForegroundColor = "FFA5A5A5";
            style.CurrentFill.PatternFill = Fill.PatternValue.solid;

            return style;
        }

        private static Style MakeEmptyStyle()
        {
            var style = new Style();
            return style;
        }

        private IList<uint> GetTimes()
        {
            var fogTimes = from param in this.twpf.TppGlobalVolumetricFog.Parameters
                           from setting in param.Settings
                           from track in setting.Tracks
                           from keyframe in track.GetKeyframes()
                           select keyframe.Time;

            var genericTimes = from @struct in this.twpf.GenericStructs
                               from param in @struct.Parameters
                               from setting in param.Settings
                               from track in setting.Tracks
                               from keyframe in track.GetKeyframes()
                               select keyframe.Time;

            return fogTimes
                    .Union(genericTimes)
                    .OrderBy(time => time)
                    .ToList();
        }
    }
}
