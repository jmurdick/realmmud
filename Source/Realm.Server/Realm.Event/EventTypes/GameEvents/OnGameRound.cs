﻿using Realm.Library.Common;

// ReSharper disable CheckNamespace
namespace Realm.Event
// ReSharper restore CheckNamespace
{
    public class OnGameRound : EventBase
    {
        public OnGameRound()
        {
            Name = "OnGameRound";
        }
    }
}