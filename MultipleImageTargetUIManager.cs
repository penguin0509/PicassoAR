using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using UnityEngine.Video;  // �[�J VideoPlayer ���R�W�Ŷ�
public class MultipleImageTargetUIManager : MonoBehaviour
{
    public GameObject[] uiPanels;       // �Ψ���� UI �����O�A�����C�ӵe�@
    public GameObject[] videoPanels;    // �Ψ���ܼv�������O�A�����C�ӵe�@
    public GameObject[] npcImages;      // �Ψ���� NPC �Ϥ�������A�����C�ӵe�@
    private VideoPlayer[] videoPlayers; // �Ψӱ���C�Ӽv��������

    private bool[] imageTargetsFound;   // �ΨӰl�ܭ��ǵe�@�w�g���y�L
    private int currentTargetIndex = -1; // ��e��ܪ��e�@����

    void Start()
    {
        // ��l���A���éҦ� UI�B�v���M NPC �Ϥ�
        HideAll();
        imageTargetsFound = new bool[uiPanels.Length];

        // ��l�� VideoPlayer �}�C
        videoPlayers = new VideoPlayer[videoPanels.Length];
        for (int i = 0; i < videoPanels.Length; i++)
        {
            // �T�O�C�� videoPanel ������ VideoPlayer �ե�
            videoPlayers[i] = videoPanels[i].GetComponent<VideoPlayer>();
        }
    }

    // ���éҦ��� UI�B�v���M NPC �Ϥ�
    private void HideAll()
    {
        foreach (var panel in uiPanels)
            panel.SetActive(false);
        foreach (var videoPanel in videoPanels)
            videoPanel.SetActive(false);
        foreach (var npcImage in npcImages)
            npcImage.SetActive(false);
    }

    // ���y�� ImageTarget ����ܹ����� UI �M�v��
    public void OnImageTargetFound(int targetIndex)
    {
        // �p�G�o�ӵe�@�٨S�Q���y�L�A��ܨ� UI �M�v��
        if (!imageTargetsFound[targetIndex])
        {
            // ���äW�@����ܪ� UI �M�v��
            if (currentTargetIndex != -1)
            {
                uiPanels[currentTargetIndex].SetActive(false);
                videoPanels[currentTargetIndex].SetActive(false);
                npcImages[currentTargetIndex].SetActive(false);

                // �Ȱ��W�@�����v��
                if (videoPlayers[currentTargetIndex].isPlaying)
                {
                    videoPlayers[currentTargetIndex].Pause();
                }
            }

            // ��ܷs���e�@ UI �M�v��
            uiPanels[targetIndex].SetActive(true);
            videoPanels[targetIndex].SetActive(true);
            npcImages[targetIndex].SetActive(true);

            videoPlayers[targetIndex].time = 0;
            videoPlayers[targetIndex].Play();

            // �O����e�e�@���w���
            imageTargetsFound[targetIndex] = true;
            currentTargetIndex = targetIndex;
        }
        else
        {
            videoPlayers[targetIndex].Play();
        }
    }

    // �Ψӱ�����U�u��^�W�@�B�v�����s�A���éҦ�����
    public void OnBackButtonPressed()
    {
        // ���÷�e��ܪ��e�@ UI �M�v��
        if (currentTargetIndex != -1)
        {
            uiPanels[currentTargetIndex].SetActive(false);
            videoPanels[currentTargetIndex].SetActive(false);
            npcImages[currentTargetIndex].SetActive(false);
            imageTargetsFound[currentTargetIndex] = false;
            currentTargetIndex = -1;

            // �Ȱ��v��
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
