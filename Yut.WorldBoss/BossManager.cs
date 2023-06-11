using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Yut.WorldBoss
{
    public enum EState
    {
        WaitingStart,
        Preparing,
        Fighting,
        Rewarding
    }
    public class BossManager : MonoBehaviour
    {
        private static BossManager instance;
        public static BossManager Instance => instance;
        private DateTime refreshTime;
        private bool isStart = false;
        private EState state = EState.WaitingStart;
        private float frame = 0;
        private int lastFrame = 0;
        private ushort stateSeconds = 0;
        private bool success = true;
        public EState State => state;
        public bool IsStart => isStart;
        private void Awake()
        {
            instance = this;
            InitNextRefreshTime();
        }
        public void InitNextRefreshTime()
            => refreshTime = Yut.Instance.InitNextRefreshTime();
        private void GetNextRefreshTime()
            => refreshTime += new TimeSpan(1, 0, 0, 0);
        public void EndFight()
        {
            frame = 0;
            state = EState.Rewarding;
            stateSeconds = Yut.Instance.Configuration.Instance.RewardSeconds;
            PlayerManager.Instance.UpdatePlayerStusUI();
            ZombieManager.Instance.KillAll();
            if (success)
                PlayerManager.Instance.Reward();
            else
                UnturnedChat.Say(Yut.Instance.Translate("Challenge_Failed"));
        }
        private void EndBoss()
        {
            state = EState.WaitingStart;
            isStart = false;
            success = true;
            PlayerManager.Instance.CloseUI();
            PlayerManager.Instance.Clear();
            ZombieManager.Instance.Clear();
            GetNextRefreshTime();
            UnturnedChat.Say(Yut.Instance.Translate("Challenge_Ends"));
        }
        private void Update()
        {
            if(!isStart)
            {
                if ((DateTime.Now - refreshTime).TotalDays < 0)
                    return;
                if (!LevelNavigation.tryGetBounds(Yut.Instance.Configuration.Instance.BossRefreshPoint, out byte _))
                {
                    Rocket.Core.Logging.Logger.Log(Yut.Instance.Translate("Invalid_Point"),ConsoleColor.Red);
                    GetNextRefreshTime();
                    return;
                }
                ZombieManager.Instance.KillAll();
                state = EState.Preparing;
                isStart = true;
                stateSeconds = Yut.Instance.Configuration.Instance.PrepareSeconds;
                Yut.Instance.LoadCloth();
                PlayerManager.Instance.Clear();
                UnturnedChat.Say(Yut.Instance.Translate("Boss_Start", stateSeconds));
            }
            else
            {
                frame += Time.deltaTime;
                if (frame < stateSeconds)
                {
                    if(state == EState.Preparing)
                    {
                        int a = DataModule.Math.RangeToInt32(frame);
                        if ((a % Yut.Instance.Configuration.Instance.PrepareNoticeSeconds == 0 || stateSeconds - a <= 3) && a != lastFrame)
                        {
                            UnturnedChat.Say(Yut.Instance.Translate("Boss_Start", stateSeconds - a));
                            lastFrame = a;
                        }
                    }
                    return;
                }
                frame = 0;
                if (state == EState.Preparing)
                {
                    state = EState.Fighting;
                    stateSeconds = Yut.Instance.Configuration.Instance.FightingSeconds;
                    ZombieManager.Instance.Run();
                    UnturnedChat.Say(Yut.Instance.Translate("Boss_Fighting"));
                }
                else if (state == EState.Fighting)
                {
                    success = false;
                    EndFight();
                }
                else if (state == EState.Rewarding)
                    EndBoss();
            }
        }
    }
}