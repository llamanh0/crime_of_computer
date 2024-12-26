using System; // Environment sınıfını kullanmak için ekledik
using System.Diagnostics;
using System.IO;
using UnityEngine;

/*
public class CCompiler : MonoBehaviour
{
    [SerializeField] private CodePanelManager codePanelManager;
    [SerializeField] private PuzzleManager puzzleManager;


    private string GetCompilerPath()
    {
        #if UNITY_STANDALONE_WIN
            return Path.Combine(Application.dataPath, "Plugins/Windows/tcc.exe");
        #elif UNITY_STANDALONE_OSX
            return Path.Combine(Application.dataPath, "Plugins/macOS/tcc");
        #elif UNITY_STANDALONE_LINUX
            return Path.Combine(Application.dataPath, "Plugins/Linux/tcc");
        #else
            return string.Empty;
        #endif
    }

    public void CompileAndRun(string code)
    {
        UnityEngine.Debug.Log("Derleme işlemi başlatılıyor...");

        // Güvenli kod kontrolü
        if (!IsCodeSafe(code))
        {
            UnityEngine.Debug.LogError("Güvenli olmayan kod tespit edildi.");
            if (codePanelManager != null)
            {
                codePanelManager.DisplayError("Güvenli olmayan kod tespit edildi.");
            }
            return;
        }

        string compilerPath = GetCompilerPath();
        UnityEngine.Debug.Log("Derleyici Yolu: " + compilerPath);

        if (!File.Exists(compilerPath))
        {
            UnityEngine.Debug.LogError("TinyCC derleyicisi bulunamadı: " + compilerPath);
            if (codePanelManager != null)
            {
                codePanelManager.DisplayError("Derleyici bulunamadı.");
            }
            return;
        }

        // Geçici dosya yolları
        string tempDir = Path.Combine(Application.temporaryCachePath, "CCompiler");
        if (!Directory.Exists(tempDir))
        {
            Directory.CreateDirectory(tempDir);
            UnityEngine.Debug.Log("Geçici dizin oluşturuldu: " + tempDir);
        }

        string sourcePath = Path.Combine(tempDir, "temp_code.c");
        string executablePath = Path.Combine(tempDir, "temp_program");

        // Windows için .exe uzantısı ekleyin
        #if UNITY_STANDALONE_WIN
            executablePath += ".exe";
        #endif

        // Kodu geçici dosyaya yazma
        File.WriteAllText(sourcePath, code);
        UnityEngine.Debug.Log("C kodu geçici dosyaya yazıldı: " + sourcePath);

        // Derleme komutu
        ProcessStartInfo compileInfo = new ProcessStartInfo();
        compileInfo.FileName = compilerPath;
        compileInfo.Arguments = $"-I \"{Path.Combine(Application.dataPath, "Plugins/Windows/include")}\" -L \"{Path.Combine(Application.dataPath, "Plugins/Windows/lib")}\" -o \"{executablePath}\" \"{sourcePath}\"";
        compileInfo.RedirectStandardOutput = true;
        compileInfo.RedirectStandardError = true;
        compileInfo.UseShellExecute = false;
        compileInfo.CreateNoWindow = true;

        // PATH'e Plugins/Windows dizinini ekleyerek DLL'lerin bulunmasını sağlama
        string pluginsWindowsPath = Path.Combine(Application.dataPath, "Plugins/Windows");
        string existingPath = Environment.GetEnvironmentVariable("PATH");
        compileInfo.Environment["PATH"] = pluginsWindowsPath + ";" + existingPath;

        Process compileProcess = new Process();
        compileProcess.StartInfo = compileInfo;

        UnityEngine.Debug.Log("Derleyici çalıştırılıyor: " + compileInfo.FileName + " " + compileInfo.Arguments);

        try
        {
            compileProcess.Start();
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Derleyici başlatılamadı: " + e.Message);
            if (codePanelManager != null)
            {
                codePanelManager.DisplayError("Derleyici başlatılamadı: " + e.Message);
            }
            return;
        }

        // Çıktıyı oku
        string compileOutput = compileProcess.StandardOutput.ReadToEnd();
        string compileErrors = compileProcess.StandardError.ReadToEnd();
        compileProcess.WaitForExit();

        UnityEngine.Debug.Log("Derleme süreci tamamlandı. Çıkış Kodu: " + compileProcess.ExitCode);
        UnityEngine.Debug.Log("Derleme Çıktısı: " + compileOutput);
        if (!string.IsNullOrEmpty(compileErrors))
        {
            UnityEngine.Debug.LogError("Derleme Hataları: " + compileErrors);
        }

        if (compileProcess.ExitCode == 0)
        {
            UnityEngine.Debug.Log("Derleme başarılı.");
            if (codePanelManager != null)
            {
                codePanelManager.DisplayOutput("Derleme başarılı!");
            }
            RunExecutable(executablePath);
        }
        else
        {
            UnityEngine.Debug.LogError("Derleme hataları: " + compileErrors);
            if (codePanelManager != null)
            {
                codePanelManager.DisplayError(compileErrors);
            }
        }
    }

    private void RunExecutable(string executablePath)
    {
        UnityEngine.Debug.Log("Çalıştırılabilir dosya yolu: " + executablePath);

        // Çalıştırılabilir dosyanın varlığını kontrol edin
        if (!File.Exists(executablePath))
        {
            UnityEngine.Debug.LogError("Çalıştırılabilir dosya bulunamadı: " + executablePath);
            if (codePanelManager != null)
            {
                codePanelManager.DisplayError("Çalıştırılabilir dosya bulunamadı.");
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

        // PATH'e Plugins/Windows dizinini ekleyerek DLL'lerin bulunmasını sağlama
        string pluginsWindowsPath = Path.Combine(Application.dataPath, "Plugins/Windows");
        string existingPath = Environment.GetEnvironmentVariable("PATH");
        runInfo.Environment["PATH"] = pluginsWindowsPath + ";" + existingPath;

        Process runProcess = new Process();
        runProcess.StartInfo = runInfo;

        UnityEngine.Debug.Log("Program çalıştırılıyor: " + runInfo.FileName);

        try
        {
            runProcess.Start();
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Çalıştırma işlemi başlatılamadı: " + e.Message);
            if (codePanelManager != null)
            {
                codePanelManager.DisplayError("Çalıştırma işlemi başlatılamadı: " + e.Message);
            }
            return;
        }

        // Çıktıyı oku
        string runOutput = runProcess.StandardOutput.ReadToEnd();
        string runErrors = runProcess.StandardError.ReadToEnd();
        runProcess.WaitForExit();

        UnityEngine.Debug.Log("Çalıştırma süreci tamamlandı. Çıkış Kodu: " + runProcess.ExitCode);
        UnityEngine.Debug.Log("Program Çıktısı: " + runOutput);
        if (!string.IsNullOrEmpty(runErrors))
        {
            UnityEngine.Debug.LogError("Program Hataları: " + runErrors);
        }

        if (runProcess.ExitCode == 0)
        {
            UnityEngine.Debug.Log("Program başarıyla çalıştırıldı.");
            if (codePanelManager != null)
            {
                codePanelManager.DisplayOutput(runOutput);
            }
            // PuzzleManager'a başarılı çıktıyı bildir
            if(puzzleManager != null)
            {
                puzzleManager.OnCompilationFinished(true, runOutput);
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Program hatalarla sona erdi: " + runErrors);
            if (codePanelManager != null)
            {
                codePanelManager.DisplayError(runErrors);
            }
            // PuzzleManager'a program hatasını bildir
            if(puzzleManager != null)
            {
                puzzleManager.OnCompilationFinished(false, runErrors);
            }
        }

        if (compileProcess.ExitCode == 0)
        {
            UnityEngine.Debug.Log("Derleme başarılı.");
            if (codePanelManager != null)
            {
                codePanelManager.DisplayOutput("Derleme başarılı!");
            }
            RunExecutable(executablePath);
        }
        else
        {
            UnityEngine.Debug.LogError("Derleme hataları: " + compileErrors);
            if (codePanelManager != null)
            {
                codePanelManager.DisplayError(compileErrors);
            }
            // PuzzleManager'a derleme hatasını bildir
            if(puzzleManager != null)
            {
                puzzleManager.OnCompilationFinished(false, compileErrors);
            }
        }
    }

    // Güvenli kod kontrolü için basit bir yöntem
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
*/