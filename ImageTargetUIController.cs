using UnityEngine;
using Vuforia;

public class ImageTargetUIController : MonoBehaviour
{
    public GameObject targetUI; // 指定顯示的 UI 畫面
    private ObserverBehaviour observerBehaviour;
    private bool isUIActive = false; // 用來判斷 UI 是否已經顯示


    private void Start()
    {
        if (targetUI != null)
        {
            targetUI.SetActive(false); // 初始隱藏 UI
        }
        else
        {
            Debug.LogError("Target UI 未設置！請在 Inspector 中指定目標 UI。");
        }

        observerBehaviour = GetComponentInChildren<ObserverBehaviour>();
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged; // 訂閱事件
        }
        else
        {
            Debug.LogError("找不到 ObserverBehaviour，請確認 Vuforia 組件是否正確設定。");
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED && !isUIActive)
        {
            OnTargetFound();
        }
    }
    public void OnTargetFound()
    {
        Debug.Log("Target Found! 顯示 UI。");

        // 如果 UI 尚未顯示，則顯示 UI 並禁用識別
        if (!isUIActive)
        {
            isUIActive = true;
            DisableTracking();             // 禁用識別，防止掃描第二幅畫
        }

    }

    public void OnTargetLost()
    {
        Debug.Log("Target Lost! UI 仍然保持顯示。");
        // 不隱藏 UI，保持顯示狀態
    }

    // 禁用識別
    private void DisableTracking()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.enabled = false; // 停止目標識別
            Debug.Log("已禁用 Vuforia 目標識別。");
        }
    }

    // 重新啟動 Vuforia 目標識別
    private void ResumeTracking()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.enabled = true; // 重新啟用目標識別
            Debug.Log("Vuforia 已重啟目標識別。");
        }
    }

    // 用來處理退出按鈕的邏輯
    public void OnExitButtonPressed()
    {
        Debug.Log("退出按鈕被按下！");
        Invoke("ResumeTracking", 0.1f); // 延遲後重新啟動識別
        isUIActive = false; // 重置 UI 顯示狀態

    }
}
