using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Avatar
{
    [AddComponentMenu("VivenSDK/Avatar/FacialExpressionComponent")]
    public class SDKFacialExpressionComponent : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer faceBlendShapeParent;

        [SerializeField] private List<SDKFacialExpression> sdkFacialExpressions;
        public                   SkinnedMeshRenderer       FaceBlendShapeParent => faceBlendShapeParent;
        public                   List<SDKFacialExpression> SDKFacialExpressions => sdkFacialExpressions;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SDKFacialExpressionComponent))]
    public class SDKFacialExpressionComponentEditor : Editor
    {
        private int _choiceIndex;

        // 필드 이름 상수 (직접 그릴 때 사용)
        private const string FaceBlendShapeParentProp = "faceBlendShapeParent";
        private const string SDKFacialExpressionsProp = "sdkFacialExpressions";

        public override void OnInspectorGUI()
        {
            // 1) SerializedObject 업데이트
            serializedObject.Update();

            // 2) 필요한 필드를 수동으로 그리기
            EditorGUILayout.PropertyField(serializedObject.FindProperty(FaceBlendShapeParentProp));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(SDKFacialExpressionsProp));

            // 3) 사용자 정의 UI (버튼, 팝업 등)를 수동으로 배치
            EditorGUILayout.Space(); // 구분을 위해 약간의 공백 추가

            // Inspector 대상 가져오기
            var facialExpressionComponent = (SDKFacialExpressionComponent)target;

            var blendShapeParent = facialExpressionComponent.FaceBlendShapeParent;

            // BlendShape Count 세팅 버튼
            if (GUILayout.Button("Set BlendShape Count"))
            {
                if (blendShapeParent)
                {
                    var count = blendShapeParent.sharedMesh.blendShapeCount;
                    foreach (var facialExpression in facialExpressionComponent.SDKFacialExpressions)
                    {
                        var newArr = new float[count];
                        for (var i = 0; i < facialExpression.BlendShapeValues.Length; i++)
                        {
                            if (i >= count) break;
                            newArr[i] = facialExpression.BlendShapeValues[i];
                        }

                        facialExpression.BlendShapeValues = newArr;
                    }
                }
                else
                {
                    Debug.LogError("얼굴의 BlendShape를 가진 GameObject를 선택해주세요.");
                }
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("CopyCurrentBlendShape"))
                {
                    if (blendShapeParent)
                    {
                        // 현재 선택된 Expression
                        var targetExpression = facialExpressionComponent.SDKFacialExpressions[_choiceIndex];
                        targetExpression.BlendShapeValues = new float[blendShapeParent.sharedMesh.blendShapeCount];
                        for (int i = 0; i < blendShapeParent.sharedMesh.blendShapeCount; i++)
                        {
                            targetExpression.BlendShapeValues[i] = blendShapeParent.GetBlendShapeWeight(i);
                        }
                    }
                    else
                    {
                        Debug.LogError("얼굴의 BlendShape를 가진 GameObject를 선택해주세요.");
                    }
                }

                // 팝업으로 Expression 선택
                _choiceIndex = EditorGUILayout.Popup(_choiceIndex,
                                                     facialExpressionComponent.SDKFacialExpressions
                                                                              .Select(expression =>
                                                                                  expression.ExpressionName)
                                                                              .ToArray());
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Preview Facial Expression"))
                {
                    if (blendShapeParent)
                    {
                        var targetExpression = facialExpressionComponent.SDKFacialExpressions[_choiceIndex];
                        for (int i = 0; i < blendShapeParent.sharedMesh.blendShapeCount; i++)
                        {
                            blendShapeParent.SetBlendShapeWeight(i, targetExpression.BlendShapeValues[i]);
                        }
                    }
                    else
                    {
                        Debug.LogError("얼굴의 BlendShape를 가진 GameObject를 선택해주세요.");
                    }
                }

                // 팝업으로 Expression 선택
                _choiceIndex = EditorGUILayout.Popup(_choiceIndex,
                                                     facialExpressionComponent.SDKFacialExpressions
                                                                              .Select(expression =>
                                                                                  expression.ExpressionName)
                                                                              .ToArray());
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Reset Preview"))
            {
                if (blendShapeParent)
                {
                    for (var i = 0; i < blendShapeParent.sharedMesh.blendShapeCount; i++)
                    {
                        blendShapeParent.SetBlendShapeWeight(i, 0);
                    }
                }
                else
                {
                    Debug.LogError("얼굴의 BlendShape를 가진 GameObject를 선택해주세요.");
                }
            }

            // 4) 변경사항 반영
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}