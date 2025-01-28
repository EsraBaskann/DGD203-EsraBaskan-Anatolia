using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace JourneyThroughAnatolia
{
    public class SaveManager
    {
        private readonly Game _game;
        private Player _player;
        private GameMap _map;
        private readonly string _saveDirectory;
        private const string SaveExtension = ".sgf";

        public SaveManager(Game game)
        {
            _game = game;
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            _saveDirectory = Path.Combine(projectDirectory, "saves");
            
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
                Console.WriteLine($"Created save directory at: {_saveDirectory}");
            }
        }

        public void Initialize(Player player, GameMap map)
        {
            _player = player;
            _map = map;
        }

        public void SaveGame()
        {
            Console.Clear();
            Console.WriteLine("=== Save Game ===\n");
            Console.Write("Enter save name: ");
            string saveName = Console.ReadLine()?.Trim() ?? DateTime.Now.ToString("yyyyMMdd_HHmmss");
            
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                saveName = saveName.Replace(c, '_');
            }

            string savePath = Path.Combine(_saveDirectory, saveName + SaveExtension);
            Console.WriteLine($"\nSaving to: {savePath}");
            
            using (StreamWriter writer = new StreamWriter(savePath))
            {
                // Save basic info
                writer.WriteLine(_player.Name);
                writer.WriteLine($"{_player.CurrentLocation.Coordinates.X},{_player.CurrentLocation.Coordinates.Y}");
                writer.WriteLine(_player.WisdomPoints);
                
                // Save artifacts
                writer.WriteLine(_player.CollectedArtifacts.Count);
                foreach (var artifact in _player.CollectedArtifacts)
                {
                    writer.WriteLine(artifact);
                }
                
                // Save quests
                writer.WriteLine(_player.CompletedQuests.Count);
                foreach (var quest in _player.CompletedQuests)
                {
                    writer.WriteLine($"{quest.Key},{quest.Value}");
                }
            }
            
            Console.WriteLine($"Game saved successfully as '{saveName}{SaveExtension}'!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public bool LoadGame()
        {
            Console.Clear();
            Console.WriteLine("=== Load Game ===\n");
            Console.WriteLine($"Looking for saves in: {_saveDirectory}\n");

            var saves = ListSaves();
            if (saves.Count == 0)
            {
                Console.WriteLine("No save files found.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return false;
            }

            Console.WriteLine($"Found {saves.Count} save files:\n");
            for (int i = 0; i < saves.Count; i++)
            {
                try
                {
                    var saveLines = File.ReadAllLines(saves[i]);
                    var saveName = Path.GetFileNameWithoutExtension(saves[i]);
                    var playerName = saveLines[0];
                    var coords = saveLines[1].Split(',');
                    var position = new Vector2Int(int.Parse(coords[0]), int.Parse(coords[1]));
                    var locationName = _map.GetLocationAtPosition(position)?.Name ?? "Unknown";
                    var wisdomPoints = int.Parse(saveLines[2]);
                    var artifactCount = int.Parse(saveLines[3]);

                    Console.WriteLine($"{i + 1}. {saveName}");
                    Console.WriteLine($"   Player: {playerName}");
                    Console.WriteLine($"   Location: {locationName} ({position.X}, {position.Y})");
                    Console.WriteLine($"   Wisdom Points: {wisdomPoints}");
                    Console.WriteLine($"   Artifacts: {artifactCount}\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(saves[i])} (Corrupted save file)");
                }
            }

            Console.Write("Enter save number to load (or 0 to cancel): ");
            if (int.TryParse(Console.ReadLine()?.Trim(), out int choice) && choice > 0 && choice <= saves.Count)
            {
                try
                {
                    var lines = File.ReadAllLines(saves[choice - 1]);
                    int currentLine = 0;

                    // Load basic info
                    _player.Name = lines[currentLine++];
                    var coords = lines[currentLine++].Split(',');
                    var position = new Vector2Int(int.Parse(coords[0]), int.Parse(coords[1]));
                    
                    // Set location
                    var location = _map.GetLocationAtPosition(position);
                    if (location != null)
                    {
                        _player.CurrentLocation = location;
                    }

                    // Load wisdom points
                    _player.WisdomPoints = int.Parse(lines[currentLine++]);

                    // Load artifacts
                    _player.CollectedArtifacts.Clear();
                    int artifactCount = int.Parse(lines[currentLine++]);
                    for (int i = 0; i < artifactCount; i++)
                    {
                        _player.AddArtifact(lines[currentLine++]);
                    }

                    // Load quests
                    _player.CompletedQuests.Clear();
                    int questCount = int.Parse(lines[currentLine++]);
                    for (int i = 0; i < questCount; i++)
                    {
                        var questData = lines[currentLine++].Split(',');
                        _player.CompletedQuests[questData[0]] = bool.Parse(questData[1]);
                    }

                    Console.WriteLine($"\nLoaded save successfully!");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError loading save file: {ex.Message}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return false;
                }
            }

            return false;
        }

        private List<string> ListSaves()
        {
            try
            {
                return Directory.GetFiles(_saveDirectory, $"*{SaveExtension}").ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing saves: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
