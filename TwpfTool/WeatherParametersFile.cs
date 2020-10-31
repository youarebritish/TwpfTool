﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace TwpfTool
{
    public sealed class WeatherParametersFile
    {
        public enum TrackType
        {
            Strength = 1,
            Rgb = 2,
            Lut = 4
        }

        private int tppGlobalVolumetricFogEntryCount;
        private int structCount;

        private TppGlobalVolumetricFog tppGlobalVolumetricFog;
        private readonly IList<GenericStruct> genericStructs = new List<GenericStruct>();

        public bool Read(BinaryReader reader)
        {
            this.ReadHeader(reader);
            this.ReadTppGlobalVolumetricFog(reader);

            for (var i = 0; i < this.structCount - 1; i++)
            {
                this.ReadGenericStruct(reader);
            }

            return true;
        }

        private bool ReadHeader(BinaryReader reader)
        {
            var format = reader.ReadChars(7);
            var version = reader.ReadByte();
            var magic1 = reader.ReadInt32();
            this.structCount = reader.ReadInt32();
            var tagCount = reader.ReadInt32();

            var structOffsets = new uint[this.structCount - 1];
            for (var i = 0; i < this.structCount - 1; i++)
            {
                structOffsets[i] = reader.ReadUInt32();
            }

            this.tppGlobalVolumetricFogEntryCount = reader.ReadUInt16();
            var magic2 = reader.ReadUInt16();

            var tppGlobalVolumetricFogEntryOffsets = new uint[this.tppGlobalVolumetricFogEntryCount];
            for (var i = 0; i < this.tppGlobalVolumetricFogEntryCount; i++)
            {
                tppGlobalVolumetricFogEntryOffsets[i] = reader.ReadUInt32();
            }

            return true;
        }

        private bool ReadTppGlobalVolumetricFog(BinaryReader reader)
        {
            this.tppGlobalVolumetricFog = TppGlobalVolumetricFog.Read(reader, this.tppGlobalVolumetricFogEntryCount);
            return true;
        }

        private bool ReadGenericStruct(BinaryReader reader)
        {
            var result = GenericStruct.Read(reader);
            this.genericStructs.Add(result);
            return true;
        }
    }
}
