using System.Json;

namespace RimWorldTV {

    public delegate void EffectCommandHandler(object sender, EffectCommand command);

    public class EffectCommand {

        // Required
        public int? id = null;
        public string code = null;
        public int? type = null;
        public string viewerName = null;

        // Optional
        public JsonArray targets = null;
        public JsonArray parameters = null;

        public bool IsValid { 
            get { 
                return (id != null) && (code != null) && (type != null) && (viewerName != null); 
            } 
        }

        private string rawMessage = string.Empty;

        public EffectCommand(string message) {
            rawMessage = message;
        }

        public void TryParse() {
            JsonValue jsonMessage = JsonValue.Parse(rawMessage.Replace("\0", ""));

            if (jsonMessage.ContainsKey("id"))
                id = jsonMessage["id"];

            if (jsonMessage.ContainsKey("code"))
                code = jsonMessage["code"];

            if (jsonMessage.ContainsKey("type"))
                type = jsonMessage["type"];

            if (jsonMessage.ContainsKey("viewer"))
                viewerName = jsonMessage["viewer"];

            if (jsonMessage.ContainsKey("targets"))
                targets = jsonMessage["targets"] as JsonArray;

            if (jsonMessage.ContainsKey("parameters"))
                targets = jsonMessage["parameters"] as JsonArray;
        }

        public override string ToString() { 
            return rawMessage; 
        }
    
    }
}