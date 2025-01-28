using System.Collections.Generic;
using System.Linq;

namespace JourneyThroughAnatolia
{
    public class Player
    {
        public string Name { get; set; } = "";
        public MapLocationData CurrentLocation { get; set; }
        public Dictionary<string, bool> CompletedQuests { get; set; } = new Dictionary<string, bool>();
        public int WisdomPoints { get; set; } = 0;
        public List<string> CollectedArtifacts { get; set; } = new List<string>();

        public Player()
        {
            Name = "Adventurer";
            CurrentLocation = null;
        }

        public bool HasCompletedQuest(string questName)
        {
            return CompletedQuests.ContainsKey(questName) && CompletedQuests[questName];
        }

        public void CompleteQuest(string questName)
        {
            CompletedQuests[questName] = true;
        }

        public void AddArtifact(string artifactName)
        {
            if (!CollectedArtifacts.Contains(artifactName))
            {
                CollectedArtifacts.Add(artifactName);
            }
        }

        public bool HasAllArtifacts()
        {
            string[] requiredArtifacts = {
                "Ancient Tea Cup", // From Karadeniz
                "Sacred Olive Branch" // From Ege Kiyilari
            };

            return requiredArtifacts.All(artifact => CollectedArtifacts.Contains(artifact));
        }
    }
}
