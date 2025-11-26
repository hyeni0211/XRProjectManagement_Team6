using UnityEditor;
using UnityEditor.PackageManager;

namespace TwentyOz.VivenSDK.Scripts.Editor.Setting
{
    [InitializeOnLoad]
    public static class PackageInitializer
    {
        static PackageInitializer()
        {
            Events.registeredPackages += OnPackagesRegistered;
        }

        private static void OnPackagesRegistered(PackageRegistrationEventArgs args)
        {
            foreach (var package in args.added)
            {
                if (package.name == "com.viven.sdk") // 패키지 이름 확인
                {
                    // 설치 후 UI 창 열기
                    VivenSDKCustomSettingsWindow.ShowWindow();
                    break;
                }
            }
        }
    }
}