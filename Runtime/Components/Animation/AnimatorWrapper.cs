using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EwigeDreamer.Additional.Components.Animation
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorWrapper<TStateEnum> : MonoBehaviour where TStateEnum : Enum
    {
        private static readonly Dictionary<TStateEnum, int> AnimHashDict;
        private static readonly Dictionary<int, TStateEnum> HashAnimDict;

        private readonly UnityEvent<string> _onAnimationEvent = new();
        public UnityEvent<string> OnAnimationEvent => _onAnimationEvent;
        
        private IReadOnlyList<AnimationClip> _clips;
        
        public Animator Animator { get; private set; }
        public IReadOnlyList<AnimationClip> Clips => _clips ??= Animator.runtimeAnimatorController.animationClips;

        static AnimatorWrapper()
        {
            var pairs = Enum.GetValues(typeof(TStateEnum))
                .Cast<TStateEnum>()
                .Select(a => (state: a, hash: Animator.StringToHash(a.ToString())))
                .ToList();

            AnimHashDict = pairs.ToDictionary(a => a.state, a => a.hash);
            HashAnimDict = pairs.ToDictionary(a => a.hash, a => a.state);
        }

        protected virtual void Awake()
        {
            Animator = GetComponent<Animator>();
        }
        
        public AnimationClip GetClip(TStateEnum state) => GetClip(state.ToString());
        public AnimationClip GetClip(string key)
        {
            key = key.ToLower();
            foreach (var clip in Clips)
                if (clip.name.ToLower() == key)
                    return clip;
            return null;
        }
        
        public float GetDuration(TStateEnum state) => GetDuration(state.ToString());
        public float GetDuration(string key)
        {
            var clip = GetClip(key);
            if (clip != null) return clip.length;
            Debug.LogWarning($"{nameof(clip)} '{key}' is null!", this);
            return 0f;
        }
        
        /// <summary>
        /// Calls from Unity. Needs animation event 'CallAnimationEvent' with string parameter inside animation clip
        /// </summary>
        /// <param name="eventId"></param>
        private void CallAnimationEvent(string eventId) => _onAnimationEvent.Invoke(eventId);

        public float GetEventTime(TStateEnum state, string eventId) => GetEventTime(state.ToString(), eventId);
        public float GetEventTime(string key, string eventId)
        {
            var clip = GetClip(key);
            if (clip == null) return 0f;
            var eventName = nameof(CallAnimationEvent);
            return clip.events
                .Where(a => a.functionName == eventName && a.stringParameter == eventId)
                .Select(a => a.time)
                .FirstOrDefault();
        }

        public float GetFirstEventTime(TStateEnum state) => GetFirstEventTime(state.ToString());
        public float GetFirstEventTime(string key)
        {
            var clip = GetClip(key);
            if (clip == null) return 0f;
            return clip.events
                .Select(a => a.time)
                .OrderBy(a => a)
                .FirstOrDefault();
        }

        public void SwitchState(TStateEnum state, float duration)
        {
            if (Animator.HasState(0, AnimHashDict[state]))
                Animator.CrossFade(AnimHashDict[state], duration);
            else
                Debug.LogWarning($"{nameof(Animator)} doesn't have state {typeof(TStateEnum).Name}.{state}", this);
        }
        
        public void PlayState(TStateEnum state)
        {
            if (Animator.HasState(0, AnimHashDict[state]))
                Animator.Play(AnimHashDict[state]);
            else
                Debug.LogWarning($"{nameof(Animator)} doesn't have state {typeof(TStateEnum).Name}.{state}", this);
        }

        public void CrossFade(TStateEnum state, float normalizedDuration = 0.1f)
        {
            if (Animator.HasState(0, AnimHashDict[state]))
                Animator.CrossFade(AnimHashDict[state], normalizedDuration);
            else
                Debug.LogWarning($"{nameof(Animator)} doesn't have state {typeof(TStateEnum).Name}.{state}", this);
        }
        
        public void CrossFadeFixedTime(TStateEnum state, float duration = 0.25f)
        {
            if (Animator.HasState(0, AnimHashDict[state]))
                Animator.CrossFadeInFixedTime(AnimHashDict[state], duration);
            else
                Debug.LogWarning($"{nameof(Animator)} doesn't have state {typeof(TStateEnum).Name}.{state}", this);
        }

        public void Stop()
        {
            Animator.StopPlayback();
        }

        public TStateEnum GetCurrentState()
        {
            var stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
            var hash = stateInfo.shortNameHash;
            if (HashAnimDict.TryGetValue(hash, out var state)) return state;
            Debug.LogError("Unknown animator state!", this);
            return default;
        }
    }
}
