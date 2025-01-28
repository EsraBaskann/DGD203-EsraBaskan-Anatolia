using System;
using System.Collections.Generic;
using System.Linq;

namespace JourneyThroughAnatolia
{
    public class Game
    {
        private const string NewCommandSeparator = "----------------------------------------";
        private GameMap _gameMap;
        private Player _player;
        private SaveManager _saveManager;
        private bool _isRunning;
        private bool _usedTeaCup = false;

        public Game()
        {
            _gameMap = new GameMap();
            _gameMap.InitializeMap();
            _player = new Player();
            _player.CurrentLocation = _gameMap.StartLocation;
            _saveManager = new SaveManager(this);
            _saveManager.Initialize(_player, _gameMap);
            _isRunning = true;
        }

        public void Start()
        {
            MainMenu();
        }

        private void MainMenu()
        {
            while (_isRunning)
            {
                Console.Clear();
                Console.WriteLine("\n=== Journey Through Anatolia ===\n");
                Console.WriteLine("1. New Game");
                Console.WriteLine("2. Load Game");
                Console.WriteLine("3. Credits");
                Console.WriteLine("4. Exit");
                Console.Write("\nEnter your choice (1-4): ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        StartNewGame();
                        break;
                    case "2":
                        LoadGame();
                        break;
                    case "3":
                        ShowCredits();
                        break;
                    case "4":
                        Exit();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowCredits()
        {
            Console.Clear();
            Console.WriteLine("\n=== Credits ===\n");
            Console.WriteLine("Journey Through Anatolia");
            Console.WriteLine("A Text Adventure Game\n");
            Console.WriteLine("DGD-203");
            Console.WriteLine(" Game Programming Course");
            Console.WriteLine(" 2025 All Rights Reserved\n");
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }

        private void ReturnToMainMenu()
        {
            Console.WriteLine("\nPress any key to return to main menu...");
            Console.ReadKey();
            MainMenu();
        }

        private void StartNewGame()
        {
            Console.Clear();
            DisplayGameIntroduction();
            
            Console.Write("\nEnter your name, traveler: ");
            string playerName = Console.ReadLine();
            _player.Name = playerName;
            _player.CurrentLocation = _gameMap.StartLocation;

            Console.WriteLine($"\nWelcome, {playerName}! Your journey begins in the {_gameMap.StartLocation.Name}.");
            Console.WriteLine("\nPress any key to begin your adventure...");
            Console.ReadKey();
            
            GameLoop();
        }

        private void DisplayGameIntroduction()
        {
            Console.WriteLine("\n=== Journey Through Anatolia ===\n");
            Console.WriteLine("Welcome, brave adventurer, to the mystical lands of Anatolia!");
            Console.WriteLine("A great darkness looms over these ancient realms - the Shadow Dragon has awakened.");
            Console.WriteLine("This ancient evil threatens to plunge our world into eternal darkness.\n");
            
            Console.WriteLine("Your Quest:");
            Console.WriteLine("You must journey across Anatolia to find four sacred artifacts of power:");
            Console.WriteLine("\n1. The Ancient Tea Cup from Karadeniz");
            Console.WriteLine("   - Its mystical steam reveals hidden truths and banishes illusions");
            Console.WriteLine("\n2. The Sacred Olive Branch from Ege");
            Console.WriteLine("   - Blessed with the power to seal away ancient evils");
            Console.WriteLine("\n3. The Sun Disk from Ankara");
            Console.WriteLine("   - Its radiant light pierces through darkness");
            Console.WriteLine("\n4. The Wisdom Stone from İzmir");
            Console.WriteLine("   - Contains the knowledge of ages past\n");
            
            Console.WriteLine("Instructions:");
            Console.WriteLine("• Explore the lands using commands like 'go north', 'go south', etc.");
            Console.WriteLine("• Talk to the wise ones in each region using 'talk'");
            Console.WriteLine("• Collect artifacts using 'take' when you find them");
            Console.WriteLine("• Type 'help' to see all available commands");
            Console.WriteLine("• Save your progress anytime with 'save'\n");
            
            Console.WriteLine("Only by gathering all four artifacts can you hope to defeat the Shadow Dragon");
            Console.WriteLine("and prevent the apocalypse. Time is short - your journey begins now!\n");
        }

        private void LoadGame()
        {
            Console.Clear();
            if (_saveManager.LoadGame())
            {
                GameLoop();
            }
            else
            {
                Console.WriteLine("\nStarting new game...");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                StartNewGame();
            }
        }

        private void GameLoop()
        {
            while (_isRunning)
            {
                Console.Clear();
                DisplayCurrentLocation();
                ProcessCommand();
            }
        }

        private void DisplayCurrentLocation()
        {
            if (_player.CurrentLocation == null)
            {
                Console.WriteLine("Error: Current location is invalid. Returning to main menu...");
                WaitForKey();
                return;
            }

            var location = _player.CurrentLocation;
            Console.WriteLine($"\nYou are in {location.Name}");
            Console.WriteLine($"\n{location.Description}");

            Console.WriteLine();
            Console.Write(_gameMap.GetMapDisplay());
            
            Console.WriteLine("Available directions:");
            foreach (var exit in location.GetAvailableExits())
            {
                Console.WriteLine($"- {exit}");
            }

            if (location.HasNPC())
            {
                Console.WriteLine($"\nCharacters you can talk to:");
                Console.WriteLine($"- {location.LocationNPC.Name} ({location.LocationNPC.Description})");
            }

            Console.WriteLine("\nAvailable Commands:");
            Console.WriteLine("- go <direction>: Move in the specified direction");
            if (location.HasNPC())
            {
                Console.WriteLine("- talk: Talk to the character in the location");
            }
            Console.WriteLine("- help: Display all commands");
            Console.WriteLine("- save: Save your progress");
            Console.WriteLine("- clear: Clear the screen");
            Console.WriteLine("- quit: Exit the game");
            Console.WriteLine("- status: Display your collected artifacts and wisdom points");
            Console.WriteLine("- fight: Confront the apocalypse");
            
            Console.WriteLine(NewCommandSeparator);
            Console.Write("\nWhat would you like to do? ");
        }

        private void ProcessCommand()
        {
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                return;

            string[] parts = input.Trim().ToLower().Split(' ');
            string command = parts[0];

            switch (command)
            {
                case "go":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Go where? Please specify a direction.");
                        break;
                    }
                    Move(parts[1]);
                    break;

                case "talk":
                    if (_player.CurrentLocation.HasNPC())
                    {
                        TalkToNPC();
                    }
                    else
                    {
                        Console.WriteLine("There's no one here to talk to.");
                    }
                    WaitForKey();
                    break;

                case "help":
                    DisplayHelp();
                    WaitForKey();
                    break;

                case "save":
                    SaveGame();
                    break;

                case "quit":
                    Exit();
                    break;

                case "clear":
                    Console.Clear();
                    break;

                case "status":
                    DisplayStatus();
                    WaitForKey();
                    break;

                case "fight":
                    InitiateApocalypseBattle();
                    WaitForKey();
                    break;

                default:
                    Console.WriteLine("I don't understand that command. Type 'help' for a list of commands.");
                    WaitForKey();
                    break;
            }
        }

        private void TalkToNPC()
        {
            var npc = _player.CurrentLocation.LocationNPC;
            Console.WriteLine($"\nTalking to {npc.Name}...");
            Console.WriteLine($"{npc.Description}");
            
            while (true)
            {
                Console.WriteLine("\nWhat would you like to talk about?");
                Console.WriteLine("Available topics:");
                foreach (var topic in npc.DialogueOptions)
                {
                    Console.WriteLine($"- {topic}");
                }
                
                Console.WriteLine("- secret object (ask about any secret items)");
                Console.WriteLine("- leave (end conversation)");
                
                Console.Write("\nYour choice: ");
                string choice = Console.ReadLine().ToLower();

                if (choice == "leave")
                {
                    break;
                }
                else if (npc.DialogueOptions.Contains(choice))
                {
                    var (response, secretObject) = npc.GetResponse(choice);
                    Console.WriteLine(response);
                    _player.WisdomPoints++;
                    CheckForEnding();
                }
                else if (choice == "secret object")
                {
                    var (response, secretObject) = npc.GetResponse(choice);
                    Console.WriteLine(response);
                    if (secretObject != null)
                    {
                        Console.WriteLine($"\nYou received: {secretObject}");
                        _player.AddArtifact(secretObject);
                    }
                }
                else
                {
                    Console.WriteLine("I don't understand that topic.");
                }
            }
        }

        private void HandleQuest(string locationName)
        {
            string artifact = "";
            string questDescription = "";

            switch (locationName)
            {
                case "Karadeniz Yaylalari":
                    artifact = "Ancient Tea Cup";
                    questDescription = "Find the Ancient Tea Cup hidden in the misty tea gardens.";
                    break;
                case "Ege Kiyilari":
                    artifact = "Sacred Olive Branch";
                    questDescription = "Collect the Sacred Olive Branch from the ancient grove.";
                    break;
                default:
                    return;
            }

            if (!_player.HasCompletedQuest(locationName))
            {
                Console.WriteLine($"\nNew Quest: {questDescription}");
                Console.WriteLine($"You have found the {artifact}!");
                _player.AddArtifact(artifact);
                _player.CompleteQuest(locationName);
                CheckForEnding();
            }
        }

        private void CheckForEnding()
        {
            if (_player.HasAllArtifacts())
            {
                InitiateFinalBattle();
            }
        }

        private void InitiateFinalBattle()
        {
            Console.Clear();
            Console.WriteLine("\n=== The Final Battle ===\n");
            Console.WriteLine("As you collect the second artifact, the ground trembles...");
            Console.WriteLine("The Shadow Dragon appears before you, its dark form blocking out the sun!");
            Console.WriteLine("\nThe dragon roars: 'Foolish mortal! You dare challenge me?'\n");

            while (true)
            {
                Console.WriteLine("\nWhat will you do?");
                Console.WriteLine("1. Use the Ancient Tea Cup");
                Console.WriteLine("2. Use the Sacred Olive Branch");
                Console.WriteLine("3. Try to run away");

                Console.Write("\nYour choice (1-3): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("\nYou raise the Ancient Tea Cup...");
                        Console.WriteLine("Its mystical steam swirls around the dragon,");
                        Console.WriteLine("revealing its true form - a creature of shadow and fear!");
                        Console.WriteLine("\nThe dragon staggers, its invisibility broken!");
                        Console.WriteLine("Now is your chance to use the Olive Branch!");
                        _usedTeaCup = true;
                        WaitForKey();
                        return;

                    case "2":
                        if (!_usedTeaCup)
                        {
                            Console.WriteLine("\nYou wave the Sacred Olive Branch...");
                            Console.WriteLine("But the dragon is too powerful in its shadow form!");
                            Console.WriteLine("Perhaps you should try the Tea Cup first...");
                        }
                        else
                        {
                            DisplayVictoryEnding();
                            return;
                        }
                        break;

                    case "3":
                        DisplayCowardEnding();
                        return;

                    default:
                        Console.WriteLine("\nThat's not a valid choice. Try again!");
                        break;
                }
                WaitForKey();
            }
        }

        private void DisplayVictoryEnding()
        {
            Console.Clear();
            Console.WriteLine("\n=== Victory: The Dragon's Defeat ===\n");
            Console.WriteLine("You raise the Sacred Olive Branch high...");
            Console.WriteLine("Its leaves begin to glow with an ancient power!");
            Console.WriteLine("\nThe dragon roars in defiance, but it's too late.");
            Console.WriteLine("The combined power of both artifacts is too strong!");
            Console.WriteLine("\nWith a final burst of light, the Shadow Dragon is sealed away.");
            Console.WriteLine("Peace returns to the lands of Anatolia once more.");
            Console.WriteLine("\nTHE END - Hero of Anatolia Ending Achieved!");
            ReturnToMainMenu();
        }

        private void DisplayCowardEnding()
        {
            Console.Clear();
            Console.WriteLine("\n=== The Coward's Path ===\n");
            Console.WriteLine("You turn and run from the Shadow Dragon...");
            Console.WriteLine("While you escape with your life, the dragon remains,");
            Console.WriteLine("free to spread darkness across the lands of Anatolia.");
            Console.WriteLine("\nPerhaps another hero will rise to face it.");
            Console.WriteLine("\nTHE END - Coward's Ending Achieved!");
            ReturnToMainMenu();
        }

        private void DisplayHelp()
        {
            Console.WriteLine("\nAvailable Commands:");
            Console.WriteLine("- go <direction>: Move in the specified direction");
            Console.WriteLine("- talk: Talk to an NPC if one is present");
            Console.WriteLine("- help: Display all commands");
            Console.WriteLine("- save: Save your progress");
            Console.WriteLine("- clear: Clear the screen");
            Console.WriteLine("- quit: Exit the game");
            Console.WriteLine("- status: Display your collected artifacts and wisdom points");
            Console.WriteLine("- fight: Confront the apocalypse");
        }

        private void DisplayStatus()
        {
            Console.WriteLine("\n=== Your Status ===");
            Console.WriteLine($"Wisdom Points: {_player.WisdomPoints}");
            Console.WriteLine("\nCollected Artifacts:");
            if (_player.CollectedArtifacts.Count == 0)
            {
                Console.WriteLine("You haven't collected any artifacts yet.");
            }
            else
            {
                foreach (var artifact in _player.CollectedArtifacts)
                {
                    Console.WriteLine($"- {artifact}");
                }
            }
            Console.WriteLine($"\nTotal Artifacts: {_player.CollectedArtifacts.Count}/4");
            Console.WriteLine("\nPress any key to continue...");
        }

        private void Move(string direction)
        {
            if (_player.CurrentLocation == null)
            {
                Console.WriteLine("Error: Current location is invalid.");
                WaitForKey();
                return;
            }

            var nextLocation = _gameMap.GetLocationInDirection(_player.CurrentLocation, direction);
            if (nextLocation != null)
            {
                _player.CurrentLocation = nextLocation;
                Console.WriteLine($"\nYou are heading {direction}...");
                Console.WriteLine($"\n{nextLocation.Description}");
                WaitForKey();
            }
            else
            {
                Console.WriteLine("You cannot go that way.");
                WaitForKey();
            }
        }

        private void SaveGame()
        {
            _saveManager.SaveGame();
        }

        private void WaitForKey()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void Exit()
        {
            Console.Clear();
            Console.WriteLine("\nThanks for visiting Anatolia! Goodbye!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            _isRunning = false;
        }

        private void InitiateApocalypseBattle()
        {
            Console.Clear();
            Console.WriteLine("\n=== The Apocalypse Approaches ===");
            Console.WriteLine("Dark clouds gather over Anatolia. The ancient prophecy speaks of this moment...");
            Console.WriteLine("The time has come to face the darkness that threatens these lands.\n");

            if (!_player.CollectedArtifacts.Contains("Mystical Tea Leaves") ||
                !_player.CollectedArtifacts.Contains("Ancient Scroll of Epics") ||
                !_player.CollectedArtifacts.Contains("Ancient Trident of Poseidon") ||
                !_player.CollectedArtifacts.Contains("Ancient Olive Branch"))
            {
                Console.WriteLine("You stand before the darkness, but you feel unprepared...");
                Console.WriteLine("The ancient artifacts could help you in this battle, but you haven't found them all.");
                Console.WriteLine("\nThe darkness overwhelms you. Anatolia falls into eternal night.");
                Console.WriteLine("\nGame Over - Collect all artifacts to stand a chance against the apocalypse!");
                ReturnToMainMenu();
                return;
            }

            Console.WriteLine("You raise the artifacts you've collected:");
            Console.WriteLine("- The Mystical Tea Leaves glow with the essence of nature's harmony");
            Console.WriteLine("- The Ancient Scroll of Epics resonates with the wisdom of ages");
            Console.WriteLine("- The Ancient Trident of Poseidon pulses with the power of the seas");
            Console.WriteLine("- The Ancient Olive Branch radiates peace and ancient strength");

            Console.WriteLine("\nThe artifacts respond to each other, their powers combining...");
            Console.WriteLine("A brilliant light emerges, pushing back the darkness!");
            Console.WriteLine("\nThe prophecy was true - only by uniting the sacred artifacts of Anatolia");
            Console.WriteLine("could the apocalypse be prevented. The land is saved!");
            Console.WriteLine("\nCongratulations! You have completed your journey and saved Anatolia!");
            
            ReturnToMainMenu();
        }
    }
}
