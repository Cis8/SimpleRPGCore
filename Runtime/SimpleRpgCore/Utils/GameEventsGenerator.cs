using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Events
{
    [CreateAssetMenu(fileName = "GameEventGenerator", menuName = "Simple RPG Core/Tools/GameEventGenerator")]
    public sealed class GameEventGenerator : ScriptableObject
    {
        [Serializable]
        public class EventParameter
        {
            public enum NativeType
            {
                @int,
                @long,
                @float,
                @bool
            }

            public enum ParameterType { Native, MonoScript }
            public ParameterType parameterType;
            public NativeType nativeType;
            public MonoScript monoScript; // Used if parameterType == MonoScript
        }

        [Serializable]
        public class GameEventDefinition
        {
            public string eventName;
            [HideInInspector] public string documentation; // Documentation for each event
            public List<EventParameter> parameters = new List<EventParameter>();
            [HideInInspector] public bool isGenerated; // Flag to track if the event has been generated
        }
        
        public string rootNamespace = "ElectricDrill.SimpleRpgCore.Events";
        public List<GameEventDefinition> eventsToGenerate = new List<GameEventDefinition>();
        public string baseSaveLocation = "Assets";

        private List<string> generatedEventNames = new List<string>();
        [SerializeField, HideInInspector] string previousSaveLocation;

        public void GenerateGameEvents()
        {
            string saveLocation = Path.Combine(baseSaveLocation, $"GeneratedEvents/{name}");

            if (!string.IsNullOrEmpty(previousSaveLocation) && previousSaveLocation != saveLocation)
            {
                string oldGeneratedEventsPath = Path.Combine(previousSaveLocation, $"GeneratedEvents/{name}");
                if (Directory.Exists(oldGeneratedEventsPath))
                {
                    try {
                        FileUtil.DeleteFileOrDirectory(oldGeneratedEventsPath);
                        FileUtil.DeleteFileOrDirectory(oldGeneratedEventsPath + ".meta");

                        // if GeneratedEvents folder is empty, delete it
                        if (Directory.GetDirectories(Path.Combine(previousSaveLocation, "GeneratedEvents")).Length == 0)
                        {
                            FileUtil.DeleteFileOrDirectory(Path.Combine(previousSaveLocation, "GeneratedEvents"));
                            FileUtil.DeleteFileOrDirectory(Path.Combine(previousSaveLocation, "GeneratedEvents.meta"));
                        }
                    }
                    catch (Exception e) {
                        Debug.LogError($"Failed to delete old folder at {oldGeneratedEventsPath}: {e.Message}");
                    }
                }
            }

            previousSaveLocation = baseSaveLocation;

            if (!Directory.Exists(saveLocation))
                Directory.CreateDirectory(saveLocation);

            foreach (var gameEvent in eventsToGenerate)
            {
                string parameterCountFolder = gameEvent.parameters.Count.ToString();
                string gameEventsPath = Path.Combine(saveLocation, "GameEvents", parameterCountFolder);
                string listenersPath = Path.Combine(saveLocation, "GameEventListeners", parameterCountFolder);

                if (!Directory.Exists(gameEventsPath))
                    Directory.CreateDirectory(gameEventsPath);

                if (!Directory.Exists(listenersPath))
                    Directory.CreateDirectory(listenersPath);

                generatedEventNames.Clear();

                string className = $"{gameEvent.eventName}GameEvent";
                string filePath = Path.Combine(gameEventsPath, $"{className}.cs");

                List<string> usings = new List<string> { "using UnityEngine;" };
                List<string> parameterTypes = new List<string>();

                foreach (var parameter in gameEvent.parameters)
                {
                    if (parameter.parameterType == EventParameter.ParameterType.Native)
                    {
                        parameterTypes.Add(parameter.nativeType.ToString());
                    }
                    else if (parameter.parameterType == EventParameter.ParameterType.MonoScript && parameter.monoScript != null)
                    {
                        string scriptNamespace = parameter.monoScript.GetClass().Namespace;
                        if (!string.IsNullOrEmpty(scriptNamespace) && scriptNamespace != "ElectricDrill.SimpleRpgCore.Events")
                        {
                            usings.Add($"using {scriptNamespace};");
                        }
                        parameterTypes.Add(parameter.monoScript.GetClass().Name);
                    }
                }

                string genericType = $"GameEventGeneric{parameterTypes.Count}";
                string parameterList = string.Join(", ", parameterTypes);

                string eventScriptContent = $@"{string.Join(Environment.NewLine, usings.Distinct())}

namespace ElectricDrill.SimpleRpgCore.Events
{{
    /// <summary>
    /// {gameEvent.documentation}
    /// </summary>
    [CreateAssetMenu(fileName = ""{gameEvent.eventName} Game Event"", menuName = ""Simple RPG Core/Events/Generated/{gameEvent.eventName}"")]
    public class {className} : {genericType}<{parameterList}>
    {{
    }}
}}";

                File.WriteAllText(filePath, eventScriptContent);
                Debug.Log($"Generated {filePath}");

                // Generate the listener script
                string listenerClassName = $"{gameEvent.eventName}GameEventListener";
                string listenerFilePath = Path.Combine(listenersPath, $"{listenerClassName}.cs");

                string listenerScriptContent = $@"{string.Join(Environment.NewLine, usings.Distinct())}

namespace {rootNamespace}
{{
    /// <summary>
    /// {gameEvent.documentation}
    /// </summary>
    public class {listenerClassName} : GameEventListenerGeneric{parameterTypes.Count}<{parameterList}>
    {{
    }}
}}";

                File.WriteAllText(listenerFilePath, listenerScriptContent);
                Debug.Log($"Generated {listenerFilePath}");

                // Add the new event name to the list
                generatedEventNames.Add(gameEvent.eventName);
                gameEvent.isGenerated = true;
            }

            AssetDatabase.Refresh();
        }

        public void RemoveGeneratedEventFiles(string eventName, int parameterCount)
        {
            string saveLocation = Path.Combine(baseSaveLocation, $"GeneratedEvents/{name}");
            string parameterCountFolder = parameterCount.ToString();
            string gameEventsPath = Path.Combine(saveLocation, "GameEvents", parameterCountFolder);
            string listenersPath = Path.Combine(saveLocation, "GameEventListeners", parameterCountFolder);

            string className = $"{eventName}GameEvent";
            string filePath = Path.Combine(gameEventsPath, $"{className}.cs");
            if (File.Exists(filePath))
            {
                FileUtil.DeleteFileOrDirectory(filePath);
                FileUtil.DeleteFileOrDirectory(filePath + ".meta");
            }

            string listenerClassName = $"{eventName}GameEventListener";
            string listenerFilePath = Path.Combine(listenersPath, $"{listenerClassName}.cs");
            if (File.Exists(listenerFilePath))
            {
                FileUtil.DeleteFileOrDirectory(listenerFilePath);
                FileUtil.DeleteFileOrDirectory(listenerFilePath + ".meta");
            }
        }
        
        private void OnValidate() {
            // if the list of the parameters is long 4 already, we can't add more parameters
            foreach (var gameEvent in eventsToGenerate)
            {
                if (gameEvent.parameters.Count >= 4)
                {
                    gameEvent.parameters.RemoveRange(4, gameEvent.parameters.Count - 4);
                    Debug.LogWarning("The maximum number of parameters is 4");
                }
            }
        }
    }
}