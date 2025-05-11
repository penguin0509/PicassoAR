using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine.Networking;
public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI[] textBoxes; // 8 個對話框
    public Button[] nextButtons; // 儲存 8 個"下一段"按鈕
    public float typingSpeed = 0.05f; // 每個字符打字的速度
    public float fadeDuration = 0.5f; // 漸變效果的持續時間
    private string[][] textStages; // 儲存每個對話框的不同階段文本
    private int currentTextBoxIndex = 0; // 當前正在顯示的文本框索引
    private int currentStageIndex = 0; // 當前文本框顯示的階段索引
    private Coroutine typingCoroutine; // 儲存當前文本框的協程
    private bool isTyping = false; // 紀錄當前文本框是否正在進行打字機效果
    private CanvasGroup currentCanvasGroup; // 當前文本框的 CanvasGroup，用於控制漸變
    public GameObject Canvas; // 顯示問題畫布

    private string csvFileName = "text.csv"; // CSV 檔案名稱

    public AudioSource typingSound; // 在編輯器中設置對應的 AudioSource
    public AudioClip typingClip; // 在編輯器中設置音效
    private bool isSoundPlaying = false; // 判斷音效是否已經開始播放

    public bool hasScannedImage = false;  // 用來控制是否已經掃描到圖

    void Start()
    {
        foreach (var button in nextButtons)
        {
            button.interactable = false;
        }
        Canvas.SetActive(false); // 確保畫布一開始是隱藏的
        // 組合路徑為 StreamingAssets 資料夾中的 CSV 檔案
        string fullPath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        // 檢查平台，如果是 Android 或其他需要特別處理的平臺，則使用 UnityWebRequest
        if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(LoadTextStagesFromAndroid(fullPath));
        }
        else
        {
            // 如果是其他平臺，則可以直接讀取文件
            if (File.Exists(fullPath))
            {
                LoadTextStagesFromCSV(fullPath);

            }
            else
            {
                // 顯示警告並在對話框中顯示「找不到文本」
                Debug.LogWarning("CSV file not found: " + fullPath);
                textBoxes[0].text = "找不到文本";
            }
        }
    }

    // 從 CSV 檔案讀取文本
    private void LoadTextStagesFromCSV(string filePath)
    {
        // 初始化 textStages 陣列（假設有 8 個對話框）
        textStages = new string[8][];
        for (int i = 0; i < textStages.Length; i++)
        {
            textStages[i] = new string[] { }; // 每個對話框初始化為空
        }

        // 讀取 CSV 檔案
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines.Skip(1)) // 跳過標題行
        {
            var columns = line.Split(',');
            if (columns.Length >= 3)
            {
                int textBoxIndex = int.Parse(columns[0]);
                int stageIndex = int.Parse(columns[1]);
                string text = columns[2];

                // 根據對話框索引與階段索引，將文本存入對應位置
                if (textStages[textBoxIndex] == null)
                {
                    textStages[textBoxIndex] = new string[] { };
                }

                textStages[textBoxIndex] = textStages[textBoxIndex].Concat(new string[] { text }).ToArray();
            }
        }
    }

    // 從 Android 平臺的 StreamingAssets 資料夾讀取 CSV 文件
    private IEnumerator LoadTextStagesFromAndroid(string filePath)
    {
        // 使用 UnityWebRequest 讀取 StreamingAssets 中的文件
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string[] lines = www.downloadHandler.text.Split('\n');
            textStages = new string[8][];
            for (int i = 0; i < textStages.Length; i++)
            {
                textStages[i] = new string[] { }; // 每個對話框初始化為空
            }

            foreach (var line in lines.Skip(1)) // 跳過標題行
            {
                var columns = line.Split(',');
                if (columns.Length >= 3)
                {
                    int textBoxIndex = int.Parse(columns[0]);
                    int stageIndex = int.Parse(columns[1]);
                    string text = columns[2];

                    // 根據對話框索引與階段索引，將文本存入對應位置
                    if (textStages[textBoxIndex] == null)
                    {
                        textStages[textBoxIndex] = new string[] { };
                    }

                    textStages[textBoxIndex] = textStages[textBoxIndex].Concat(new string[] { text }).ToArray();
                }
            }
        }
        else
        {
            Debug.LogWarning("Failed to load CSV file from Android: " + www.error);
            textBoxes[0].text = "找不到文本";
            foreach (var button in nextButtons)
            {
                button.interactable = false; // 禁用所有按鈕
            }
        }
    }
    // 儲存文本內容到 CSV 檔案
    private void SaveTextStagesToCSV()
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        using (var writer = new StreamWriter(fullPath))
        {
            // 寫入標題
            writer.WriteLine("TextBoxIndex,StageIndex,Text");

            // 遍歷所有對話框和階段，寫入 CSV
            for (int i = 0; i < textStages.Length; i++)
            {
                if (textStages[i] != null)
                {
                    for (int j = 0; j < textStages[i].Length; j++)
                    {
                        writer.WriteLine($"{i},{j},{textStages[i][j]}");
                    }
                }
            }
        }
    }

    // 開始顯示打字機效果
    public void StartTypingEffect()
    {
        // 如果有更多的階段文本，開始顯示第一個文本框的第一個階段
        if (currentTextBoxIndex < textBoxes.Length && textStages[currentTextBoxIndex].Length > 0)
        {
            // 確保當前文本框有 CanvasGroup 用於漸變
            currentCanvasGroup = textBoxes[currentTextBoxIndex].gameObject.GetComponent<CanvasGroup>();
            typingCoroutine = StartCoroutine(TypingEffect(textBoxes[currentTextBoxIndex], textStages[currentTextBoxIndex][currentStageIndex]));
        }
    }

    // 按鈕觸發，開始顯示下一段文字
    public void OnNextButtonClicked()
    {
        // 只在當前文本框的打字機效果完成後，才能顯示下一段文字
        if (!isTyping)
        {
            // 顯示下一個階段的文字
            if (currentStageIndex + 1 < textStages[currentTextBoxIndex].Length)
            {
                // 如果該文本框還有下一個階段，顯示下一段文字
                currentStageIndex++;
                typingCoroutine = StartCoroutine(TypingEffect(textBoxes[currentTextBoxIndex], textStages[currentTextBoxIndex][currentStageIndex]));
            }
            else
            {
                // 如果該文本框的所有階段顯示完成，開始顯示下一個文本框
                currentTextBoxIndex++;

                // 如果還有文本框，開始顯示下一個文本框的第一個階段
                if (currentTextBoxIndex < textBoxes.Length && textStages[currentTextBoxIndex].Length > 0)
                {
                    currentStageIndex = 0;
                    typingCoroutine = StartCoroutine(TypingEffect(textBoxes[currentTextBoxIndex], textStages[currentTextBoxIndex][currentStageIndex]));
                }

                // 如果所有文本框顯示完畢，禁用按鈕
                if (currentTextBoxIndex >= textBoxes.Length)
                {
                    nextButtons[currentTextBoxIndex].interactable = false;
                    // 在此處儲存更改到 CSV 文件
                    SaveTextStagesToCSV();
                }
            }
        }
    }

    // 打字機效果顯示文字
    private IEnumerator TypingEffect(TextMeshProUGUI textBox, string text)
    {
        isTyping = true; // 開始打字機效果

        textBox.text = ""; // 清空文本框
        typingSound.loop = true; // 設定音效循環播放
        typingSound.Play(); // 播放音效
        isSoundPlaying = true;
        foreach (char letter in text)
        {
            textBox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingSound.Stop(); // 停止音效
        isSoundPlaying = false;

        isTyping = false; // 完成打字機效果

        // 完成打字機效果後啟用漸變效果
        if (currentCanvasGroup != null)
        {
            StartCoroutine(FadeIn(currentCanvasGroup)); // 漸變顯示
        }

        // 檢查是否可以啟用按鈕
        if (currentTextBoxIndex < textBoxes.Length && currentStageIndex + 1 < textStages[currentTextBoxIndex].Length)
        {
            nextButtons[currentTextBoxIndex].interactable = true; // 啟用按鈕，準備顯示下一段文字
        }
        else if (currentTextBoxIndex + 1 < textBoxes.Length)
        {
            nextButtons[currentTextBoxIndex].interactable = true; // 啟用按鈕，準備顯示下一個文本框
        }
    }

    // 漸變顯示
    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    // 停止所有協程並重置文本顯示
    public void RestartTypingEffect()
    {
        // 停止當前正在進行的打字機效果
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // 清空所有文本框中的文字
        foreach (var textBox in textBoxes)
        {
            textBox.text = "";
        }

        // 重置所有參數
        currentTextBoxIndex = 0;
        currentStageIndex = 0;

        // 重新開始打字機效果
        StartTypingEffect();
    }
}
