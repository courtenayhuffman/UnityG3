using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CubeAgent : Agent
{
    // public event EventHandler eat;
     public event EventHandler beginEvent;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material rewardMaterial;
    [SerializeField] private Material penaltyMaterial;
    [SerializeField] private MeshRenderer agentMeshRenderer;

   // [SerializeField] private Button button;

    private Rigidbody agentRigidbody;

    
    private void Awake()
    {
        agentRigidbody = GetComponent<Rigidbody>();
    }
    
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(UnityEngine.Random.Range(-2f, +0.6f), 0, UnityEngine.Random.Range(-0.3f, +1.5f));
         targetTransform.localPosition = new Vector3(UnityEngine.Random.Range(-2f, +2f), 0, UnityEngine.Random.Range(-0.6f, +0.6f));
        beginEvent?.Invoke(this, EventArgs.Empty);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        /*
        sensor.AddObservation(button.CanUseButton() ? 1 : 0);

        Vector3 dirToButton = (button.transform.localPosition - transform.localPosition).normalized;
        sensor.AddObservation(dirToButton.x);
        sensor.AddObservation(dirToButton.z);
        */
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
         float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
         float moveSpeed = 2f;
        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        /*
        float moveX = actions.DiscreteActions[0];
        float moveZ = actions.DiscreteActions[1];

        Vector3 addForce = new Vector3(0, 0, 0);

        switch (moveX) {
            case 0: addForce.x = 0f; break;
            case 1: addForce.x = -1f; break;
            case 2: addForce.x = +1f; break;
        }

        switch (moveZ)
        {
            case 0: addForce.z = 0f; break;
            case 1: addForce.z = -1f; break;
            case 2: addForce.z = +1f; break;
        }

        float moveSpeed = 2f;
        agentRigidbody.velocity = addForce * moveSpeed + new Vector3(0, agentRigidbody.velocity.y, 0);
        */



    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
         ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
         continuousActions[0] = Input.GetAxisRaw("Horizontal");
          continuousActions[1] = Input.GetAxisRaw("Vertical");


        /*
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")))
        {
            case -1: discreteActions[0] = 1; break;
            case 0: discreteActions[0] = 0; break;
            case +1: discreteActions[0] = 2; break;
        }

        switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical")))
        {
            case -1: discreteActions[1] = 1; break;
            case 0: discreteActions[1] = 0; break;
            case +1: discreteActions[1] = 2; break;
        }

        discreteActions[2] = Input.GetKey(KeyCode.E) ? 1 : 0;
        */

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Button>(out Button target))
        {
            agentMeshRenderer.material = rewardMaterial;
            SetReward(+1f);
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            agentMeshRenderer.material = penaltyMaterial;
            SetReward(-1f);
            EndEpisode();
        }
    }
}
