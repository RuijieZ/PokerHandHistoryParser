﻿using System;
using System.Runtime.Serialization;
using HandHistories.Objects.Cards;
using System.Diagnostics;

namespace HandHistories.Objects.Actions
{
    [DataContract]
    [DebuggerDisplay("{ToString()}")]
    public partial class HandAction
    {
        [DataMember]
        public string PlayerName { get; private set; }

        [DataMember]
        public HandActionType HandActionType { get; protected set; }

        /// <summary>
        /// How much was added to the pot with this action.
        /// If HandActionType is RAISE and the player have previously made a BET of 20 
        /// and then makes a RAISE to 100 the amount is 80
        /// </summary>
        [DataMember]
        public decimal Amount { get; private set; }

        /// <summary>
        /// Math.Abs(Amount)
        /// </summary>
        public decimal Absolute { get { return Math.Abs(Amount); } }

        [DataMember]
        public Street Street { get; private set; }

        [DataMember]
        public int ActionNumber { get; set; }

        [DataMember]
        public bool IsAllIn { get; private set; }

        public HandAction(string playerName,
            HandActionType handActionType,
            decimal amount,
            Street street,
            int actionNumber)
            : this(playerName, handActionType, amount, street, false, actionNumber)
        {
        }

        public HandAction(string playerName,
                          HandActionType handActionType,
                          Street street,
                          bool AllInAction = false,
                          int actionNumber = 0)
        {
            Street = street;
            HandActionType = handActionType;
            PlayerName = playerName;
            Amount = 0m;
            ActionNumber = actionNumber;
            IsAllIn = AllInAction;
        }

        public HandAction(string playerName, 
                          HandActionType handActionType,                           
                          decimal amount,
                          Street street,
                          bool AllInAction = false,
                          int actionNumber = 0)
        {
            Street = street;
            HandActionType = handActionType;
            PlayerName = playerName;
            Amount = GetAdjustedAmount(amount, handActionType);
            ActionNumber = actionNumber;
            IsAllIn = AllInAction;
        }

        public override int GetHashCode()
        {
            return PlayerName.GetHashCode() ^ HandActionType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            HandAction handAction = obj as HandAction;
            if (handAction == null) return false;

            return this == handAction;
        }

        public override string ToString()
        {
            string format = "{0} does {1} for {2} on street {3}{4}";

            return string.Format(format,
                PlayerName,
                HandActionType,
                Amount.ToString("N2"),
                Street,
                IsAllIn ? " and is All-In" : "");
        }

        public void DecreaseAmount(decimal value)
        {
            Amount = Math.Abs(Amount) - Math.Abs(value);
            Amount = GetAdjustedAmount(Amount, HandActionType);
        }

        /// <summary>
        /// Actions like calling, betting, raising should be negative amounts.
        /// Actions such as winning should be positive.
        /// Actions such as chatting should be 0 and can cause false positives if people say certain things.
        /// </summary>
        /// <param name="amount">The amount in the action.</param>
        /// <param name="type">The type of the action.</param>
        /// <returns></returns>
        public static decimal GetAdjustedAmount(decimal amount, HandActionType type)
        {
            if (amount == 0m)
            {
                return 0m;
            }

            amount = Math.Abs(amount);

            switch (type)
            {
                case HandActionType.CALL:
                    return amount*-1;                    
                case HandActionType.RAISE:
                    return amount * -1;
                case HandActionType.ALL_IN:
                    return amount * -1;
                case HandActionType.BET:
                    return amount * -1;
                case HandActionType.SMALL_BLIND:
                    return amount * -1;
                case HandActionType.BIG_BLIND:
                    return amount * -1;
                case HandActionType.STRADDLE:
                    return amount * -1;
                case HandActionType.UNCALLED_BET:
                    return amount;
                case HandActionType.POSTS:
                    return amount * -1;
                case HandActionType.POSTS_DEAD:
                    return amount * -1;
                case HandActionType.ANTE:
                    return amount * -1;
                case HandActionType.ADDS:
                    return 0.0M; // when someone adds to their stack it doesnt effect their winnings in the hand
                case HandActionType.CHAT:
                    return 0.0M; // overwrite any $ talk in the chat
                case HandActionType.JACKPOTCONTRIBUTION:
                    return 0.0M; // does not affect pot, as it goes to a jackpot
                case HandActionType.PAYS_INSURANCE_FEE:
                    return amount * -1; 
            }

            throw new ArgumentException("GetAdjustedAmount: Uknown action " + type + " to have amount " + amount);
        }

        public bool IsRaise
        {
            get
            {
                return HandActionType == HandActionType.RAISE;
            }
        }

        public bool IsPreFlopRaise
        {
            get
            {
                return Street == Street.Preflop && HandActionType == HandActionType.RAISE;
            }
        }

        [Obsolete]
        public bool IsAllInAction
        {
            get { return HandActionType == HandActionType.ALL_IN; }
        }

        public bool IsAggressiveAction
        {
            get
            {
                return HandActionType == HandActionType.RAISE ||                       
                       HandActionType == HandActionType.BET;
            }
        }

        /// <summary>
        /// This includes all actions that you have to perform to be allowwed to play(BB/SS/ANTE)
        /// </summary>
        public bool IsBlinds
        {
            get
            {
                return HandActionType == HandActionType.SMALL_BLIND ||
                       HandActionType == HandActionType.BIG_BLIND ||
                       HandActionType == HandActionType.ANTE;
            }
        }

        /// <summary>
        /// All actions that can be performed before you are dealt a hand
        /// </summary>
        public bool IsPreGameAction
        {
            get
            {
                return HandActionType == HandActionType.SMALL_BLIND ||
                       HandActionType == HandActionType.BIG_BLIND ||
                       HandActionType == HandActionType.ANTE ||
                       HandActionType == HandActionType.POSTS ||
                       HandActionType == HandActionType.POSTS_DEAD;
            }
        }

        /// <summary>
        /// POSTS & POSTS_DEAD
        /// </summary>
        public bool IsPostAction
        {
            get
            {
                return HandActionType == HandActionType.POSTS ||
                       HandActionType == HandActionType.POSTS_DEAD;
            }
        }

        public bool IsGameAction
        {
            get
            {
                return HandActionType == Actions.HandActionType.SMALL_BLIND ||
                    HandActionType == Actions.HandActionType.BIG_BLIND ||
                    HandActionType == Actions.HandActionType.ANTE ||
                    HandActionType == Actions.HandActionType.POSTS ||
                    HandActionType == Actions.HandActionType.BET ||
                    HandActionType == Actions.HandActionType.CHECK ||
                    HandActionType == Actions.HandActionType.FOLD ||
                    HandActionType == Actions.HandActionType.ALL_IN ||
                    HandActionType == Actions.HandActionType.CALL ||
                    HandActionType == Actions.HandActionType.RAISE;
            }
        }

        public bool IsDead
        {
            get
            {
                return HandActionType == Actions.HandActionType.POSTS_DEAD;
            }
        }
    }
}
