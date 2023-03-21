using UnityEngine;
using Unity.MLAgents;

namespace Unity.MLAgentsExamples
{
    /// <summary>
    /// This class contains logic for locomotion agents with joints which might make contact with the ground.
    /// By attaching this as a component to those joints, their contact with the ground can be used as either
    /// an observation for that agent, and/or a means of punishing the agent for making undesirable contact.
    /// </summary>
    [DisallowMultipleComponent]
    public class GroundContact : MonoBehaviour
    {
        [HideInInspector] 
        public Agent agent;

        // public GameObject PredStep;

        public PredictedStepSO predStepRef;

        [Header("Ground Check")] public bool agentDoneOnGroundContact; // Whether to reset agent on ground contact.
        public bool penalizeGroundContact; // Whether to penalize on contact.
        public float groundContactPenalty; // Penalty amount (ex: -1).
        public bool touchingGround;
        const string k_Ground = "ground"; // Tag of ground object.

        const float groundHeight = 0.568f; 
        const float stepSize = 2.0f;
        const float stepWidth = 1.0f;


        private void GenerateNewStep(string name)
        {
            predStepRef.DestroyStep();

            // normalized forward direction
            Vector3 forward = Vector3.Normalize(new Vector3(gameObject.transform.forward.x, 0.0f, gameObject.transform.forward.z));
            // rotate toward the goal with maximum delta corresponding to agent's speed

            // calculate left vector
            Vector3 left = Vector3.Cross(forward, Vector3.up);

            // spawn position
            Vector3 spawnPos = gameObject.transform.position + forward * stepSize;
            spawnPos += ((gameObject.name == "footR") ? left * stepWidth : -left * stepWidth);
            spawnPos.y = groundHeight;

            predStepRef.CreateStep(spawnPos, Quaternion.Euler(0, gameObject.transform.eulerAngles.y, 0));
        }


        public void Awake()
        {
            if ((gameObject.name == "footR") || (gameObject.name == "footL"))
            {
                GenerateNewStep(gameObject.name);
            }
        }


        /// <summary>
        /// Check for collision with ground, and optionally penalize agent.
        /// </summary>
        void OnCollisionEnter(Collision col)
        {
            if (col.transform.CompareTag(k_Ground))
            {
                if ((gameObject.name == "footR") || (gameObject.name == "footL"))
                {
                    float reward = (stepSize - predStepRef.Distance(gameObject.transform)) / stepSize;
                    agent.AddReward(reward);

                    GenerateNewStep(gameObject.name);
                    
                }

                touchingGround = true;
                if (penalizeGroundContact)
                {
                    agent.SetReward(groundContactPenalty);
                }

                if (agentDoneOnGroundContact)
                {
                    agent.EndEpisode();
                }
            }
        }

        /// <summary>
        /// Check for end of ground collision and reset flag appropriately.
        /// </summary>
        void OnCollisionExit(Collision other)
        {
            if (other.transform.CompareTag(k_Ground))
            {
                touchingGround = false;
            }
        }
    }
}
