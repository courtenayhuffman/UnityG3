using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CubeAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform buttonTransform;
    [SerializeField] private Material rewardMaterial;
    [SerializeField] private Material penaltyMaterial;
    [SerializeField] private MeshRenderer agentMeshRenderer;

    public override void OnEpisodeBegin()
    {
        Vector3 agentPosition = new Vector3(Random.Range(0f, +2f), 0, Random.Range(-0.7f, +0.7f));
        Vector3 buttonPosition = new Vector3(Random.Range(-2f, -0.5f), -0.262f, Random.Range(-0.8f, +0.9f));
        Vector3 targetPosition = new Vector3(Random.Range(-2f, +2f), -0.189f, Random.Range(2.75f, 4.5f));
        
        transform.localPosition = agentPosition;
        buttonTransform.localPosition = buttonPosition;
        targetTransform.localPosition = targetPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(buttonTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 2f;
        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
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
