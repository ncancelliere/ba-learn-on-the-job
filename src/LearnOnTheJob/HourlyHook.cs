using System;
using BAModAPI;
using UnityEngine;

namespace LearnOnTheJob
{
    /// <summary>
    /// Keeps a handler subscribed to GlobalEvents.onNewHour for as long as this object lives.
    ///
    /// A plain one-time subscription isn't enough: loading a save from inside the city (the in-game
    /// Load screen, or continuing as an heir) calls GlobalEvents.Init(), which drops every
    /// subscriber, but the mod loader sees the City scope as already active and doesn't reload
    /// city-scope mods. Without this, the mod would silently stop after the first in-game load.
    /// </summary>
    internal sealed class HourlyHook : MonoBehaviour
    {
        private Action _handler;
        private IModLogger _logger;
        private Action _lastSeen;

        public static HourlyHook Attach(Action handler, IModLogger logger)
        {
            var host = new GameObject("LearnOnTheJob.HourlyHook");
            DontDestroyOnLoad(host);

            var hook = host.AddComponent<HourlyHook>();
            hook._handler = handler;
            hook._logger = logger;
            hook.EnsureSubscribed();
            return hook;
        }

        public void Detach()
        {
            GlobalEvents.onNewHour -= _handler;
            _handler = null;
            Destroy(gameObject);
        }

        private void Update()
        {
            // The delegate reference changes on any subscribe/unsubscribe, so only scan the
            // invocation list when it has changed since we last looked.
            if (_handler == null || ReferenceEquals(GlobalEvents.onNewHour, _lastSeen)) return;

            if (EnsureSubscribed())
            {
                _logger.Info("Re-attached hourly hook after the game reset its events.");
            }
        }

        private void OnDestroy()
        {
            if (_handler != null) GlobalEvents.onNewHour -= _handler;
        }

        private bool EnsureSubscribed()
        {
            var current = GlobalEvents.onNewHour;
            var subscribed = current != null && Array.IndexOf(current.GetInvocationList(), _handler) >= 0;
            if (!subscribed) GlobalEvents.onNewHour += _handler;

            _lastSeen = GlobalEvents.onNewHour;
            return !subscribed;
        }
    }
}
