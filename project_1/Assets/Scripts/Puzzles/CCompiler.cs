// Assets/Scripts/Puzzles/CCompiler.cs
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MyGame.Puzzles;
using MyGame.Managers;
using MyGame.UI;
using Debug = UnityEngine.Debug;

namespace MyGame.Puzzles
{
    /// <summary>
    /// C kodunu derler ve çalıştırır, ardından sonuçları CodeChecker'a iletir.
    /// </summary>
    public class CCompiler : MonoBehaviour
    {
        [Header("Script References")]
        [SerializeField] private CodeChecker codeChecker;

        [Header("Puzzle Settings")]
        [SerializeField] private PuzzleData currentPuzzle; // Şu anki bulmaca verisi

        [Header("Compiler Settings")]
        [SerializeField] private CompilerSettings compilerSettings;

        private void Awake()
        {
            if (codeChecker == null)
            {
                Debug.LogError("CCompiler: CodeChecker referansı atanmamış!");
            }

            if (currentPuzzle == null)
            {
                Debug.LogError("CCompiler: currentPuzzle referansı atanmamış!");
            }
        }

        /// <summary>
        /// Kullanıcının yazdığı C kodunu derler ve çalıştırır.
        /// </summary>
        public async void CompileAndRun(string userCode)
        {
            if (codeChecker == null || currentPuzzle == null)
            {
                Debug.LogError("CCompiler: Eksik referanslar veya bulmaca verisi.");
                return;
            }

            Debug.Log("Derleme işlemi başlatılıyor...");

            if (!IsCodeSafe(userCode))
            {
                Debug.LogError("CCompiler: Güvenli olmayan kod tespit edildi.");
                codeChecker.DisplayError("Hata: Güvenli olmayan kod tespit edildi.");
                return;
            }

            string compilerPath = compilerSettings.GetCompilerPath();
            Debug.Log($"Derleyici Yolu: {compilerPath}");

            if (!File.Exists(compilerPath))
            {
                Debug.LogError($"CCompiler: TinyCC derleyicisi bulunamadı: {compilerPath}");
                codeChecker.DisplayError("Hata: Derleyici bulunamadı.");
                return;
            }

            string tempDir = Path.Combine(Application.temporaryCachePath, "CCompiler");
            Directory.CreateDirectory(tempDir);

            string sourcePath = Path.Combine(tempDir, "temp_code.c");
            string executablePath = Path.Combine(tempDir, "temp_program");

#if UNITY_STANDALONE_WIN
            executablePath += ".exe";
#endif

            string fullCode = CodeTemplate.GetFullCode(currentPuzzle.puzzleID, userCode);

            await File.WriteAllTextAsync(sourcePath, fullCode);
            Debug.Log($"C kodu geçici dosyaya yazıldı: {sourcePath}");

            string compileArguments = compilerSettings.GetCompileArguments(sourcePath, executablePath);

            var compileResult = await RunProcessAsync(compilerPath, compileArguments, compilerSettings.CompilerWorkingDirectory);

            if (compileResult.ExitCode == 0)
            {
                Debug.Log("Derleme başarılı.");
                codeChecker.DisplayOutput(currentPuzzle.successMessage);
                await RunExecutableAsync(executablePath);
            }
            else
            {
                Debug.LogError($"Derleme hataları: {compileResult.StandardError}");
                codeChecker.DisplayError($"Derleme Hataları:\n{compileResult.StandardError}");
            }
        }

        private async Task RunExecutableAsync(string executablePath)
        {
            Debug.Log($"Çalıştırılabilir dosya yolu: {executablePath}");

            if (!File.Exists(executablePath))
            {
                Debug.LogError($"CCompiler: Çalıştırılabilir dosya bulunamadı: {executablePath}");
                codeChecker.DisplayError("Hata: Çalıştırılabilir dosya bulunamadı.");
                return;
            }

            var runResult = await RunProcessAsync(executablePath, string.Empty, Path.GetDirectoryName(executablePath));

            if (runResult.ExitCode == 0)
            {
                Debug.Log($"Program başarıyla çalıştırıldı. Çıktı: {runResult.StandardOutput}");
                codeChecker.DisplayOutput(runResult.StandardOutput.Trim());
                codeChecker.CheckPuzzleOutput(runResult.StandardOutput.Trim());
            }
            else
            {
                Debug.LogError($"Program hatalarla sona erdi: {runResult.StandardError}");
                codeChecker.DisplayError($"Program Hataları:\n{runResult.StandardError}");
            }
        }

        private async Task<ProcessResult> RunProcessAsync(string fileName, string arguments, string workingDirectory)
        {
            return await Task.Run(() =>
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    string stdout = process.StandardOutput.ReadToEnd();
                    string stderr = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    return new ProcessResult
                    {
                        ExitCode = process.ExitCode,
                        StandardOutput = stdout,
                        StandardError = stderr
                    };
                }
            });
        }

        private bool IsCodeSafe(string code)
        {
            string[] forbiddenFunctions = { "system", "exec", "fork", "kill" };
            foreach (var func in forbiddenFunctions)
            {
                if (code.Contains(func + "("))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Süreç sonuçlarını tutar.
        /// </summary>
        private class ProcessResult
        {
            public int ExitCode { get; set; }
            public string StandardOutput { get; set; }
            public string StandardError { get; set; }
        }
    }
}
