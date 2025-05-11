using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using UnityEngine.Video;  // 加入 VideoPlayer 的命名空間
public class MultipleImageTargetUIManager : MonoBehaviour
{
    public GameObject[] uiPanels;       // 用來顯示 UI 的面板，對應每個畫作
    public GameObject[] videoPanels;    // 用來顯示影片的面板，對應每個畫作
    public GameObject[] npcImages;      // 用來顯示 NPC 圖片的物件，對應每個畫作
    private VideoPlayer[] videoPlayers; // 用來控制每個影片的播放

    private bool[] imageTargetsFound;   // 用來追蹤哪些畫作已經掃描過
    private int currentTargetIndex = -1; // 當前顯示的畫作索引

    void Start()
    {
        // 初始狀態隱藏所有 UI、影片和 NPC 圖片
        HideAll();
        imageTargetsFound = new bool[uiPanels.Length];

        // 初始化 VideoPlayer 陣列
        videoPlayers = new VideoPlayer[videoPanels.Length];
        for (int i = 0; i < videoPanels.Length; i++)
        {
            // 確保每個 videoPanel 內都有 VideoPlayer 組件
            videoPlayers[i] = videoPanels[i].GetComponent<VideoPlayer>();
        }
    }

    // 隱藏所有的 UI、影片和 NPC 圖片
    private void HideAll()
    {
        foreach (var panel in uiPanels)
            panel.SetActive(false);
        foreach (var videoPanel in videoPanels)
            videoPanel.SetActive(false);
        foreach (var npcImage in npcImages)
            npcImage.SetActive(false);
    }

    // 當掃描到 ImageTarget 時顯示對應的 UI 和影片
    public void OnImageTargetFound(int targetIndex)
    {
        // 如果這個畫作還沒被掃描過，顯示其 UI 和影片
        if (!imageTargetsFound[targetIndex])
        {
            // 隱藏上一次顯示的 UI 和影片
            if (currentTargetIndex != -1)
            {
                uiPanels[currentTargetIndex].SetActive(false);
                videoPanels[currentTargetIndex].SetActive(false);
                npcImages[currentTargetIndex].SetActive(false);

                // 暫停上一次的影片
                if (videoPlayers[currentTargetIndex].isPlaying)
                {
                    videoPlayers[currentTargetIndex].Pause();
                }
            }

            // 顯示新的畫作 UI 和影片
            uiPanels[targetIndex].SetActive(true);
            videoPanels[targetIndex].SetActive(true);
            npcImages[targetIndex].SetActive(true);

            videoPlayers[targetIndex].time = 0;
            videoPlayers[targetIndex].Play();

            // 記錄當前畫作為已顯示
            imageTargetsFound[targetIndex] = true;
            currentTargetIndex = targetIndex;
        }
        else
        {
            videoPlayers[targetIndex].Play();
        }
    }

    // 用來控制按下「返回上一步」的按鈕，隱藏所有介面
    public void OnBackButtonPressed()
    {
        // 隱藏當前顯示的畫作 UI 和影片
        if (currentTargetIndex != -1)
        {
            uiPanels[currentTargetIndex].SetActive(false);
            videoPanels[currentTargetIndex].SetActive(false);
            npcImages[currentTargetIndex].SetActive(false);
            imageTargetsFound[currentTargetIndex] = false;
            currentTargetIndex = -1;

            // 暫停影片
            if (videoPlayers[currentTargetIndex] != null && videoPlayers[currentTargetIndex].isPlaying)
            {
                videoPlayers[currentTargetIndex].Pause();
            }
        }
    }

    public void OnButtonAudioPauseSetting()
    {
        videoPlayers[currentTargetIndex].Pause();
    }

    public void OnButtonAudioPlaySetting()
    {
        videoPlayers[currentTargetIndex].Play();
    }
}
