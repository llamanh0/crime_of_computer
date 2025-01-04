// Assets/Scripts/Puzzles/CCompiler.cs
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MyGame.UI;
using Debug = UnityEngine.Debug;

namespace MyGame.Puzzles
{
    /// <summary>
    /// C kodunu derler ve çalıştırır, ardından sonuçları ICodeChecker'a iletir.
    /// </summary>
    public class CCompiler : MonoBehaviour
    {
        [Header("Checker (Interface Reference)")]
        [SerializeField] private MonoBehaviour codeCheckerObject;
        private ICodeChecker codeChecker;
        [Header("Puzzle Settings")]
        [SerializeField] private PuzzleData currentPuzzle;
        [SerializeField] private CompilerSettings compilerSettings;

        private void Awake()
        {
            codeChecker = codeCheckerObject as ICodeChecker;
            if (codeChecker == null)
            {
                Debug.LogError("CCompiler: codeCheckerObject bir ICodeChecker değil!");
            }

            if (currentPuzzle == null)
            {
                Debug.LogError("CCompiler: currentPuzzle referansı atanmamış!");
            }

            if (compilerSettings == null)
            {
                Debug.LogError("CCompiler: compilerSettings referansı atanmamış!");
            }
        }

        public async void CompileAndRun(string userCode, PuzzleData puzzleData)
        {
            if (codeChecker == null || currentPuzzle == null || compilerSettings == null)
            {
                Debug.LogError("CCompiler: Eksik referanslar!");
                return;
            }

            Debug.Log("Derleme işlemi başlatılıyor...");

            if (!IsCodeSafe(userCode))
            {
                codeChecker.DisplayError("Hata: Güvenli olmayan kod tespit edildi.");
                return;
            }

            string compilerPath = compilerSettings.GetCompilerPath();
            if (!File.Exists(compilerPath))
            {
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

            string fullCode = puzzleData != null ? puzzleData.GetFullCode(userCode) : userCode;
            //string fullCode = CodeTemplate.GetFullCode(currentPuzzle.puzzleID, userCode);

            await File.WriteAllTextAsync(sourcePath, fullCode);
            Debug.Log($"C kodu geçici dosyaya yazıldı: {sourcePath}");

            string compileArguments = compilerSettings.GetCompileArguments(sourcePath, executablePath);
            var compileResult = await RunProcessAsync(compilerPath, compileArguments, compilerSettings.CompilerWorkingDirectory);

            if (compileResult.ExitCode == 0)
            {
                // Derleme başarılı
                codeChecker.DisplayOutput(currentPuzzle.successMessage);
                await RunExecutableAsync(executablePath);
            }
            else
            {
                // Derleme hatası
                codeChecker.DisplayError($"Derleme Hataları:\n{compileResult.StandardError}");
            }
        }

        private async Task RunExecutableAsync(string executablePath)
        {
            if (!File.Exists(executablePath))
            {
                codeChecker.DisplayError("Hata: Çalıştırılabilir dosya bulunamadı.");
                return;
            }

            var runResult = await RunProcessAsync(executablePath, string.Empty, Path.GetDirectoryName(executablePath));

            if (runResult.ExitCode == 0)
            {
                // Program çıktısı
                string trimmedOutput = runResult.StandardOutput.Trim();
                codeChecker.DisplayOutput(trimmedOutput);
                codeChecker.CheckPuzzleOutput(trimmedOutput);
            }
            else
            {
                // Program hataları
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

        // Basit bir sınıf, süreç sonuçlarını saklamak için
        private class ProcessResult
        {
            public int ExitCode;
            public string StandardOutput;
            public string StandardError;
        }
    }
}
