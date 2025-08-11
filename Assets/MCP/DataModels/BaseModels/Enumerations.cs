namespace MCP.DataModels.BaseModels
{
    public enum AuthenType { None, Guess, Email, Facebook, Apple, Google, CustomId }
    public enum ApplicationType { None, Android, iOS, WebGL, Windows }

    //API
    public enum ApiAction { CompleteQuest, BuyItem, UpgradeItem, UseItem, MergeItem, MeltItem, StartBattle, EndBattle, BuyIAP }
    //Chat
    public enum ChatID
    {
        None = 0,
        TakeItemFromStorage = 1,
        MoveItemToStorage = 2,
        WatchTower = 3,
        Player = 4,
        StartProtect = 5,
        EndProtect = 6,
    }

    // Item
    public enum RareType { Common = 0, Rare = 1, Epic = 2, Legendary = 3 };
    public enum RewardState { None, Ready, Rewarded, Opening };
    public enum QuestType { KillMobNumber, KillBossNumber, OpenChestNumber, KillPlayerNumber, TrophyNumber, CraftNumber, HaveItem, Text, CraftItem, JoinClan, PlayerStat, GoldNumber, DiamondNumber, ShardNumber };

    public enum ItemType { None, Box, Bundle }
    public enum ItemClass { BaseItem = -1, Card = 0, Item = 1, Emote = 2, Recipe = 3, Currency = 4, Bundle = 5, Box = 6 }
    public enum MemberRole { Leader, Coleader, Member }
    public enum PlayerRole { Admin, Operator, Player };
    public enum GamePlayState { Lock, InProgress, Completed }
    public enum DifficultMode { Easy, Hard, Heroic }
    public enum OrderType { Sell, Buy }
    public enum RandomType { Option, CheckBox };
    public enum QuestResetSchedule { None, Daily, Weekly, Monthly }

    //WARNING !!! Add new stat need to change total number in Card class 
    public enum StatType {
        Health, Damage, Speed, HitSpeed, Defense, Critical, InventorySlot, CraftChance, SuccessChance, IngredientSlot,
        ClanSlot, Heal, Range, Duration, CriticalBonus, Dodge, Accuracy, Luck, LifeSteal, Counterattack,
        CoolDown, NumberTarget, Radius, GoldRate,DiamondRate,IngredientRate, PoisonStat, PoisonDoT, Multitarget
    }

    public enum SkillType { None, Buff, TeleportRamdom, TeleportHome, BuffAOE, Spawner, AttackRandom, AttackAOE, Invisible, Immortal, DebuffAOE, Heath, HeathAOE }
    public enum NFTState { None, Active, Deactive, Staked, Withdrawed }
    public enum NpcType { Shop, Governor, Assasin, Trader, Clan, Artifact, Guard, Merge, ChangeName, Colosseum, ArenaShop, ClanWar, Event }
    public enum MapItemState { None, Opening, Opened }
    public enum MapItemType { Chest, Gate, Key }
    public enum MapZoneType { None,Village, SafeZone, FireCraft, Undead,White, Arena, RegisterZone, Viking }

    public enum LoginType {Login,Register,QuickLogin,ForgetPass,VerifyForgetPass}

    public enum AccountInfoState { None, AddEmail,RemoveEmail, ForgetPass};

    public enum DropType
    {
        None = 0,
        Skin = 1,
        Equipment = 2,
        Item = 3,
        Mob = 4,
        Scroll = 5,
        Ingredient = 6,
        Pet = 7,
        Horse = 8,
        Summon = 9,
        BuffItem = 10,
        Collection = 11,
        EventItem = 12,
    }

    public enum EquipmentState
    {
        New,
        Inventory,
        Character,
        Ingredient,
        Trading,
        Lock,
        Collection,
    }

    public enum EquipmentType
    {
        Hair = 0,
        FaceHair = 1,
        Helmet = 2,
        Cloth = 3,
        Boot = 4,
        Back = 5,
        Weapons = 6,
        Body = 7,
        Eye = 8,
        Horse = 9,
        Item = 10,
        Pet = 11,
        Mob = 12,
        Key = 13,
        Artifact = 14,
        Ingredient = 15,
        Scroll = 16,
        Npc = 17,
        Summon = 18,
        ArtifactShard = 19,
        BuildingPart = 20,
        Building = 21,
        BuffItem = 22,
        SkinAttack = 23,
        SkinRun = 24,
        SkinIdle = 25,
        SkinDeath = 26,
        SkinClan = 27,
        SkinTitle = 28,
        EventItem = 29,
    }

    public enum MessageStatus
    {
        New,
        Read,
        Deleted
    }

    public enum MobLimited { None, RedDragon, BossSafeZone }
    public enum CharacterType { Player, Bot, Mob, Npc, Building }
    public enum GamePlayMemberType { Player = 0, Bot = 1 }
    public enum GamePlayCompleteType { WinNumber, ScoreNumber, TrophyNumber, Level }

    public enum TutorialStep
    {
        None,
        Move,
        Minning,
        UseEquipment,
        Upgrade,
        Fullgear
    }

    public enum StatDataType { Normal, Multipler }
    public enum MessageType
    {
        Mail = 0,
        PushNotification = 1,
        FriendRequest = 2,
        News = 3
    }
    public enum NotificationSendType { Onetime, Repeate }
    public enum NotificationScheduleType { Daily = 1, Weekly = 8 }
    public enum TransactionState { Created, Pending, Error, Completed, Cancelled }
    public enum DialogType { Tip, Dialog, Tutorial, Notification, Loading, Help, Mission, All }

    //Battle

    public enum PlayMode { Solo = 0, Group = 1, Corrupted = 2, PVP = 3 }
    public enum BattleState { Prepared, Ready, Playing, Ended, Validated, Invalided }
    public enum RankFilterType { None = 0, Global = 1, Local = 2 }
    public enum BoardType { Player = 0, Friend = 1, Clan = 2, ClanWar = 3 }
    public enum NotificationEventType { None = 0, OnUpdatePlayerFriends = 1, OnUpdatePlayerMessage = 2 }
    public enum MCPEventDetail { Price, Level, Currency, ItemId, GameplayConfig, RewardType, RewardId, Quantity }
    public enum AccountBannedStatus {PendingBanAction = 1, Banned = 2, PendingUnbanAction = 3, Unbanned = 4}

    public enum MCPEventType
    {
        FirstOpen,

        ///<summary>
        ///Activation.OpenActivationWindow
        ///</summary>
        OpenActivationWindow,
        ///<summary>
        ///Activation.ActivateFailed
        ///</summary>
        ActivateFailed,
        ///<summary>
        ///Activation.ActivateSuccessful
        ///</summary>
        ActivateSuccessful,
        ///<summary>
        ///Activation.DiscardActivationWindow
        ///</summary>
        DiscardActivationWindow,
        ///<summary>
        ///Activation.OpenExternalLink
        ///</summary>
        OpenExternalLink,
        ///<summary>
        ///Admob.VideoAdLoaded
        ///</summary>
        VideoAdLoaded,
        ///<summary>
        ///Admob.VideoAdStarted
        ///</summary>
        VideoAdStarted,
        ///<summary>
        ///Admob.VideoAdOpened
        ///</summary>
        VideoAdOpened,
        ///<summary>
        ///Admob.VideoAdClosed
        ///</summary>
        VideoAdClosed,
        ///<summary>
        ///Admob.VideoAdRewarded
        ///</summary>
        VideoAdRewarded,
        ///<summary>
        ///Admob.VideoAdFailtoLoaded
        ///</summary>
        VideoAdFailtoLoaded,
        ///<summary>
        ///Admob.InterstitialAdLoaded
        ///</summary>
        InterstitialAdLoaded,
        ///<summary>
        ///Admob.InterstitialAdShow
        ///</summary>
        InterstitialAdShow,
        ///<summary>
        ///Admob.InterstitialAdOpened
        ///</summary>
        InterstitialAdOpened,
        ///<summary>
        ///Admob.InterstitialAdClosed
        ///</summary>
        InterstitialAdClosed,
        ///<summary>
        ///Admob.InterstitialAdFailtoLoaded
        ///</summary>
        InterstitialAdFailtoLoaded,
        ///<summary>
        ///Admob.BannerAdLoaded
        ///</summary>
        BannerAdLoaded,
        ///<summary>
        ///Admob.BannerAdShow
        ///</summary>
        BannerAdShow,
        ///<summary>
        ///Admob.BannerAdClosed
        ///</summary>
        BannerAdClosed,
        ///<summary>
        ///Admob.BannerAdOpened
        ///</summary>
        BannerAdOpened,
        ///<summary>
        ///UserAction.OpenApplication
        ///</summary>
        OpenApplication,
        ///<summary>
        ///UserAction.QuitApplication
        ///</summary>
        QuitApplication,
        ///<summary>
        ///UserAction.OpenCreateUserProfile
        ///</summary>
        OpenCreateUserProfile,
        ///<summary>
        ///UserAction.CancelCreateUserProfile
        ///</summary>
        CancelCreateUserProfile,
        ///<summary>
        ///UserAction.CreateUserProfileFailed
        ///</summary>
        CreateUserProfileFailed,
        ///<summary>
        ///UserAction.CreateUserProfileSuccesful
        ///</summary>
        CreateUserProfileSuccesful,
        ///<summary>
        ///UserAction.UpdateUserData
        ///</summary>
        UpdateUserData,
        ///<summary>
        ///UserAction.OpenLogin
        ///</summary>
        OpenLogin,
        ///<summary>
        ///UserAction.LoginFailed
        ///</summary>
        LoginFailed,
        ///<summary>
        ///UserAction.LoginSuccessful
        ///</summary>
        LoginSuccessful,
        ///<summary>
        ///UserAction.AddCharacter
        ///</summary>
        AddCharacter,
        ///<summary>
        ///UserAction.EquipItem
        ///</summary>
        EquipItem,
        ///<summary>
        ///UserAction.UseItem
        ///</summary>
        UseItem,
        ///<summary>
        ///UserAction.OpenScreen
        ///</summary>
        OpenScreen,
        ///<summary>
        ///UserAction.ClickObject
        ///</summary>
        ClickObject,
        ///<summary>
        ///Store.OpenStore
        ///</summary>
        OpenStore,
        ///<summary>
        ///Store.OpenStoreCategory
        ///</summary>
        OpenStoreCategory,
        ///<summary>
        ///Store.SearchItem
        ///</summary>
        SearchItem,
        ///<summary>
        ///Store.OpenItem
        ///</summary>
        OpenItem,
        ///<summary>
        ///Store.CheckOutItem
        ///</summary>
        CheckOutItem,
        ///<summary>
        ///Store.ConfirmPaymentItem
        ///</summary>
        ConfirmPaymentItem,
        ///<summary>
        ///Store.CancelPaymentItem
        ///</summary>
        CancelPaymentItem,
        ///<summary>
        ///InAppPurchase.OpenIAPWindow
        ///</summary>
        OpenIAPWindow,
        ///<summary>
        ///InAppPurchase.CloseIAPWindow
        ///</summary>
        CloseIAPWindow,
        ///<summary>
        ///InAppPurchase.CheckoutIAPWindow
        ///</summary>
        CheckoutIAPWindow,
        Quest,
        ApplicationSignatureFailed,
        ApplicationSignatureSuccess,
        LevelUp,
        StartBattle,
        WinBattle,
        LoseBattle,
        DrawBattle,
        BuyItem,
        GetReward,
        UpgradeItem,
        VideoAdRequest,
        InterstitialAdRequest,
        FirstIAPAction,
        ConfirmPaymentItemCancelled,
    }
}