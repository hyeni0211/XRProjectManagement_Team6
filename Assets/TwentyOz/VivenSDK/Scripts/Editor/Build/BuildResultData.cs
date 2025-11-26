using System;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    /// <summary>
    /// 빌드 결과를 담는 구조체입니다.
    /// 빌드 대상, 성공 여부, 메시지 등의 정보를 포함합니다.
    /// </summary>
    public struct BuildResultData
    {
        /// <summary>
        /// 빌드 대상의 이름
        /// </summary>
        public readonly string BuildTarget;
        
        /// <summary>
        /// 빌드 성공 여부
        /// </summary>
        public readonly bool IsSuccess;
        
        /// <summary>
        /// 빌드 결과 메시지
        /// </summary>
        public readonly string BuildMessage;
        
        /// <summary>
        /// 빌드 소요 시간 (초)
        /// </summary>
        public readonly float BuildDuration;
        
        /// <summary>
        /// 빌드 출력 경로
        /// </summary>
        public readonly string OutputPath;

        /// <summary>
        /// BuildResultData 구조체의 생성자
        /// </summary>
        /// <param name="buildTarget">빌드 대상 이름</param>
        /// <param name="buildMessage">빌드 결과 메시지</param>
        /// <param name="isSuccess">빌드 성공 여부</param>
        /// <param name="buildDuration">빌드 소요 시간 (초)</param>
        /// <param name="outputPath">빌드 출력 경로</param>
        public BuildResultData(string buildTarget, string buildMessage, bool isSuccess = false, float buildDuration = 0f, string outputPath = "")
        {
            BuildTarget = buildTarget;
            IsSuccess = isSuccess;
            BuildMessage = buildMessage;
            BuildDuration = buildDuration;
            OutputPath = outputPath;
        }

        /// <summary>
        /// 빌드 실패 결과를 생성합니다.
        /// </summary>
        /// <param name="buildTarget">빌드 대상 이름</param>
        /// <param name="buildMessage">실패 메시지</param>
        /// <param name="buildDuration">빌드 소요 시간 (초)</param>
        /// <param name="outputPath">빌드 출력 경로</param>
        /// <returns>빌드 실패 결과 데이터</returns>
        public static BuildResultData Fail(string buildTarget, string buildMessage, float buildDuration = 0f, string outputPath = "")
        {
            return new BuildResultData(buildTarget, buildMessage, false, buildDuration, outputPath);
        }
        
        /// <summary>
        /// 빌드 성공 결과를 생성합니다.
        /// </summary>
        /// <param name="buildTarget">빌드 대상 이름</param>
        /// <param name="buildMessage">성공 메시지</param>
        /// <param name="buildDuration">빌드 소요 시간 (초)</param>
        /// <param name="outputPath">빌드 출력 경로</param>
        /// <returns>빌드 성공 결과 데이터</returns>
        public static BuildResultData Success(string buildTarget, string buildMessage, float buildDuration = 0f, string outputPath = "")
        {
            return new BuildResultData(buildTarget, buildMessage, true, buildDuration, outputPath);
        }

        /// <summary>
        /// VivenBuildData를 사용하여 빌드 실패 결과를 생성합니다.
        /// </summary>
        /// <param name="buildData">빌드 데이터</param>
        /// <param name="buildMessage">실패 메시지</param>
        /// <param name="buildDuration">빌드 소요 시간 (초)</param>
        /// <param name="outputPath">빌드 출력 경로</param>
        /// <returns>빌드 실패 결과 데이터</returns>
        public static BuildResultData Fail(VivenBuildData buildData, string buildMessage, float buildDuration = 0f, string outputPath = "")
        {
            return new BuildResultData(buildData.GetBuildName(), buildMessage, false, buildDuration, outputPath);
        }
        
        /// <summary>
        /// VivenBuildData를 사용하여 빌드 성공 결과를 생성합니다.
        /// </summary>
        /// <param name="buildData">빌드 데이터</param>
        /// <param name="buildMessage">성공 메시지</param>
        /// <param name="buildDuration">빌드 소요 시간 (초)</param>
        /// <param name="outputPath">빌드 출력 경로</param>
        /// <returns>빌드 성공 결과 데이터</returns>
        public static BuildResultData Success(VivenBuildData buildData, string buildMessage, float buildDuration = 0f, string outputPath = "")
        {
            return new BuildResultData(buildData.GetBuildName(), buildMessage, true, buildDuration, outputPath);
        }
        
        public static implicit operator bool(BuildResultData result)
        {
            return result.IsSuccess;
        }
    }
}