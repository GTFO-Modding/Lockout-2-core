using AssetShards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace Lockout_2_core
{
    public static class CoroutineHandler
    {
        private class Handler : MonoBehaviour
        {
            public Handler(System.IntPtr ptr) : base(ptr)
            { }

            private List<IRoutine> m_routines;
            public static Handler Current { get; private set; }

            [HideFromIl2Cpp]
            public void Add(IRoutine routine)
            {
                this.m_routines.Add(routine);
            }

            public void Start()
            {
                this.m_routines = new List<IRoutine>();
                Current = this;
            }

            public void Update()
            {
                int index = 0;
                while (index < this.m_routines.Count)
                {
                    var routine = this.m_routines[index];
                    if (!routine.Tick())
                        this.m_routines.RemoveAt(index);
                    else
                        index++;
                }
            }
        }

        internal static void Init() // initialize internal il2cpp class
        {
            ClassInjector.RegisterTypeInIl2Cpp<Handler>();

            AssetShardManager.add_OnStartupAssetsLoaded((System.Action)LoadHandler);
        }

        public static IRoutine Add(IEnumerator enumerator)
        {
            var routine = new Routine(enumerator);
            Handler.Current.Add(routine);
            return routine;
        }

        public static IRoutine Add(Il2CppSystem.Collections.IEnumerator enumerator)
        {
            var routine = new Routine(new Il2CppEnumeratorWrapper(enumerator));
            Handler.Current.Add(routine);
            return routine;
        }

        private static void LoadHandler()
        {
            var obj = new GameObject("Coroutine Handler [Twitch Dice]");
            obj.AddComponent<Handler>();
            GameObject.DontDestroyOnLoad(obj);
        }

        private static IEnumerator WaitForSecondsEnumerator(float seconds)
        {
            float start = Time.time;

            while (Time.time - start < seconds)
                yield return null;
        }

        private static IEnumerator WaitForSecondsRealtimeEnumerator(float seconds)
        {
            DateTime start = DateTime.Now;

            while ((DateTime.Now - start).TotalSeconds < seconds)
                yield return null;
        }

        /// <summary>
        /// Representation of a routine.
        /// </summary>
        public interface IRoutine
        {
            /// <summary>
            /// Adds an inner routine.
            /// </summary>
            /// <param name="enumerator">The inner routine</param>
            void StartSubRoutine(IEnumerator enumerator);

            /// <summary>
            /// Ticks this routine.
            /// </summary>
            /// <returns><see langword="true"/> if the routine is not finished, otherwise <see langword="false"/></returns>
            bool Tick();

            /// <summary>
            /// Stops this routine.
            /// </summary>
            void Stop();
        }

        private abstract class BaseRoutine : IRoutine
        {
            public Stack<IEnumerator> m_stack;

            public BaseRoutine(IEnumerator enumerator)
            {
                this.m_stack = new Stack<IEnumerator>();
                this.m_stack.Push(enumerator);
            }

            public void StartSubRoutine(IEnumerator enumerator)
            {
                this.m_stack.Push(enumerator);
            }

            public void Stop()
            {
                this.m_stack.Clear();
            }

            protected abstract void HandleResult(object result);

            public virtual bool Tick()
            {
                if (this.m_stack.Count > 0)
                {
                    var current = this.m_stack.Peek();
                    bool finished = current.MoveNext();
                    if (!finished)
                    {
                        this.m_stack.Pop();
                    }
                    else
                    {
                        this.HandleResult(current.Current);
                    }
                }

                return this.m_stack.Count > 0;
            }
        }

        private class Routine : BaseRoutine
        {
            public Routine(IEnumerator enumerator) : base(enumerator)
            { }

            protected override void HandleResult(object result)
            {
                if (result is WaitForSeconds wfs)
                    this.StartSubRoutine(WaitForSecondsEnumerator(wfs.m_Seconds));
                else if (result is WaitForSecondsRealtime wfsr)
                    this.StartSubRoutine(WaitForSecondsRealtimeEnumerator(wfsr.waitTime));
                else if (result is Il2CppSystem.Collections.IEnumerator gayEnumerator)
                    this.StartSubRoutine(new Il2CppEnumeratorWrapper(gayEnumerator));
                else if (result is IEnumerator enumerator)
                    this.StartSubRoutine(enumerator);
            }
        }

        private class Il2CppEnumeratorWrapper : IEnumerator
        {
            private readonly Il2CppSystem.Collections.IEnumerator il2cppEnumerator;

            public Il2CppEnumeratorWrapper(Il2CppSystem.Collections.IEnumerator il2CppEnumerator) => il2cppEnumerator = il2CppEnumerator;
            public bool MoveNext() => il2cppEnumerator.MoveNext();
            public void Reset() => il2cppEnumerator.Reset();
            public object Current => il2cppEnumerator.Current;
        }
    }
}
