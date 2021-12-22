using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ImageCreatorWebAPI
{
    public class LatLng : IEquatable<LatLng>
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }
        [JsonPropertyName("lng")]
        public double Lng { get; set; }
        [JsonPropertyName("alt")]
        public double? Alt { get; set; }

        public bool Equals(LatLng other)
        {
            return Math.Abs(other.Lat - Lat) < double.Epsilon && Math.Abs(other.Lng - Lng) < double.Epsilon;
        }

        public override bool Equals(object obj)
        {
            if (obj is LatLng latLng)
            {
                return Equals(latLng);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Lat.GetHashCode() * 397) ^ Lng.GetHashCode();
            }
        }

        public LatLng() : this(0, 0) { }

        public LatLng(double latitude, double longitude, double? altitude = null)
        {
            Lat = latitude;
            Lng = longitude;
            Alt = altitude;
        }

        public LatLng(string latlngString) : this()
        {
            var split = latlngString.Split(',');
            if (split.Length != 2)
            {
                return;
            }
            Lat = double.Parse(split.First());
            Lng = double.Parse(split.Last());
        }
    }
    
    public class LatLngTime : LatLng
    {
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }

        public LatLngTime()
            : this(0, 0) { }

        public LatLngTime(double latitude, double longitude, double? altitude = null)
            : base(latitude, longitude, altitude) { }
    }
    
    public class RouteSegmentData
    {
        [JsonPropertyName("routingType")]
        public string RoutingType { get; set; }
        [JsonPropertyName("routePoint")]
        public LatLng RoutePoint { get; set; }
        [JsonPropertyName("latlngs")]
        public List<LatLngTime> Latlngs { get; set; }

        public RouteSegmentData()
        {
            Latlngs = new List<LatLngTime>();
        }
    }
    
    public class RouteData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }
        [JsonPropertyName("opacity")]
        public double? Opacity { get; set; }
        [JsonPropertyName("weight")]
        public int? Weight { get; set; }
        [JsonPropertyName("markers")]
        public List<MarkerData> Markers { get; set; }
        [JsonPropertyName("segments")]
        public List<RouteSegmentData> Segments { get; set; }

        public RouteData()
        {
            Markers = new List<MarkerData>();
            Segments = new List<RouteSegmentData>();
        }
    }
    
    public class MarkerData
    {
        [JsonPropertyName("latlng")]
        public LatLng Latlng { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("urls")]
        public List<LinkData> Urls { get; set; }

        public MarkerData()
        {
            Urls = new List<LinkData>();
        }
    }
    
    public class LinkData
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; }
    }
    public class LayerData
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("minZoom")]
        public int? MinZoom { get; set; }
        [JsonPropertyName("maxZoom")]
        public int? MaxZoom { get; set; }
        [JsonPropertyName("opacity")]
        public double? Opacity { get; set; }
    }
    
    public class DataContainer
    {
        [JsonPropertyName("routes")]
        public List<RouteData> Routes { get; set; }
        [JsonPropertyName("northEast")]
        public LatLng NorthEast { get; set; }
        [JsonPropertyName("southWest")]
        public LatLng SouthWest { get; set; }
        [JsonPropertyName("baseLayer")]
        public LayerData BaseLayer { get; set; }
        [JsonPropertyName("overlays")]
        public List<LayerData> Overlays { get; set; }
    }
}