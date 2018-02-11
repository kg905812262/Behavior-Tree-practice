using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour {

	//link to Animator component
	private Animator animController;
	//used to set anim controller parameters
	public enum MoveState { Idle, Walking, Running, Sprinting }
	public MoveState moveState;
	//link to NavMeshAgent component
	private NavMeshAgent navAgent;
	private float currentSpeed;
	private BehaviorTree behaviorTree;

	private void Awake()
	{
		animController = GetComponent<Animator>();
		navAgent = GetComponent<NavMeshAgent>();
		behaviorTree = GetComponent<BehaviorTree>();
	}
	private void Start()
	{
		currentSpeed = animController.GetFloat("Speed");
	}

	void Update()
	{
		//print(behaviorTree.FindTaskWithName("Can See Object").OnUpdate());
		var seenObject = (SharedGameObject)behaviorTree.GetVariable("SeenObject");
		var sqrDist = Vector3.SqrMagnitude(navAgent.destination - transform.position);
		animController.speed = navAgent.speed;
		//character walks if there is a navigation path set, idle all other times
		if (navAgent.hasPath && seenObject.Value != null && sqrDist > Mathf.Pow(navAgent.stoppingDistance * 8f, 2f))
		{
			moveState = MoveState.Sprinting;
		}
		else if (navAgent.hasPath && seenObject.Value != null && sqrDist > Mathf.Pow(navAgent.stoppingDistance * 5f, 2f))
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
		//send move state info to animator controller
		currentSpeed = Mathf.Lerp(currentSpeed, (float)moveState, navAgent.acceleration * Time.deltaTime);
		animController.SetFloat("Speed", currentSpeed);
		
	}

	void OnAnimatorMove()
	{
		//only perform if walking
		if (moveState != MoveState.Idle)
		{
			//set the navAgent's velocity to the velocity of the animation clip currently playing
			navAgent.velocity = animController.deltaPosition / Time.deltaTime;
			//set the rotation in the direction of movement
			Quaternion lookRotation = Quaternion.LookRotation(navAgent.desiredVelocity);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, navAgent.angularSpeed * Time.deltaTime);
		}
	}
}
