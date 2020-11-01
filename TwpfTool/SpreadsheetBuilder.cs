using NanoXLSX;
using NanoXLSX.Styles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TwpfTool
{
    public class SpreadsheetBuilder
    {
        private readonly WeatherParametersFile twpf;
        private readonly Workbook workbook;
        private readonly Style headerStyle = MakeHeaderStyle();
        private readonly Style tagStyle = MakeTagStyle();
        private readonly Style emptyStyle = MakeEmptyStyle();
        private readonly Style timeStyle = MakeTimeStyle();

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

            var weatherId = (ushort)0;

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

            workbook.Save();
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
            this.workbook.WS.Value("Param " + param.ParamId);
            this.RenderTracks(param.Settings[0].Tracks, weatherId, times, keyframe => keyframe.WriteValue(this.workbook, this.emptyStyle));
        }

        private void RenderColorParam(Parameter param, ushort weatherId, IList<uint> times)
        {
            var tracks = param.Settings[0].Tracks;

            this.workbook.WS.Value("Param " + param.ParamId + "(.R)");
            this.RenderTracks(tracks, weatherId, times, keyframe => this.workbook.WS.Value((keyframe as ColorKeyframe)?.Value.Red));
            this.workbook.WS.Down();

            this.workbook.WS.Value(string.Empty);
            this.workbook.WS.Value("Param " + param.ParamId + "(.G)");
            this.RenderTracks(tracks, weatherId, times, keyframe => this.workbook.WS.Value((keyframe as ColorKeyframe)?.Value.Green));
            this.workbook.WS.Down();

            this.workbook.WS.Value(string.Empty);
            this.workbook.WS.Value("Param " + param.ParamId + "(.B)");
            this.RenderTracks(tracks, weatherId, times, keyframe => this.workbook.WS.Value((keyframe as ColorKeyframe)?.Value.Blue));
        }

        private void RenderTimeRow(IList<uint> times)
        {
            this.workbook.WS.Value("Time", this.timeStyle);
            this.workbook.WS.Value(string.Empty, this.timeStyle);

            for (var i = 2; i < this.width; i++)
            {
                this.workbook.WS.Value(times[i - 2], this.timeStyle);
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
