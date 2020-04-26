using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DeBox.Analytics
{
    /// <summary>
    /// A proxy analytics manager that performs Flush automatically by time
    /// </summary>
    public class AutoFlushAnalyticsManager : BaseProxyAnalyticsManager
    {
        internal class AutoflushAnaltyticsManagerCoroutineRunner : MonoBehaviour {}

        /// <summary>
        /// The duration between the Flush operations
        /// </summary>
        public float FlushDuration { get; private set; }
        
        private AutoflushAnaltyticsManagerCoroutineRunner _coroutineRunner;
        
        /// <summary>
        /// Create a new auto-flushing analytics manager
        /// </summary>
        /// <param name="internalManager">The manager to perform the autoflush on</param>
        /// <param name="flushDuration">The duration between the flush operations</param>
        /// <exception cref="NotSupportedException">Indicates the flush duration is invalid</exception>
        public AutoFlushAnalyticsManager(IAnalyticsManager internalManager, float flushDuration) : base(internalManager)
        {
            if (flushDuration <= 1)
            {
                throw new NotSupportedException("flushDuration must be >= 1");
            }
            _coroutineRunner = new GameObject("AutoflushAnalyticsManager")
                .AddComponent<AutoflushAnaltyticsManagerCoroutineRunner>();
            var gameObject = _coroutineRunner.gameObject;
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(gameObject);
            FlushDuration = flushDuration;
        }

        /// <summary>
        /// Initialize the contained manager
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            _coroutineRunner.StartCoroutine(AutoFlushCoroutine());
        }

        private IEnumerator AutoFlushCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(FlushDuration);
                Flush();
            }
        }
    }
}
