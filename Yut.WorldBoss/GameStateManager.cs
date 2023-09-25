using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
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
    public delegate void StateChangedHandler(EState newState);
    public class GameStateManager : MonoBehaviour
    {
        private static GameStateManager instance;
        public static GameStateManager Instance => instance;
        private Refresh refreshTime;
        private ModeConfig modeConfig;
        private bool isStart = false;
        private EState state = EState.WaitingStart;
        private float frame = 0;
        private int lastFrame = 0;
        private ushort stateSeconds = 0;
        private bool success = true;
        private bool isCloseUI = false;
        private int bossTable;
        private int minionTable;
        private int lastSeconds = 0;
        public static event StateChangedHandler OnStateChanged;
        internal int BossTable => bossTable;
        internal int MinionTable => minionTable;
        internal ModeConfig ModeConfig => modeConfig;
        internal EState State => state;
        internal bool IsStart => isStart;
        internal SkillPair GetSkill(string skillName)
        {
            SkillPair config = modeConfig.SkillConfig.SkillPairs.Find(x => x.Key.ToLower() == skillName.ToLower());
            if (config != null)
                return config;
            return new SkillPair(skillName, "Unknow", 10);
        }
        internal void LoadCloth()
        {
            bossTable = LevelZombies.tables.FindIndex(x => x.name == modeConfig.Region.BossTable);
            minionTable = LevelZombies.tables.FindIndex(x => x.name == modeConfig.Region.MinionTable);
            if (bossTable < 0)
                bossTable = UnityEngine.Random.Range(0, LevelZombies.tables.Count);
            if (minionTable < 0)
                minionTable = UnityEngine.Random.Range(0, LevelZombies.tables.Count);
        }
        private void Awake()
        {
            instance = this;
            InitNextRefreshTime();
        }
        public void InitNextRefreshTime()
        {
            refreshTime = Yut.Instance.InitNextRefreshTime();
            modeConfig = Yut.Instance.GetModeConfig(refreshTime.Mode);
            if (modeConfig == null)
                modeConfig = ModeConfig.Default;
        }
        internal void EndFight()
        {
            frame = 0;
            lastSeconds = 0;
            state = EState.Rewarding;
            stateSeconds = modeConfig.StateConfig.RewardSeconds;
            PlayerManager.Instance.UpdatePlayerStusUI();
            ZombieManager.Instance.KillAll();
            BossSkillManager.Instance.Destroy();
            if (success)
                PlayerManager.Instance.Reward();
            else
                UnturnedChat.Say(Yut.Instance.Translate("Challenge_Failed"));
        }
        private void EndBoss()
        {
            state = EState.WaitingStart;
            isStart = false;
            isCloseUI = false;
            success = true;
            PlayerManager.Instance.CloseUI();
            PlayerManager.Instance.SendMessageToPlayers(Yut.Instance.Translate("Challenge_Ends"));
            PlayerManager.Instance.Clear();
            ZombieManager.Instance.Clear();
            InitNextRefreshTime();
        }
        private void Update()
        {
            if (!isStart)
            {
                TimeSpan span = DateTime.Now.TimeOfDay;
                if (span.Hours != refreshTime.Hour || span.Minutes != refreshTime.Minute)
                    return;
                if (!LevelNavigation.tryGetBounds(Yut.Instance.Configuration.Instance.BossRefreshPoint, out byte _))
                {
                    Rocket.Core.Logging.Logger.Log(Yut.Instance.Translate("Invalid_Point"), ConsoleColor.Red);
                    InitNextRefreshTime();
                    return;
                }
                state = EState.Preparing;
                stateSeconds = modeConfig.StateConfig.PrepareSeconds;
                LoadCloth();
                PlayerManager.Instance.Clear();
                ZombieManager.Instance.KillAll();
                isStart = true;
                UnturnedChat.Say(Yut.Instance.Translate("Boss_Start", stateSeconds));
                OnStateChanged?.Invoke(state);
            }
            else
            {
                frame += Time.deltaTime;
                if (frame < stateSeconds)
                {
                    if (state == EState.Preparing)
                    {
                        int a = DataModule.Math.RangeToInt32(frame);
                        if ((a % DataModule.Math.RangeToByte(Yut.Instance.Configuration.Instance.PrepareNoticeSeconds, 1) == 0 || stateSeconds - a <= 3) && a != lastFrame)
                        {
                            UnturnedChat.Say(Yut.Instance.Translate("Boss_Start", stateSeconds - a));
                            lastFrame = a;
                        }
                    }
                    else if (state == EState.Fighting)
                    {
                        int a = Mathf.FloorToInt(stateSeconds - frame);
                        if (a != lastSeconds)
                        {
                            PlayerManager.Instance.UpdateTime(a);
                            lastSeconds = a;
                        }
                    }
                    else if (state == EState.Rewarding) 
                    {
                        if(frame >= 60 && !isCloseUI)
                        {
                            PlayerManager.Instance.CloseUI();
                            isCloseUI = true;
                        }
                    }
                    return;
                }
                frame = 0;
                switch (state)
                {
                    case EState.Preparing:
                        state = EState.Fighting;
                        lastFrame = 0;
                        stateSeconds = modeConfig.StateConfig.FightingSeconds;
                        PlayerManager.Instance.OpenUI();
                        ZombieManager.Instance.Run();
                        PlayerManager.Instance.TeleportPlayers();
                        UnturnedChat.Say(Yut.Instance.Translate("Boss_Fighting"));
                        OnStateChanged?.Invoke(state);
                        break;
                    case EState.Fighting:
                        success = false;
                        EndFight();
                        OnStateChanged?.Invoke(state);
                        break;
                    case EState.Rewarding:
                        EndBoss();
                        OnStateChanged?.Invoke(state);
                        break;
                }
            }
        }
    }
}