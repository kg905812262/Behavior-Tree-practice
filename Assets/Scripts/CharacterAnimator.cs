using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour {
	
	private Animator animController;
	public enum MoveState { Idle, Walking, Running, Sprinting }
	public MoveState moveState;
	private NavMeshAgent navAgent;
	private float currentSpeed;
	private BehaviorTree behaviorTree;
	private SharedGameObject seenObject;
	private SharedBool targetFound;

	private void Awake()
	{
		animController = GetComponent<Animator>();
		navAgent = GetComponent<NavMeshAgent>();
		behaviorTree = GetComponent<BehaviorTree>();
	}
	private void Start()
	{
		currentSpeed = animController.GetFloat("Speed");
		seenObject = (SharedGameObject)behaviorTree.GetVariable("SeenObject");
		targetFound = (SharedBool)behaviorTree.GetVariable("TargetFound");
	}

	void Update()
	{
		var sqrDist = Vector3.SqrMagnitude(navAgent.destination - transform.position);
		animController.speed = navAgent.speed;
		// Character moves if there is a navigation path set, idle all other times
		if (navAgent.hasPath && targetFound.Value && sqrDist > Mathf.Pow(navAgent.stoppingDistance * 8f, 2f))
		{
			moveState = MoveState.Sprinting;
		}
		else if (navAgent.hasPath && targetFound.Value && sqrDist > Mathf.Pow(navAgent.stoppingDistance, 2f))
		{
			moveState = MoveState.Running;
		}
		else if (navAgent.hasPath && sqrDist > Mathf.Pow(navAgent.stoppingDistance, 2f))
		{
			moveState = MoveState.Walking;
		}
		else
		{
			moveState = MoveState.Idle;
		}
		Debug.DrawLine(transform.position, navAgent.destination, Color.red, 0.01f);
		// Send move state info to animator controller
		currentSpeed = Mathf.Lerp(currentSpeed, (float)moveState, navAgent.acceleration * Time.deltaTime);
		animController.SetFloat("Speed", currentSpeed);
		
	}

	void OnAnimatorMove()
	{
		// Only perform if not idling
		if (moveState != MoveState.Idle)
		{
			// Set the navAgent's velocity to the velocity of the animation clip currently playing
			navAgent.velocity = animController.deltaPosition / Time.deltaTime;
			// Set the rotation in the direction of movement
			Quaternion lookRotation = Quaternion.LookRotation(navAgent.desiredVelocity);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, navAgent.angularSpeed * Time.deltaTime);
		}
	}
}
