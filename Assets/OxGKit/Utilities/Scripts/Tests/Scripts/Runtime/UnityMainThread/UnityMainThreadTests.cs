using NUnit.Framework;
using OxGKit.Utilities.UnityMainThread;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace OxGKit.Utilities.Tests
{
    public class UnityMainThreadTests
    {
        [SetUp]
        public void SetUp()
        {
            // 在每個測試之前初始化 UMT.worker
            _ = UMT.worker;
        }

        [TearDown]
        public void TearDown()
        {
        }

        [UnityTest]
        public IEnumerator UMTWorkerReturnsInstance()
        {
            // 確保 UMT.worker 返回一個實例
            Assert.IsNotNull(UMT.worker);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AddJobAndExecutesIt()
        {
            bool jobExecuted = false;

            // 添加一個任務, 任務會將 jobExecuted 設置為 true
            UMT.worker.AddJob(() =>
            {
                jobExecuted = true;
            });

            // 等待下一幀, 確保隊列中的任務被執行
            yield return null;

            // 驗證任務是否已經執行
            Assert.IsTrue(jobExecuted);
        }

        [UnityTest]
        public IEnumerator RunCoroutine()
        {
            bool routineExecuted = false;

            // 運行一個簡單的協程, 任務完成後將 routineExecuted 設置為 true
            UMT.worker.RunCoroutine(RunTestRoutine());

            // 等待協程完成
            yield return new WaitForSeconds(0.2f);

            // 驗證協程是否已成功執行
            Assert.IsTrue(routineExecuted);

            // 協程定義
            IEnumerator RunTestRoutine()
            {
                yield return new WaitForSeconds(0.1f);
                routineExecuted = true;
            }
        }

        [UnityTest]
        public IEnumerator StopCoroutine()
        {
            bool routineExecuted = false;
            IEnumerator routine = RunTestRoutine();

            // 啟動協程
            UMT.worker.RunCoroutine(routine);

            // 取消協程
            UMT.worker.CancelCoroutine(routine);

            // 等待一段時間, 確保協程被停止
            yield return new WaitForSeconds(0.1f);

            // 驗證協程未被執行
            Assert.IsFalse(routineExecuted);

            // 協程定義
            IEnumerator RunTestRoutine()
            {
                yield return new WaitForSeconds(0.2f);
                routineExecuted = true;
            }
        }

        [UnityTest]
        public IEnumerator CancelAllCoroutines()
        {
            bool executedFlag1 = false;
            bool executedFlag2 = false;

            // 啟動兩個協程
            UMT.worker.RunCoroutine(RunTestRoutine1());
            UMT.worker.RunCoroutine(RunTestRoutine2());

            // 取消所有協程
            UMT.worker.CancelAllCoroutines();

            // 等待一段時間, 確保協程被停止
            yield return new WaitForSeconds(0.1f);

            // 驗證所有協程都沒有執行
            Assert.IsFalse(executedFlag1);
            Assert.IsFalse(executedFlag2);

            // 協程定義
            IEnumerator RunTestRoutine1()
            {
                yield return new WaitForSeconds(0.2f);
                executedFlag1 = true;
            }

            IEnumerator RunTestRoutine2()
            {
                yield return new WaitForSeconds(0.2f);
                executedFlag2 = true;
            }
        }
    }
}
