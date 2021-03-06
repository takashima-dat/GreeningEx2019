﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GreeningEx2019
{
    public class TitleManager : SceneManagerBase
    {
        [Tooltip("New Gameの座標。0=保存無し / 1=保存あり"), SerializeField]
        Vector3[] newGamePositions = new Vector3[2];
        [Tooltip("New Gameテキストのオブジェクト"), SerializeField]
        GameObject newGameObject = null;
        [Tooltip("Continueテキストのオブジェクト"), SerializeField]
        GameObject continueObject = null;
        [Tooltip("ミュートトグル"), SerializeField]
        Toggle seMuteToggle = null;

        /// <summary>
        /// ミュート前の効果音ボリューム
        /// </summary>
        float lastSeVolume = 0;

        /// <summary>
        /// コンティニューかどうかのフラグ
        /// </summary>
        public static bool IsContinue { get; private set; }
        
        public override void OnFadeOutDone()
        {
            SoundController.PlayBGM(SoundController.BgmType.Title, true);
            SceneManager.SetActiveScene(gameObject.scene);
            if (GameParams.ClearedStageCount == 0)
            {
                newGameObject.transform.localPosition = newGamePositions[0];
                continueObject.SetActive(false);
                IsContinue = false;
            }
            else
            {
                newGameObject.transform.localPosition = newGamePositions[1];
                continueObject.SetActive(true);
                IsContinue = true;
            }

            SeMute();
        }

        private void Update()
        {
            if (Fade.IsFading 
                || SceneChanger.NextScene != SceneChanger.SceneType.None
                || SceneChanger.NowScene != SceneChanger.SceneType.Title) return;

            if (GameParams.IsActionAndWaterButtonDown)
            {
                SoundController.Play(SoundController.SeType.Decision);

                if (!IsContinue)
                {
                    GameParams.SetNewGame();
                }
                else
                {
                    GameParams.SetContinue();
                }
                SceneChanger.ChangeScene(SceneChanger.SceneType.StageSelect);
                return;
            }

            if (Input.GetButtonDown("Esc"))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                return;
            }

            // クリアステージがなければ変更なし
            if (GameParams.ClearedStageCount == 0) return;

            if(Input.GetAxisRaw("Horizontal")>0)
            {
                SoundController.Play(SoundController.SeType.MoveCursor);
                IsContinue = true;
            }
            else if(Input.GetAxisRaw("Horizontal")<0)
            {
                SoundController.Play(SoundController.SeType.MoveCursor);
                IsContinue = false;
            }
        }

        /// <summary>
        /// SEの消音を設定します。
        /// </summary>
        public void SeMute()
        {
            if (SoundController.SeVolume > 0f)
            {
                lastSeVolume = SoundController.SeVolume;
            }

            SoundController.SeVolume = !seMuteToggle.isOn ? 0f : lastSeVolume;
            SoundController.Play(SoundController.SeType.MoveCursor);
        }
    }
}
