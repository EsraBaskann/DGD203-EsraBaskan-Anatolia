using System.Collections.Generic;

namespace JourneyThroughAnatolia
{
    public class NPC
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> DialogueOptions { get; private set; }
        public Dictionary<string, string> Responses { get; private set; }
        public string SecretObject { get; private set; }
        public bool HasGivenSecretObject { get; private set; }

        public NPC(string name, string description, string secretObject = null)
        {
            Name = name;
            Description = description;
            DialogueOptions = new List<string>();
            Responses = new Dictionary<string, string>();
            SecretObject = secretObject;
            HasGivenSecretObject = false;
        }

        public void AddDialogueOption(string option, string response)
        {
            DialogueOptions.Add(option);
            Responses[option] = response;
        }

        public (string response, string secretObject) GetResponse(string option)
        {
            if (option.ToLower() == "secret object" && SecretObject != null && !HasGivenSecretObject)
            {
                HasGivenSecretObject = true;
                return ($"Here, take this {SecretObject}. Use it wisely.", SecretObject);
            }
            
            return (Responses.ContainsKey(option) ? Responses[option] : "I don't have anything to say about that.", null);
        }
    }
}
