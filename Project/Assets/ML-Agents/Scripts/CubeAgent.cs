using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CubeAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material rewardMaterial;
    [SerializeField] private MeshRenderer agentMeshRenderer;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject barrier;

    private bool isButtonAvailable;
    private bool isBarrierGone;
    private Rigidbody agentRigidBody;

    private void Awake()
    {
        agentRigidBody = GetComponent<Rigidbody>();
    }

    // Reset environment after every episode
    public override void OnEpisodeBegin()
    {
        agentRigidBody.velocity = Vector3.zero;
        transform.localEulerAngles = new Vector3(0, 180, 0);

        // Randomise agent, button, and target positions
        Vector3 agentPosition = new Vector3(Random.Range(0f, +2f), 0, Random.Range(-0.7f, +0.7f));
        Vector3 buttonPosition = new Vector3(Random.Range(-2f, -0.5f), -0.262f, Random.Range(-0.8f, +0.9f));
        Vector3 targetPosition = new Vector3(Random.Range(-2f, +2f), -0.059f, Random.Range(2.75f, 4.5f));
        transform.localPosition = agentPosition;
        button.transform.localPosition = buttonPosition;
        targetTransform.localPosition = targetPosition;

        // Reset button and barrier
        button.SetActive(true);
        isButtonAvailable = true;
        barrier.SetActive(true);
        isBarrierGone = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(isButtonAvailable ? 1 : 0);

        // Observations for button position
        Vector3 dirToButton = (button.transform.localPosition - transform.localPosition).normalized;
        sensor.AddObservation(dirToButton.x);
        sensor.AddObservation(dirToButton.z);

        // Check if barrier is deactivated
        sensor.AddObservation(isBarrierGone ? 1 : 0);
        if (isBarrierGone)
        {
            // Can reach target so set directions to target
            Vector3 dirToTarget = (targetTransform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dirToTarget.x);
            sensor.AddObservation(dirToTarget.z);
        }
        else
        {
            // Can not reach target so x & z are 0
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 addForce = new Vector3(0, 0, 0);
        
        // Set agent X direction movement
        int moveX = actions.DiscreteActions[0];
        switch (moveX)
        {
            case 0: 
                addForce.x = 0f; break;
            case 1:
                addForce.x = -1f; break;
            case 2:
                addForce.x = +1f; break;
        }

        // Set agent Z direction movement
        int moveZ = actions.DiscreteActions[1];
        switch (moveZ)
        {
            case 0:
                addForce.z = 0f; break;
            case 1:
                addForce.z = -1f; break;
            case 2:
                addForce.z = +1f; break;
        }

        // Move agent
        float movementSpeed = 2f;
        agentRigidBody.velocity = addForce * movementSpeed + new Vector3(0, agentRigidBody.velocity.y, 0);

        // Agent tries to press button
        bool isButtonPressed = actions.DiscreteActions[2] == 1;
        if (isButtonPressed)
        {
            Collider[] colliderArr = Physics.OverlapBox(transform.position, Vector3.one * 0.5f);
            foreach (Collider collider in colliderArr)
            {
                if (collider.TryGetComponent<Button>(out _))
                {
                    if (isButtonAvailable)
                    {
                        button.SetActive(false);
                        isButtonAvailable = false;
                        barrier.SetActive(false);
                        isBarrierGone = true;
                        AddReward(+1f); // Reward agent for button press
                    }
                }
            }
        }

        AddReward(-1f / MaxStep);   // Penalise long action times
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        // Move left & right
        int horizontalInput = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        switch (horizontalInput)
        {
            case -1:
                discreteActions[0] = 1; break;
            case 0:
                discreteActions[0] = 0; break;
            case +1:
                discreteActions[0] = 2; break;
        }

        // Move up & down
        int verticalInput = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        switch (verticalInput)
        {
            case -1:
                discreteActions[1] = 1; break;
            case 0:
                discreteActions[1] = 0; break;
            case +1:
                discreteActions[1] = 2; break;
        }

        // Press button
        discreteActions[2] = Input.GetKey(KeyCode.E) ? 1 : 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if agent has reached target
        if (collision.gameObject.TryGetComponent<Target>(out _))
        {
            agentMeshRenderer.material = rewardMaterial;
            AddReward(+1f);
            EndEpisode();   // End episode after target is reached
        }
    }
}
