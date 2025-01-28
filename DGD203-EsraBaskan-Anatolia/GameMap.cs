using System;
using System.Collections.Generic;
using System.Text;

namespace JourneyThroughAnatolia
{
    public enum Direction
    {
        East,
        North,
        West,
        South
    }

    public class GameMap
    {
        private Dictionary<Vector2Int, MapLocationData> _locations;
        public MapLocationData StartLocation { get; private set; }
        private const int MapWidth = 3;
        private const int MapHeight = 3;
        private Vector2Int _playerPosition;

        public GameMap()
        {
            _locations = new Dictionary<Vector2Int, MapLocationData>();
            _playerPosition = new Vector2Int(1, 2);
        }

        public void InitializeMap()
        {
            CreateNorthernRealm();
            CreateEasternKingdom();
            CreateSouthernDesert();
            CreateWesternForest();
            ConnectLocations();
        }

        private void CreateNorthernRealm()
        {
            Vector2Int coordinates = new Vector2Int(1, 2);
            
            var north = new MapLocationData
            {
                Name = "Karadeniz Yaylalari",
                Description = "The Black Sea highlands stretch before you, adorned with a thousand shades of green. " +
                            "Tea gardens climb the misty mountain slopes like giant steps. " +
                            "The sound of the kemence (traditional violin) dances with the wind, while clouds " +
                            "float freely over the peaks, performing their own highland dance. In the heart of nature, " +
                            "traces of tradition reveal themselves in every corner.",
                Coordinates = coordinates
            };
            
            // Add Tea Master NPC
            var teaMaster = new NPC("Teyze Fatma", "An elderly woman with kind eyes and hands stained from years of picking tea leaves.", "Mystical Tea Leaves");
            teaMaster.AddDialogueOption("tea", "Ah, our tea! It's not just a drink, it's our way of life. Every sip tells a story of these misty mountains.");
            teaMaster.AddDialogueOption("traditions", "In these mountains, we rise with the sun and work until the mist covers the tea gardens.");
            teaMaster.AddDialogueOption("weather", "The rain and mist are our constant companions. They give our tea its special character.");
            north.LocationNPC = teaMaster;
            
            _locations[coordinates] = north;
            StartLocation = north;
        }

        private void CreateEasternKingdom()
        {
            Vector2Int coordinates = new Vector2Int(2, 1);
            
            var east = new MapLocationData
            {
                Name = "Dogu Anadolu",
                Description = "In the shadow of majestic Mount Agri (Ararat), vast plateaus stretch as far as the eye can see. " +
                            "The crystal-blue waters of ancient Lake Van sparkle in the sunlight, while historic " +
                            "structures whisper tales thousands of years old. Wild horses run freely across the high " +
                            "plateaus, reflecting the untamed spirit of these lands.",
                Coordinates = coordinates
            };

            // Add Local Guide NPC
            var guide = new NPC("Dede Korkut", "An old storyteller with a flowing white beard and eyes that sparkle with ancient wisdom.", "Ancient Scroll of Epics");
            guide.AddDialogueOption("history", "My child, these lands have been home to countless epics. The tales written in the shadow of Mount Agri still echo through generations.");
            guide.AddDialogueOption("legends", "They say a monster lives in the depths of Lake Van. But if you ask me, the real treasures lie in the culture of these lands.");
            guide.AddDialogueOption("traditions", "In our lands, guests are treated like kings. Every meal holds a story, every cup of tea carries a conversation.");
            east.LocationNPC = guide;
            
            _locations[coordinates] = east;
        }

        private void CreateSouthernDesert()
        {
            Vector2Int coordinates = new Vector2Int(1, 0);
            
            var south = new MapLocationData
            {
                Name = "Guney Akdeniz",
                Description = "The turquoise waters of the Mediterranean caress golden beaches. Ancient Lycian " +
                            "ruins stand defiantly against time on the mountainsides. The air is filled with the " +
                            "sweet scent of orange blossoms. As the eternal flames of Olympos dance in the darkness " +
                            "of night, history and nature merge into a single magnificent tableau.",
                Coordinates = coordinates
            };

            // Add Mediterranean Fisherman NPC
            var fisherman = new NPC("Kaptan Mehmet", "A weathered fisherman with sun-bleached hair and a warm smile.", "Ancient Trident of Poseidon");
            fisherman.AddDialogueOption("sea", "The Mediterranean has been my home for forty years. She can be gentle as a mother or fierce as a warrior.");
            fisherman.AddDialogueOption("fishing", "Every morning, we set sail before dawn. The sea provides for those who respect her.");
            fisherman.AddDialogueOption("legends", "There are many tales of ancient treasures beneath these waters. Some say they're protected by the old gods themselves.");
            south.LocationNPC = fisherman;
            
            _locations[coordinates] = south;
        }

