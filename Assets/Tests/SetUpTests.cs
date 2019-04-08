﻿using System.Collections; using NUnit.Framework; using UnityEngine; using UnityEngine.TestTools; using UnityEngine.SceneManagement;  namespace Tests {     public class SetUpTests     {          // A Test behaves as an ordinary method         [Test]         public void PlayModeTestsSimplePasses()         {             // Use the Assert class to test conditions         }          // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use         // `yield return null;` to skip a frame.         [UnityTest]         public IEnumerator PlayModeTestsWithEnumeratorPasses()         {             // Use the Assert class to test conditions.             // Use yield to skip a frame.             yield return null;         }          [UnityTest]         public IEnumerator GameObject_WithRigidBody_WillBeAffectedByPhysics()         {             var go = new GameObject();             go.AddComponent<Rigidbody>();             var originalPosition = go.transform.position.y;              yield return new WaitForFixedUpdate();              Assert.AreNotEqual(originalPosition, go.transform.position.y);         }           [UnityTest]         public IEnumerator LobbySceneSetUp()         {             LoadLobbyScene(this);             yield return null;         }          public static void LoadLobbyScene(SetUpTests instance)         {             SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);         }     } }   