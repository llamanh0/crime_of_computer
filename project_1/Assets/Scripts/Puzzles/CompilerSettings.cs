// Assets/Scripts/Puzzles/CompilerSettings.cs
using UnityEngine;
using System.IO;

namespace MyGame.Puzzles
{
    /// <summary>
    /// TinyCC gibi bir C derleyicisi için ayarlarý tutar.
    /// </summary>
    [CreateAssetMenu(fileName = "CompilerSettings", menuName = "Settings/CompilerSettings", order = 2)]
    public class CompilerSettings : ScriptableObject
    {
        public string GetCompilerPath()
        {
#if UNITY_STANDALONE_WIN
            return Path.Combine(Application.dataPath, "Plugins/Windows/bin/tcc.exe");
#elif UNITY_STANDALONE_OSX
            return Path.Combine(Application.dataPath, "Plugins/macOS/bin/tcc");
#elif UNITY_STANDALONE_LINUX
            return Path.Combine(Application.dataPath, "Plugins/Linux/bin/tcc");
#else
            return string.Empty;
#endif
        }

        public string GetCompileArguments(string sourcePath, string executablePath)
        {
#if UNITY_STANDALONE_WIN
            string includePath = Path.Combine(Application.dataPath, "Plugins/Windows/include");
            string libPath = Path.Combine(Application.dataPath, "Plugins/Windows/lib");
#elif UNITY_STANDALONE_OSX
            string includePath = Path.Combine(Application.dataPath, "Plugins/macOS/include");
            string libPath = Path.Combine(Application.dataPath, "Plugins/macOS/lib");
#elif UNITY_STANDALONE_LINUX
            string includePath = Path.Combine(Application.dataPath, "Plugins/Linux/include");
            string libPath = Path.Combine(Application.dataPath, "Plugins/Linux/lib");
#endif
            return $"-I \"{includePath}\" -L \"{libPath}\" -o \"{executablePath}\" \"{sourcePath}\"";
        }

        public string CompilerWorkingDirectory
        {
            get
            {
#if UNITY_STANDALONE_WIN
                return Path.Combine(Application.dataPath, "Plugins/Windows/bin");
#elif UNITY_STANDALONE_OSX
                return Path.Combine(Application.dataPath, "Plugins/macOS/bin");
#elif UNITY_STANDALONE_LINUX
                return Path.Combine(Application.dataPath, "Plugins/Linux/bin");
#else
                return Application.dataPath;
#endif
            }
        }
    }
}