        private void CreateWesternForest()
        {
            Vector2Int coordinates = new Vector2Int(0, 1);
            
            var west = new MapLocationData
            {
                Name = "Ege Kiyilari",
                Description = "Silver leaves of olive trees shimmer in the sunlight, while ancient stone " +
                            "paths lead to the secrets of Efes (Ephesus). The crystal-clear blue of the Aegean " +
                            "sparkles in the embrace of hidden coves. Purple bougainvillea cascade down " +
                            "white-washed walls, while the wind whispers tales thousands of years old.",
                Coordinates = coordinates
            };

            // Add Local Storyteller NPC
            var storyteller = new NPC("Anadolu Nine", "An elderly woman with a colorful headscarf and prayer beads, her eyes deep with memories of the past.", "Ancient Olive Branch");
            storyteller.AddDialogueOption("legends", "They say the stones of Efes can speak, child. A legend lies beneath every column. The footprints of Artemis still mark these lands.");
            storyteller.AddDialogueOption("traditions", "In our lands, every olive is sacred. Each olive that falls from its branch is heir to thousands of years of tradition.");
            storyteller.AddDialogueOption("history", "These shores have hosted many civilizations. Romans, Byzantines, Ottomans... Their traces are all here in these lands.");
            west.LocationNPC = storyteller;
            
            _locations[coordinates] = west;
        }

        private void ConnectLocations()
        {
            var north = _locations[new Vector2Int(1, 2)];
            var east = _locations[new Vector2Int(2, 1)];
            var south = _locations[new Vector2Int(1, 0)];
            var west = _locations[new Vector2Int(0, 1)];

            // Connect locations in a compass-like pattern
            // Northern Realm connects to East and West
            north.AddExit("east", east);
            north.AddExit("west", west);

            // Eastern Kingdom connects to North and South
            east.AddExit("north", north);
            east.AddExit("south", south);

            // Southern Desert connects to East and West
            south.AddExit("east", east);
            south.AddExit("west", west);

            // Western Forest connects to North and South
            west.AddExit("north", north);
            west.AddExit("south", south);
        }

        public string GetMapDisplay()
        {
            var sb = new StringBuilder();
            sb.AppendLine("  N  ");
            sb.AppendLine("W + E");
            sb.AppendLine("  S  ");
            return sb.ToString();
        }

        public MapLocationData GetLocationInDirection(MapLocationData currentLocation, string direction)
        {
            direction = direction.ToLower();
            if (currentLocation.Exits.ContainsKey(direction))
            {
                return currentLocation.Exits[direction];
            }
            return null;
        }

        public MapLocationData GetLocationAtPosition(Vector2Int position)
        {
            return _locations.ContainsKey(position) ? _locations[position] : null;
        }

        public Vector2Int GetPlayerPosition() => _playerPosition;

        public bool MovePlayer(Direction direction)
        {
            var newPosition = CalculateNewPosition(direction);
            return SetPlayerPosition(newPosition);
        }

        private Vector2Int CalculateNewPosition(Direction direction)
        {
            Vector2Int newPosition = _playerPosition;
            
            switch (direction)
            {
                case Direction.East:
                    newPosition.X += 1;
                    break;
                case Direction.North:
                    newPosition.Y += 1;
                    break;
                case Direction.West:
                    newPosition.X -= 1;
                    break;
                case Direction.South:
                    newPosition.Y -= 1;
                    break;
            }
            
            return newPosition;
        }

        private bool SetPlayerPosition(Vector2Int targetPosition)
        {
            if (IsValidPosition(targetPosition))
            {
                _playerPosition = targetPosition;
                return true;
            }
            return false;
        }

        private bool IsValidPosition(Vector2Int position)
        {
            return position.X >= 0 && position.X < MapWidth &&
                   position.Y >= 0 && position.Y < MapHeight &&
                   _locations.ContainsKey(position);
        }

        public IEnumerable<MapLocationData> GetAllLocations()
        {
            return _locations.Values;
        }
    }
}
