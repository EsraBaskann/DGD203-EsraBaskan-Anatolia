using System.Collections.Generic;

namespace JourneyThroughAnatolia
{
    public class MapLocationData
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public Vector2Int Coordinates { get; set; } = new Vector2Int();
        public Dictionary<string, MapLocationData> Exits { get; private set; }
        public NPC LocationNPC { get; set; }

        public MapLocationData()
        {
            Exits = new Dictionary<string, MapLocationData>();
        }

        public void AddExit(string direction, MapLocationData location)
        {
            Exits[direction.ToLower()] = location;
        }

        public List<string> GetAvailableExits()
        {
            return new List<string>(Exits.Keys);
        }

        public bool HasNPC()
        {
            return LocationNPC != null;
        }
    }
}
