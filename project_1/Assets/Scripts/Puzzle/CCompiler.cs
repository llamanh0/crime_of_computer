// Assets/Scripts/Puzzles/CCompiler.cs
using Debug = UnityEngine.Debug;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using MyGame.Puzzles;

namespace MyGame.Puzzles
{
    /// <summary>
    /// C kodunu derleyip çalıştırır ve sonuçları CodeChecker'a iletir.
    /// </summary>
    public class CCompiler : MonoBehaviour
    {
        [Header("Script References")]
        [SerializeField] private CodeChecker codeChecker; // CodeChecker referansı

        [Header("Puzzle Settings")]
        [SerializeField] private string puzzleId; // Aktif puzzle ID'si

        /// <summary>
        /// Derleyicinin yolunu belirler.
        /// </summary>
        /// <returns>Derleyicinin tam yolu.</returns>
        private string GetCompilerPath()
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

        /// <summary>
        /// Verilen C kodunu derler ve çalıştırır.
        /// </summary>
        /// <param name="code">Derlenecek C kodu.</param>
        public async void CompileAndRun(string code)
        {
            if (string.IsNullOrEmpty(puzzleId))
            {
                Debug.LogError("CCompiler: Puzzle ID'si atanmadı!");
                return;
            }

            Debug.Log("Derleme işlemi başlatılıyor...");

            // Güvenli kod kontrolü
            if (!IsCodeSafe(code))
            {
                Debug.LogError("Güvenli olmayan kod tespit edildi.");
                if (codeChecker != null)
                {
                    codeChecker.DisplayError("Hata: Güvenli olmayan kod tespit edildi.");
                }
                return;
            }

            string compilerPath = GetCompilerPath();
            Debug.Log($"Derleyici Yolu: {compilerPath}");

            if (!File.Exists(compilerPath))
            {
                Debug.LogError($"TinyCC derleyicisi bulunamadı: {compilerPath}");
                if (codeChecker != null)
                {
                    codeChecker.DisplayError("Hata: Derleyici bulunamadı.");
                }
                return;
            }

            // Geçici dosya yolları
            string tempDir = Path.Combine(Application.temporaryCachePath, "CCompiler");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
                Debug.Log($"Geçici dizin oluşturuldu: {tempDir}");
            }

            string sourcePath = Path.Combine(tempDir, "temp_code.c");
            string executablePath = Path.Combine(tempDir, "temp_program");

    #if UNITY_STANDALONE_WIN
            executablePath += ".exe";
    #endif

            // Kullanıcı kodunu template içine ekleme
            string fullCode = CodeTemplate.GetFullCode(puzzleId, code);

            // Kodu geçici dosyaya yazma
            await File.WriteAllTextAsync(sourcePath, fullCode);
            Debug.Log($"C kodu geçici dosyaya yazıldı: {sourcePath}");

            // Derleme işlemini asenkron olarak başlatma
            string includePath = Path.Combine(Application.dataPath, "Plugins/Windows/include");
            string libPath = Path.Combine(Application.dataPath, "Plugins/Windows/lib");
            string compileArguments = $"-I \"{includePath}\" -L \"{libPath}\" -o \"{executablePath}\" \"{sourcePath}\"";

            var compileResult = await RunProcessAsync(compilerPath, compileArguments, Path.Combine(Application.dataPath, "Plugins/Windows/bin"));

            if (compileResult.ExitCode == 0)
            {
                Debug.Log("Derleme başarılı.");
                if (codeChecker != null)
                {
                    codeChecker.DisplayOutput("Derleme başarılı!");
                }
                await RunExecutableAsync(executablePath);
            }
            else
            {
                Debug.LogError($"Derleme hataları: {compileResult.StandardError}");
                if (codeChecker != null)
                {
                    codeChecker.DisplayError($"Derleme Hataları:\n{compileResult.StandardError}");
                }
            }
        }

        /// <summary>
        /// Derlenen programı çalıştırır.
        /// </summary>
        /// <param name="executablePath">Çalıştırılabilir dosyanın yolu.</param>
        private async Task RunExecutableAsync(string executablePath)
        {
            Debug.Log($"Çalıştırılabilir dosya yolu: {executablePath}");

            if (!File.Exists(executablePath))
            {
                Debug.LogError($"Çalıştırılabilir dosya bulunamadı: {executablePath}");
                if (codeChecker != null)
                {
                    codeChecker.DisplayError("Hata: Çalıştırılabilir dosya bulunamadı.");
                }
                return;
            }

            string runArguments = string.Empty;

            var runResult = await RunProcessAsync(executablePath, runArguments, Path.Combine(Application.dataPath, "Plugins/Windows/bin"));

            if (runResult.ExitCode == 0)
            {
                Debug.Log($"Program başarıyla çalıştırıldı. Çıktı: {runResult.StandardOutput}");
                if (codeChecker != null)
                {
                    codeChecker.DisplayOutput(runResult.StandardOutput.Trim());
                    codeChecker.CheckPuzzleOutput(runResult.StandardOutput.Trim());
                }
            }
            else
            {
                Debug.LogError($"Program hatalarla sona erdi: {runResult.StandardError}");
                if (codeChecker != null)
                {
                    codeChecker.DisplayError($"Program Hataları:\n{runResult.StandardError}");
                }
            }
        }

        /// <summary>
        /// Bir süreci asenkron olarak çalıştırır.
        /// </summary>
        /// <param name="fileName">Çalıştırılacak dosya.</param>
        /// <param name="arguments">Argümanlar.</param>
        /// <param name="workingDirectory">Çalışma dizini.</param>
        /// <returns>ProcessResult nesnesi.</returns>
        private Task<ProcessResult> RunProcessAsync(string fileName, string arguments, string workingDirectory)
        {
            return Task.Run(() =>
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

        /// <summary>
        /// Verilen kodun güvenli olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="code">Kontrol edilecek C kodu.</param>
        /// <returns>Güvenli ise true, değilse false.</returns>
        private bool IsCodeSafe(string code)
        {
            // Örnek: sistem çağrılarını engelle
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
