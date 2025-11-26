using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.Contents
{
    public class YoutubeViewer : MonoBehaviour
    {
        /// <summary>
        /// Youtube Viewer가 초기화되었는지 확인하는 변수
        /// 초기화 확인 후 사용해주세요.
        /// </summary>
        public bool IsInit { get; private set; }

        /// <summary>
        /// 유튜브 링크를 받아서 영상을 로드합니다.
        /// </summary>
        /// <returns></returns>
        public void LoadYoutube(string url)
        {
        }

        /// <summary>
        /// 유튜브 재생/일시정지하는 함수
        /// </summary>
        public void PauseYoutube()
        {
        }
    }
    
}