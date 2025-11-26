using System;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    /// <summary>
    /// 빌드 결과를 저장하는 ScriptableObject 클래스입니다.
    /// 빌드 결과를 에셋으로 저장하고 관리할 수 있습니다.
    /// </summary>
    [CreateAssetMenu]
    public class BuildResultSO : ScriptableObject
    {
        [Header("빌드 결과 정보")]
        /// <summary>
        /// 빌드 대상의 이름
        /// </summary>
        public string _buildTarget;
        
        /// <summary>
        /// 빌드 성공 여부
        /// </summary>
        public bool IsBuildSuccess;
        
        /// <summary>
        /// 빌드 결과 메시지
        /// </summary>
        [TextArea(3, 10)]
        public string _buildMessage;
        
        /// <summary>
        /// 빌드 완료 시간
        /// </summary>
        public DateTime _buildCompletionTime;
        
        /// <summary>
        /// 빌드 소요 시간 (초)
        /// </summary>
        public float _buildDuration;
        
        /// <summary>
        /// 빌드 결과 파일 경로
        /// </summary>
        public string _buildOutputPath;

        /// <summary>
        /// 빌드 결과 데이터를 초기화합니다.
        /// </summary>
        public void Clear()
        {
            _buildTarget = "";
            IsBuildSuccess = false;
            _buildMessage = "";
            _buildCompletionTime = DateTime.Now;
            _buildDuration = 0f;
            _buildOutputPath = "";
        }
        
        /// <summary>
        /// BuildResultData로부터 빌드 결과를 설정합니다.
        /// </summary>
        /// <param name="target">설정할 빌드 결과 데이터</param>
        public void From(BuildResultData target)
        {
            Clear();
            _buildTarget = target.BuildTarget;
            IsBuildSuccess = target.IsSuccess;
            _buildMessage = target.BuildMessage;
            _buildCompletionTime = DateTime.Now;
            _buildDuration = target.BuildDuration;
            _buildOutputPath = target.OutputPath;
        }
    }
}