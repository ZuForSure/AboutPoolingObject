using System;
using MCP.DataModels.Attributes;

namespace MCP.DataModels.BaseModels
{
    public class MageError
    {
        public ErrorCode ErrorCode;
        public string ErrorMessage;

        public MageError()
        {

        }

        public MageError(ErrorCode errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }

    public class MageException : Exception
    {
        public ErrorCode ErrorCode;
        public string ErrorMessage;
    }

    public enum ErrorCode
    {
        Success = 0,
        Unknown = 1,
        UnknownError = 2,
        TradeInventoryItemIsNotTradable = 3,
        Removed = 4,
        InvalidParams = 5,
        AccountNotFound = 6,
        AccountBanned = 7,
        InvalidUsernameOrPassword = 8,
        InvalidEmailAddress = 9,
        EmailAddressNotAvailable = 10,
        InvalidUsername = 11,
        InvalidPassword = 12,
        DataNotExist = 13,
        DataAlreadyExisted = 14,
        InvalidIndex = 15,
        IndexOutOfRane = 16,
        InvalidItemState = 17,
        OpponentOffline = 18,
        InsufficientMoney = 19,
        NotEnoughItem = 20,
        ItemNotLegendary = 21,
        ItemNotMaxLevel = 22,
        ItemNotCrafted = 23,
        ItemIsLocked = 24,
        AnswerMismatch = 25,
        ClanNotJoin = 26,
        ItemTypeMismatch = 27,
        InsufficientStat = 28,
        OtpInvalid = 29,
        TrophyInvalid = 30,
        EndArena = 31,
        DontHavePermissionToRegister = 32,
        InsufficientClanTrophy = 33,
        InsufficientClanMoney = 34,
        RequestOnCooldown = 35,
        TrophyOutOfRange = 36
    }
}
