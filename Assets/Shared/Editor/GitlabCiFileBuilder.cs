using SharedEditor.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Game.Shared.Editor
{
    [InitializeOnLoad]
    public class GitlabCiFileBuilder
    {
        private const string FileContent = @"spec:
  inputs:
    mjob:
      type: string
      description: ""Job name""
      default: ""a""
---
stages:
  - deploy

deploy-game:
  stage: deploy
  tags:
    - brickstone_mac
  script:
    - |
      # Đọc dòng đầu tiên trong commit message
      first_line=$(echo ""$CI_COMMIT_MESSAGE"" | head -n1)

      # Nếu bắt đầu bằng ""-d "" thì parse ra target
      if [[ ""$first_line"" =~ ^-d[[:space:]]+(.+) ]]; then
        deploy_target=""${BASH_REMATCH[1]}""
        echo ""🚀 Deploy detected from commit message: $deploy_target""
      else
        deploy_target=""$[[ inputs.mjob ]]""
        echo ""🚀 Deploy detected from input: $deploy_target""
      fi

      case ""$deploy_target"" in
        googleplay|g) sh ./gateway_android_googleplay ;;
        googleplay_dev|g_d) sh ./gateway_android_googleplay_development ;;
        huawei|h) sh ./gateway_android_huawei ;;
        huawei_dev|h_d) sh ./gateway_android_huawei_development ;;
        appstore|a) sh ./gateway_ios_appstore ;;
        appstore_dev|a_d) sh ./gateway_ios_appstore_development ;;
        appstore_china|ac) sh ./gateway_ios_china_appstore ;;
        appstore_china_dev|ac_d) sh ./gateway_ios_china_appstore_development ;;
        facebook|fb) sh ./gateway_webgl_facebook_instant_production ;;
        facebook_dev|fb_d) sh ./gateway_webgl_facebook_instant_development ;;
        *) echo ""❌ Unknown job: $deploy_target"" && exit 1 ;;
      esac
";

        static GitlabCiFileBuilder()
        {
            EditorApplication.delayCall += OnAfterDomainReload;
        }

        private static void OnAfterDomainReload()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.delayCall -= OnAfterDomainReload;
                return;
            }
            if (SharedEditorUtils.IsGitlabRepo())
            {
                _ = SharedEditorUtils.WriteProjectRelativeFile(".gitlab-ci.yml", FileContent, silent: true);
            }
            else
            {
                SharedEditorUtils.DeleteProjectRelativeFile(".gitlab-ci.yml", silent: true);
            }

            EditorApplication.delayCall -= OnAfterDomainReload;
#if ENABLE_SHARED_EDITOR_LOGGER
            Debug.Log("✅ GitlabCiFileBuilder Domain reload complete!");
#endif
        }
    }
}