using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Agents;

namespace Tests
{
    public class MoverFromInputTests
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator move_agent_to_vector2()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            AgentConfig config = ScriptableObject.CreateInstance<AgentConfig>();
            config.MovementSpeed = 1;
            config.UsesRigidbody2D = true;
            GameObject go = new GameObject();
            Agents.Agent agent = go.AddComponent<Agents.Agent>();
            go.AddComponent<Rigidbody2D>();
            MoverFromInput mover = go.AddComponent<MoverFromInput>();
            mover.Init(agent, config);
            go.transform.position = Vector2.zero;
            mover.Move(new Vector2(5, 5));
            yield return new WaitForSeconds(1f);
            Assert.That((Vector2)go.transform.position != Vector2.zero);

            go.transform.position = Vector2.zero;
            go.GetComponent<Rigidbody2D>().AddForce(new Vector2(5, 5));
            yield return new WaitForSeconds(1f);
            Assert.That((Vector2)go.transform.position != Vector2.zero);
            Debug.Log(go.transform.position);
        }
    }
}
