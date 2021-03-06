﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRED.Proxme.Mvc.Models
{
    public class WallElement
    {
        public string Id { get; set; }
        public string Location { get; set; }
        public int LocationId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string TempId  { get; set; }
        public DateTime CreatedTime { get; set; }
        public string StandardResolutionUrl { get; set; }
        public int PhotoHeight { get; set; }
        public int PhotoWidth { get; set; }
        public string LowResoltionUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public double Distance { get; set; }
        public double Bearing { get; set; }
        public string Direction { get; set; }
        public string VideoUrl { get; set; }
        public int VideoHeight { get; set; }
        public int VideoWidth { get; set; }
        public CellCoordinats Cell { get; set; }
    }

    public class CellCoordinats
    {
        public int startX { get; set; }
        public int startY { get; set; }
        public int endX { get; set; }
        public int endY { get; set; }
    }

    class WallElementComparer : IEqualityComparer<WallElement>
    {
        public bool Equals(WallElement x, WallElement y)
        {
            return
                x.Username == y.Username &&
                x.FullName == y.FullName;
        }

        public int GetHashCode(WallElement obj)
        {
            return obj.ProfilePictureUrl.GetHashCode();
        }

    }

}
