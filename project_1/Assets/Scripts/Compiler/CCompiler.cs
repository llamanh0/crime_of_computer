using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class CCompiler : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private CodeChecker codeChecker; // CodeChecker referansı
    [SerializeField] private SingleLineOutput singleLineOutput; // SingleLineOutput referansı

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
    public void CompileAndRun(string code)
    {
        Debug.Log("Derleme işlemi başlatılıyor...");

        // Güvenli kod kontrolü
        if (!IsCodeSafe(code))
        {
            Debug.LogError("Güvenli olmayan kod tespit edildi.");
            if (codeChecker != null)
            {
                codeChecker.DisplayError("Güvenli olmayan kod tespit edildi.");
            }
            return;
        }

        string compilerPath = GetCompilerPath();
        Debug.Log("Derleyici Yolu: " + compilerPath);

        if (!File.Exists(compilerPath))
        {
            Debug.LogError("TinyCC derleyicisi bulunamadı: " + compilerPath);
            if (codeChecker != null)
            {
                codeChecker.DisplayError("Derleyici bulunamadı.");
            }
            return;
        }

        // Geçici dosya yolları
        string tempDir = Path.Combine(Application.temporaryCachePath, "CCompiler");
        if (!Directory.Exists(tempDir))
        {
            Directory.CreateDirectory(tempDir);
            Debug.Log("Geçici dizin oluşturuldu: " + tempDir);
        }

        string sourcePath = Path.Combine(tempDir, "temp_code.c");
        string executablePath = Path.Combine(tempDir, "temp_program");

        // Windows için .exe uzantısı ekleyin
        #if UNITY_STANDALONE_WIN
            executablePath += ".exe";
        #endif

        // Kodu geçici dosyaya yazma
        File.WriteAllText(sourcePath, code);
        Debug.Log("C kodu geçici dosyaya yazıldı: " + sourcePath);

        // Derleme komutu
        ProcessStartInfo compileInfo = new ProcessStartInfo();
        compileInfo.FileName = compilerPath;
        compileInfo.Arguments = $"-I \"{Path.Combine(Application.dataPath, "Plugins/Windows/include")}\" -L \"{Path.Combine(Application.dataPath, "Plugins/Windows/lib")}\" -o \"{executablePath}\" \"{sourcePath}\"";
        compileInfo.RedirectStandardOutput = true;
        compileInfo.RedirectStandardError = true;
        compileInfo.UseShellExecute = false;
        compileInfo.CreateNoWindow = true;

        // PATH'e Plugins/Windows/bin dizinini ekleyerek DLL'lerin bulunmasını sağlama
        string pluginsWindowsBinPath = Path.Combine(Application.dataPath, "Plugins/Windows/bin");
        string existingPath = Environment.GetEnvironmentVariable("PATH");
        compileInfo.Environment["PATH"] = pluginsWindowsBinPath + ";" + existingPath;

        Process compileProcess = new Process();
        compileProcess.StartInfo = compileInfo;

        Debug.Log("Derleyici çalıştırılıyor: " + compileInfo.FileName + " " + compileInfo.Arguments);

        try
        {
            compileProcess.Start();
        }
        catch (Exception e)
        {
            Debug.LogError("Derleyici başlatılamadı: " + e.Message);
            if (codeChecker != null)
            {
                codeChecker.DisplayError("Derleyici başlatılamadı: " + e.Message);
            }
            return;
        }

        // Çıktıyı oku
        string compileOutput = compileProcess.StandardOutput.ReadToEnd();
        string compileErrors = compileProcess.StandardError.ReadToEnd();
        compileProcess.WaitForExit();

        Debug.Log("Derleme süreci tamamlandı. Çıkış Kodu: " + compileProcess.ExitCode);
        Debug.Log("Derleme Çıktısı: " + compileOutput);
        if (!string.IsNullOrEmpty(compileErrors))
        {
            Debug.LogError("Derleme Hataları: " + compileErrors);
        }

        if (compileProcess.ExitCode == 0)
        {
            Debug.Log("Derleme başarılı.");
            if (codeChecker != null)
            {
                codeChecker.DisplayOutput("Derleme başarılı!");
            }
            RunExecutable(executablePath);
        }
        else
        {
            Debug.LogError("Derleme hataları: " + compileErrors);
            if (codeChecker != null)
            {
                codeChecker.DisplayError(compileErrors);
            }
        }
    }

    /// <summary>
    /// Derlenen programı çalıştırır.
    /// </summary>
    /// <param name="executablePath">Çalıştırılabilir dosyanın yolu.</param>
    private void RunExecutable(string executablePath)
    {
        Debug.Log("Çalıştırılabilir dosya yolu: " + executablePath);

        // Çalıştırılabilir dosyanın varlığını kontrol edin
        if (!File.Exists(executablePath))
        {
            Debug.LogError("Çalıştırılabilir dosya bulunamadı: " + executablePath);
            if (codeChecker != null)
            {
                codeChecker.DisplayError("Çalıştırılabilir dosya bulunamadı.");
            }
            return;
        }

        // Çalıştırma komutu
        ProcessStartInfo runInfo = new ProcessStartInfo();
        runInfo.FileName = executablePath;
        runInfo.RedirectStandardOutput = true;
        runInfo.RedirectStandardError = true;
        runInfo.UseShellExecute = false;
        runInfo.CreateNoWindow = true;

        // PATH'e Plugins/Windows/bin dizinini ekleyerek DLL'lerin bulunmasını sağlama
        string pluginsWindowsBinPath = Path.Combine(Application.dataPath, "Plugins/Windows/bin");
        string existingPathRun = Environment.GetEnvironmentVariable("PATH");
        runInfo.Environment["PATH"] = pluginsWindowsBinPath + ";" + existingPathRun;

        Process runProcess = new Process();
        runProcess.StartInfo = runInfo;

        Debug.Log("Program çalıştırılıyor: " + runInfo.FileName);

        try
        {
            runProcess.Start();
        }
        catch (Exception e)
        {
            Debug.LogError("Çalıştırma işlemi başlatılamadı: " + e.Message);
            if (codeChecker != null)
            {
                codeChecker.DisplayError("Çalıştırma işlemi başlatılamadı: " + e.Message);
            }
            return;
        }

        // Çıktıyı oku
        string runOutput = runProcess.StandardOutput.ReadToEnd();
        string runErrors = runProcess.StandardError.ReadToEnd();
        runProcess.WaitForExit();

        Debug.Log("Çalıştırma süreci tamamlandı. Çıkış Kodu: " + runProcess.ExitCode);
        Debug.Log("Program Çıktısı: " + runOutput);
        if (!string.IsNullOrEmpty(runErrors))
        {
            Debug.LogError("Program Hataları: " + runErrors);
        }

        if (runProcess.ExitCode == 0)
        {
            Debug.Log("Program başarıyla çalıştırıldı.");
            if (singleLineOutput != null)
            {
                singleLineOutput.DisplayOutput(runOutput.Trim());
            }
            if (codeChecker != null)
            {
                // Puzzle'ın çözülüp çözülmediğini kontrol et
                codeChecker.CheckPuzzleOutput(runOutput.Trim());
            }
        }
        else
        {
            Debug.LogError("Program hatalarla sona erdi: " + runErrors);
            if (codeChecker != null)
            {
                codeChecker.DisplayError(runErrors);
            }
        }
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
}
